using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Shimakaze.Framework.Kernel.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class PropertyChangedEventGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterImplementationSourceOutput(
            context.SyntaxProvider.ForAttributeWithMetadataName(
                "Shimakaze.Framework.PropertyChangedEventAttribute",
                (i, _) => i.IsKind(SyntaxKind.PropertyDeclaration),
                (i, _) => i),
            (context, data) =>
            {
                if (!context.AssertSymbol<IPropertySymbol>(data.TargetSymbol, out var property))
                    return;

                var type = $"{property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}{(property.NullableAnnotation is NullableAnnotation.Annotated ? "?" : "")}";

                StringBuilder code = new();
                if (property.IsPartialDefinition)
                {
                    if (!context.AssertSetMethod(property, out var setMethod))
                        return;

                    code.Append(property.GetAccessibility());
                    code.Append(' ');
                    if (property.IsVirtual)
                        code.Append("virtual ");
                    code.Append("partial ");
                    code.Append(type);
                    code.Append(' ');
                    code.AppendLine(property.Name);
                    code.AppendLine("{");
                    code.AppendLine("    get => field;");
                    code.Append("    ");
                    var accessibility = setMethod.GetAccessibility(@public: false);
                    if (!string.IsNullOrEmpty(accessibility))
                    {
                        code.Append(setMethod.GetAccessibility(@public: false));
                        code.Append(' ');
                    }
                    code.AppendLine("set");
                    code.AppendLine("    {");
                    code.AppendLine("        if (field == value)");
                    code.AppendLine("            return;");
                    code.AppendLine("        var old = field;");
                    code.AppendLine("        field = value;");
                    code.Append("        On");
                    code.Append(property.Name);
                    code.AppendLine("Changed(new(old, value));");
                    code.AppendLine("    }");
                    code.AppendLine("}");
                    code.AppendLine();
                }
                code.Append("public event global::System.EventHandler<global::Shimakaze.Framework.ChangedEventArgs<");
                code.Append(type);
                code.Append(">>? ");
                code.Append(property.Name);
                code.AppendLine("Changed;");
                code.AppendLine();

                code.Append("protected virtual void On");
                code.Append(property.Name);
                code.Append("Changed(global::Shimakaze.Framework.ChangedEventArgs<");
                code.Append(type);
                code.Append("> eventArgs) => ");
                code.Append(property.Name);
                code.Append("Changed?.Invoke(this, eventArgs);");

                context.AddSourceWithType(
                    property.ContainingType,
                    property.Name,
                    code.ToString());
            });
    }

}
