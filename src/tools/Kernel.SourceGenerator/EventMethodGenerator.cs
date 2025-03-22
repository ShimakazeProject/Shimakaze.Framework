using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Shimakaze.Framework.Kernel.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class EventMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterImplementationSourceOutput(
            context.SyntaxProvider.ForAttributeWithMetadataName(
                "Shimakaze.Framework.EventMethodAttribute",
                (i, _) => i is not null,
                (i, _) => i),
            (context, data) =>
            {
                if (!context.AssertSymbol<IEventSymbol>(data.TargetSymbol, out var @event))
                    return;

                if (!context.AssertEventHandler(@event, out var eventType))
                    return;

                var sender = @event.IsStatic ? "default" : "this";
                var method = eventType.IsGenericType
                    ? $"protected virtual void On{@event.Name}({eventType.TypeArguments.First().ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} args) => {@event.Name}?.Invoke({sender}, args);"
                    : $"protected virtual void On{@event.Name}() => {@event.Name}?.Invoke({sender}, global::System.EventArgs.Empty);";

                context.AddSourceWithType(
                     @event.ContainingType,
                     @event.Name,
                     method);
            });
    }
}
