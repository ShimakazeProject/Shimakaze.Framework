using Shimakaze.Framework.Controls;

namespace Shimakaze.Framework;

public abstract partial class Application
{
    [EventMethod]
    public event EventHandler? Initialize;

    public abstract void AddWindow(Window window);
    public abstract void MainLoop();
    public abstract void Stop();
}
