using Shimakaze.Framework.Controls;
using Shimakaze.Framework.Resources;

namespace Shimakaze.Framework;

public abstract partial class Application
{
    public static Application? Instance { get; private set; }

    public Dispatcher Dispatcher { get; }

    [EventMethod]
    public event EventHandler? Initialize;

    protected Application(Dispatcher dispatcher)
    {
        if (Instance is not null)
            throw new ApplicationException(Resource.SecondApplicationError);

        Instance = this;
        Dispatcher = dispatcher;
    }

    public abstract Window CreateWindow(string title);

    public Window CreateWindow(string title, Action<Window> initialize)
    {
        var window = CreateWindow(title);
        initialize(window);
        return window;
    }

    public abstract void Run();

    public void Run(Action<Application> initialize)
    {
        Initialize += (_, _) => initialize(this);
        Run();
    }

    public abstract void Stop();
}
