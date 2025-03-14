using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

namespace Shimakaze.Framework.Kernel.SourceGenerator;

public static class ContextExtension
{
    public static string GetAccessibility(this ISymbol symbol, bool @public = true) => symbol.DeclaredAccessibility switch
    {
        Accessibility.Private => "private",
        Accessibility.ProtectedAndInternal => "private protected",
        Accessibility.Protected => "protected",
        Accessibility.Internal => "internal",
        Accessibility.ProtectedOrInternal => "protected internal",
        Accessibility.Public when @public => "public",
        _ => string.Empty,
    };

    public static void AddSourceWithType(this SourceProductionContext context, INamedTypeSymbol type, string name, string content)
    {
        var ns = type.ContainingNamespace.ToDisplayString();
        StringBuilder code = new();
        code.AppendLine("#nullable enable");
        code.AppendLine();
        code.Append("namespace ");
        code.AppendLine(ns);
        code.AppendLine("{");
        code.Append("    ");
        code.Append(type.GetAccessibility());
        code.Append(' ');
        if (type.IsAbstract)
            code.Append("abstract ");
        if (type.IsSealed)
            code.Append("sealed ");
        code.Append("partial ");
        if (type.IsReadOnly)
            code.Append("readonly ");
        if (type.IsRecord)
            code.Append("record ");
        if (type.IsValueType)
            code.Append("struct ");
        else
            code.Append("class ");
        code.AppendLine(type.Name);
        code.AppendLine("    {");
        using StringReader sr = new(content);
        while (sr.Peek() is not -1)
        {
            code.Append("        ");
            code.AppendLine(sr.ReadLine());
        }
        code.AppendLine("    }");
        code.AppendLine("}");

        context.AddSource($"{ns}.{type.Name}.{name}.g.cs", code.ToString());
    }
}