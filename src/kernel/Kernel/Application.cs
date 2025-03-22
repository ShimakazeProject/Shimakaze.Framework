using System.Diagnostics.CodeAnalysis;

using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework;

public abstract partial class Application
{
    [EventMethod]
    public event EventHandler? Initialize;

    [AllowNull]
    public Dispatcher Dispatcher { get; protected set; }

    public abstract void AddWindow(Window window);

    public abstract void Run();

    public abstract void Stop();
}
