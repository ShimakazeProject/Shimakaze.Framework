using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

namespace Shimakaze.Framework.Kernel.SourceGenerator;

public static class Diagnostics
{
    public static bool AssertSymbol<TSymbol>(in this SourceProductionContext ctx, ISymbol symbol, [NotNullWhen(true)] out TSymbol? targetSymbol)
        where TSymbol : ISymbol
    {
        if (symbol is TSymbol target)
        {
            targetSymbol = target;
            return true;
        }

        ctx.ReportDiagnostic(Diagnostic.Create(
            new(
                "SFKSG0001",
                "无法识别符号",
                "无法将 {0} 识别为 {1}，源生成器仅能在 {1} 上工作",
                "Shimakaze.Framework.Kernel.SourceGenerator",
                DiagnosticSeverity.Error,
                true),
            symbol.Locations.First(),
            symbol.GetType().Name,
            typeof(TSymbol).Name
        ));
        targetSymbol = default;
        return false;
    }

    public static bool AssertSetMethod(in this SourceProductionContext ctx, IPropertySymbol property, [NotNullWhen(true)] out IMethodSymbol? setMethod)
    {
        if (property.SetMethod is { })
        {
            setMethod = property.SetMethod;
            return true;
        }

        ctx.ReportDiagnostic(Diagnostic.Create(
            new(
                "SFKSG0002",
                "属性不包含 set 访问器",
                "属性 {0} 不包含 set 访问器",
                "Shimakaze.Framework.Kernel.SourceGenerator",
                DiagnosticSeverity.Error,
                true),
            property.Locations.First(),
            property.Name
        ));
        setMethod = default;
        return false;
    }

    public static bool AssertEventHandler(in this SourceProductionContext ctx, IEventSymbol @event, [NotNullWhen(true)] out INamedTypeSymbol? eventType)
    {
        if (@event.Type is INamedTypeSymbol type && type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).StartsWith("global::System.EventHandler"))
        {
            eventType = type;
            return true;
        }

        ctx.ReportDiagnostic(Diagnostic.Create(
            new(
                "SFKSG0003",
                "事件类型必须是 EventHandler",
                "事件 {0} 的类型不是 EventHandler",
                "Shimakaze.Framework.Kernel.SourceGenerator",
                DiagnosticSeverity.Error,
                true),
            @event.Locations.First(),
            @event.Name
        ));
        eventType = default;
        return false;
    }
}
