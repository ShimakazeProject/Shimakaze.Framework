using Microsoft.CodeAnalysis;

namespace Shimakaze.Framework.Kernel.SourceGenerator;
[Generator(LanguageNames.CSharp)]
public sealed class EventAttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context =>
        {
            context.AddSource(
                "PropertyChangedEventAttribute.g.cs",
                """
                using System;

                #nullable enable

                namespace Shimakaze.Framework;

                [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
                internal sealed class PropertyChangedEventAttribute : Attribute
                {
                }
                """);
            context.AddSource(
                "EventMethodAttribute.g.cs",
                """
                using System;

                #nullable enable

                namespace Shimakaze.Framework;
                                
                [AttributeUsage(AttributeTargets.Event, Inherited = false, AllowMultiple = false)]
                internal sealed class EventMethodAttribute : Attribute
                {
                }
                """);
        });
    }
}
