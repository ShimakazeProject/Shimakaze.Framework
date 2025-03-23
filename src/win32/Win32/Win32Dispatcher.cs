using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Shimakaze.Framework.Win32;

public sealed class Win32Dispatcher : Dispatcher
{
    private readonly ConcurrentDictionary<DispatcherPriority, ConcurrentQueue<DispatchedHandler>> _tasks = [];
    private uint _threadId = 0;
    internal const uint WM_TASK = PInvoke.WM_USER + 1;

    private volatile nint _loopCount = 0;

    protected override void Enqueue(DispatcherPriority priority, DispatchedHandler handler)
    {
        if (!_tasks.TryGetValue(priority, out var queue))
            _tasks[priority] = queue = [];

        queue.Enqueue(handler);

        if (!PInvoke.PostThreadMessage(_threadId, WM_TASK, 0, 0))
            throw new Win32Exception();
    }

    private bool TryDequeue(DispatcherPriority priority, [NotNullWhen(true)] out DispatchedHandler? handler)
    {
        if (!_tasks.TryGetValue(priority, out var queue))
        {
            handler = null;
            return false;
        }

        return queue.TryDequeue(out handler);
    }

    protected override void MainLoop()
    {
        base.MainLoop();
        if (Application.Instance is not Win32Application app)
            throw new InvalidOperationException();

        if (OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
            _threadId = PInvoke.GetCurrentThreadId();

        app.OnInitialize();
        while (true)
        {
            if (!PInvoke.PeekMessage(out var msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE))
            {
                if (TryDequeue(DispatcherPriority.Idle, out var handler))
                {
                    // 没有消息时尝试调用闲置任务
                    handler.Invoke();
                }
                else
                {
                    // 没任务可执行时等待下一个消息
                    if (!PInvoke.WaitMessage())
                        throw new Win32Exception();
                }

                continue;
            }

            if (msg.message is PInvoke.WM_QUIT)
            {
                // Flush All Tasks
                while (TryDequeue(DispatcherPriority.High, out var handler)
                    || TryDequeue(DispatcherPriority.Normal, out handler)
                    || TryDequeue(DispatcherPriority.Low, out handler)
                    || TryDequeue(DispatcherPriority.Idle, out handler))
                    handler.Invoke();

                break;
            }
            else if (msg.message is WM_TASK)
            {
                if (TryDequeue(DispatcherPriority.High, out var handler))
                    handler.Invoke();
            }
            else
            {
                if ((_loopCount & 0b1000) is not 0)
                {
                    if (TryDequeue(DispatcherPriority.Normal, out var handler))
                        handler.Invoke();
                }
                else if ((_loopCount & 0b10000) is not 0)
                {
                    if (TryDequeue(DispatcherPriority.Low, out var handler))
                        handler.Invoke();
                }
            }

            PInvoke.TranslateMessage(msg);
            PInvoke.DispatchMessage(msg);
            unchecked
            {
                _loopCount++;
            }

            if (app.ShouldQuit)
                PInvoke.PostQuitMessage(0);
        }
    }
}
