using System;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Shimakaze.Framework.Binding.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class BindingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
            (node, cancellationToken) =>
            {
                if (!node.IsKind(SyntaxKind.InvocationExpression))
                    return false;

                if (node is not InvocationExpressionSyntax invocationExpression)
                    return false;

                if (!invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
                    return false;

                if (invocationExpression.Expression is not MemberAccessExpressionSyntax memberAccessExpression)
                    return false;

                if (memberAccessExpression.Name.Identifier.Text is not "Bind")
                    return false;

                if (invocationExpression.ArgumentList.Arguments is not { Count: not 0 } arguments)
                    return false;

                return true;
            },
            (context, cancellationToken) =>
            {
                var invocationExpression = (InvocationExpressionSyntax)context.Node;
                var info = context.SemanticModel.GetSymbolInfo(context.Node);
                if (info.Symbol is not IMethodSymbol method)
                    return default;

                if (method.TypeArguments is not { Length: 4 } types)
                    return default;

                if (context.SemanticModel.GetInterceptableLocation(invocationExpression) is not { } location)
                    return default;

                var arguments = invocationExpression.ArgumentList.Arguments;
                int baseIndex = 0;
                if (arguments[0].Expression.IsKind(SyntaxKind.SimpleLambdaExpression))
                    // 扩展方法调用
                    baseIndex = 0;
                else if (arguments[0].Expression.IsKind(SyntaxKind.IdentifierName))
                    // 静态方法调用
                    baseIndex = 1;

                var targetPropertyName = GetPropertyName(arguments[0 + baseIndex]);
                var sourcePropertyName = GetPropertyName(arguments[2 + baseIndex]);
                var bindingType = GetBindingType(arguments, baseIndex) switch
                {
                    "OneWay" => BindingType.OneWay,
                    "OneWayToSource" => BindingType.OneWayToSource,
                    "TwoWay" => BindingType.TwoWay,
                    _ => BindingType.TwoWay,
                };

                ITypeSymbol targetElement = types[0];
                ITypeSymbol targetType = types[1];
                ITypeSymbol sourceElement = types[2];
                ITypeSymbol sourceType = types[3];

                var sourceEvent = GetEventSymbol(sourceElement).FirstOrDefault(i => i.Name == $"{sourcePropertyName}Changed");
                if (sourceEvent is not null)
                    bindingType |= BindingType.OneWay;
                else
                    bindingType &= ~BindingType.OneWay;

                var targetEvent = GetEventSymbol(targetElement).FirstOrDefault(i => i.Name == $"{targetPropertyName}Changed");
                if (targetEvent is not null)
                    bindingType |= BindingType.OneWayToSource;
                else
                    bindingType &= ~BindingType.OneWayToSource;

                if (bindingType is BindingType.Auto)
                    return default;

                var name = $"Bind{bindingType}_{sourceElement.Name}{sourcePropertyName}To{targetElement.Name}{targetPropertyName}";

                return new BindingInfo(
                    name, location.GetInterceptsLocationAttributeSyntax(),
                    targetElement, targetType, targetPropertyName, targetEvent,
                    sourceElement, sourceType, sourcePropertyName, sourceEvent,
                    bindingType);
            })
            .Collect();

        context.RegisterImplementationSourceOutput(provider, (context, data) =>
        {
            foreach (var group in data.OfType<BindingInfo>().GroupBy(i => i.Name))
            {
                StringBuilder sb = new("""
                    using System;
                    using System.ComponentModel;
                    using System.Diagnostics;
                    using System.Linq.Expressions;
                    using System.Runtime.CompilerServices;
                    
                    namespace Shimakaze.Framework.Controls;
                    
                    #nullable enable

                    """);
                sb.AppendLine($"internal static partial class {group.Key}Bindings");
                sb.AppendLine("{");
                sb.AppendLine();

                var first = group.First();
                var targetElementType = first.TargetElement.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var targetType = first.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var sourceElementType = first.SourceElement.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var sourceType = first.SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (group.Any())
                {
                    foreach (var info in group)
                        sb.AppendLine($"    {info.InterceptsLocation}");
                    sb.AppendLine("    [EditorBrowsable(EditorBrowsableState.Never)]");
                    sb.Append($"    public static void {group.Key}(");
                    sb.Append($"this {targetElementType} targetElement, ");
                    sb.Append($"Expression<Func<{targetElementType}, {targetType}>> target, ");
                    sb.Append($"{sourceElementType} sourceElement, ");
                    sb.Append($"Expression<Func<{sourceElementType}, {sourceType}>> source, ");
                    sb.Append($"Func<{sourceType}, {targetType}>? sourceConverter = null, ");
                    sb.Append($"Func<{targetType}, {sourceType}>? targetConverter = null, ");
                    sb.AppendLine("BindingType bindingType = BindingType.Auto)");
                    sb.AppendLine("    {");
                    sb.AppendLine($"        EventHandler<ChangedEventArgs<{sourceType}>> oneWay = default!;");
                    sb.AppendLine($"        EventHandler<ChangedEventArgs<{targetType}>> oneWayToSource = default!;");
                    if (first.BindingType.HasFlag(BindingType.OneWay))
                    {
                        sb.AppendLine($"        if (sourceConverter is null)");
                        sb.AppendLine($"            sourceConverter = static i => ({targetType})i;");
                        sb.AppendLine($"        oneWay = (_, e) =>");
                        sb.AppendLine("        {");
                        if (first.BindingType.HasFlag(BindingType.OneWayToSource))
                            sb.AppendLine($"            targetElement.{first.TargetPropertyName}Changed -= oneWayToSource;");
                        sb.AppendLine($"            targetElement.{first.TargetPropertyName} = sourceConverter(e.New);");
                        if (first.BindingType.HasFlag(BindingType.OneWayToSource))
                            sb.AppendLine($"            targetElement.{first.TargetPropertyName}Changed += oneWayToSource;");
                        sb.AppendLine("        };");
                        sb.AppendLine($"        sourceElement.{first.SourcePropertyName}Changed += oneWay;");
                    }
                    if (first.BindingType.HasFlag(BindingType.OneWayToSource))
                    {
                        sb.AppendLine($"        if (targetConverter is null)");
                        sb.AppendLine($"            targetConverter = static i => ({sourceType})i;");
                        sb.AppendLine($"        oneWayToSource = (_, e) =>");
                        sb.AppendLine("        {");
                        if (first.BindingType.HasFlag(BindingType.OneWay))
                            sb.AppendLine($"            sourceElement.{first.SourcePropertyName}Changed -= oneWay;");
                        sb.AppendLine($"            sourceElement.{first.SourcePropertyName} = targetConverter(e.New);");
                        if (first.BindingType.HasFlag(BindingType.OneWay))
                            sb.AppendLine($"            sourceElement.{first.SourcePropertyName}Changed += oneWay;");
                        sb.AppendLine("        };");
                        sb.AppendLine($"        targetElement.{first.TargetPropertyName}Changed += oneWayToSource;");
                    }

                    sb.AppendLine("    }");
                    sb.AppendLine();
                }

                sb.AppendLine();
                sb.AppendLine("}");

                context.AddSource($"{group.Key}.g.cs", sb.ToString());
            }
        });
    }

    private static string? GetPropertyName(ArgumentSyntax argument)
    {
        if (argument.Expression is not SimpleLambdaExpressionSyntax { ExpressionBody: MemberAccessExpressionSyntax memberAccess })
            return null;

        return memberAccess.Name.Identifier.Text;
    }

    private static string? GetBindingType(SeparatedSyntaxList<ArgumentSyntax> arguments, int baseIndex)
    {
        var bindingTypeSyntax = arguments.FirstOrDefault(i => i.NameColon?.Expression is IdentifierNameSyntax { Identifier.Text: "bindingType" });
        if (bindingTypeSyntax is null && arguments.Count >= baseIndex + 6)
            bindingTypeSyntax = arguments[baseIndex + 5];

        return (bindingTypeSyntax?.Expression as MemberAccessExpressionSyntax)?.Name.Identifier.Text;
    }

    private IEnumerable<IEventSymbol> GetEventSymbol(ITypeSymbol type)
    {
        IEnumerable<IEventSymbol> symbols = [];
        if (type.BaseType is not null)
            symbols = GetEventSymbol(type.BaseType);

        symbols = symbols.Concat(type.GetMembers().OfType<IEventSymbol>());
        return symbols;
    }
}

file sealed record class BindingInfo(
    string Name, string InterceptsLocation,
    ITypeSymbol TargetElement, ITypeSymbol TargetType, string? TargetPropertyName, IEventSymbol? TargetEvent,
    ITypeSymbol SourceElement, ITypeSymbol SourceType, string? SourcePropertyName, IEventSymbol? SourceEvent,
    BindingType BindingType);

[Flags]
file enum BindingType
{
    Auto = 0,
    OneWay = 1,
    OneWayToSource = 2,
    TwoWay = OneWay + OneWayToSource,
}