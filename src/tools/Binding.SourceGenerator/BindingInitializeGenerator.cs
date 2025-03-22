using System.Threading;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

namespace Shimakaze.Framework.Binding.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class BindingInitializeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context =>
        {
            context.AddSource(
                "InterceptsLocationAttribute.g.cs",
                """
                using System;

                #nullable enable

                namespace System.Runtime.CompilerServices;

                [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                #pragma warning disable CS9113
                internal sealed class InterceptsLocationAttribute(int version, string data) : Attribute
                #pragma warning restore CS9113
                {
                }
                """);
            context.AddSource(
                "BindingType.g.cs",
                """
                using System;
                                
                #nullable enable

                namespace Shimakaze.Framework.Controls;

                internal enum BindingType
                {
                    Auto = 0,
                    OneWay = 1,
                    OneWayToSource = 2,
                    TwoWay = 3,
                }
                """);
            context.AddSource(
                "BindingExtensions.g.cs",
                """
                using System;
                using System.Diagnostics;
                using System.Linq.Expressions;

                using Shimakaze.Framework.Controls;

                namespace Shimakaze.Framework.Controls;
                                
                #nullable enable

                internal static partial class BindingExtensions
                {
                    public static void Bind<TTargetElement, TTargetType, TSourceElement, TSourceType>(
                        this TTargetElement targetElement,
                        Expression<Func<TTargetElement, TTargetType>> target,
                        TSourceElement sourceElement,
                        Expression<Func<TSourceElement, TSourceType>> source,
                        Func<TSourceType, TTargetType>? sourceConverter = null,
                        Func<TTargetType, TSourceType>? targetConverter = null,
                        BindingType bindingType = BindingType.Auto)
                        where TTargetElement : UIElement
                        where TSourceElement : UIElement
                    {
                        Debug.Assert(false);
                        throw new NotImplementedException("Bind failed");
                    }
                }
                """);
        });
    }
}
