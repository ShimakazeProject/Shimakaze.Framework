using System.Diagnostics.CodeAnalysis;

namespace Shimakaze.Framework;

public sealed class DispatchedHandler
{
    private readonly Func<CancellationToken, object>? _cancelableFunc;
    private readonly Func<object>? _func;
    private readonly Action<CancellationToken>? _cancelableAction;
    private readonly Action? _action;

    private readonly TaskCompletionSource<object>? _funcTcs;
    private readonly TaskCompletionSource? _actionTcs;

    public Task? Task => _funcTcs?.Task ?? _actionTcs?.Task;
    public Task<object>? ReturnableTask => _funcTcs?.Task;

    [MemberNotNullWhen(true, nameof(_cancelableFunc), nameof(_funcTcs))]
    private bool IsCancelableFunc => _cancelableFunc is not null;

    [MemberNotNullWhen(true, nameof(_func), nameof(_funcTcs))]
    private bool IsFunc => _func is not null;

    [MemberNotNullWhen(true, nameof(_cancelableAction))]
    private bool IsCancelableAction => _cancelableAction is not null;

    [MemberNotNullWhen(true, nameof(_action))]
    private bool IsAction => _action is not null;

    public DispatchedHandler(Func<CancellationToken, object> cancelableFunc)
    {
        _funcTcs = new();
        _cancelableFunc = cancelableFunc;
    }
    public static implicit operator DispatchedHandler(Func<CancellationToken, object> func) => new(func);

    public DispatchedHandler(Func<object> func)
    {
        _funcTcs = new();
        _func = func;
    }

    public static implicit operator DispatchedHandler(Func<object> func) => new(func);

    public DispatchedHandler(Action<CancellationToken> cancelableAction, bool isWaitable)
    {
        _cancelableAction = cancelableAction;
        if (isWaitable)
            _actionTcs = new();
    }

    public static implicit operator DispatchedHandler(Action<CancellationToken> action) => new(action, true);

    public DispatchedHandler(Action action, bool isWaitable)
    {
        _action = action;
        if (isWaitable)
            _actionTcs = new();
    }

    public static implicit operator DispatchedHandler(Action action) => new(action, true);

    public void Invoke()
    {
        if (IsCancelableFunc || IsCancelableAction)
        {
            Invoke(CancellationToken.None);
            return;
        }

        if (IsFunc)
        {
            try
            {
                _funcTcs.SetResult(_func.Invoke());
            }
            catch (Exception e)
            {
                _funcTcs.SetException(e);
            }
        }

        if (IsAction)
        {
            try
            {
                _action.Invoke();
                _actionTcs?.SetResult();
            }
            catch (Exception e)
            {
                _actionTcs?.SetException(e);
            }
        }
    }

    public void Invoke(CancellationToken cancellationToken = default)
    {
        if (IsCancelableFunc)
        {
            try
            {
                _funcTcs.SetResult(_cancelableFunc.Invoke(cancellationToken));
            }
            catch (Exception e)
            {
                _funcTcs.SetException(e);
            }
        }
        else if (IsCancelableAction)
        {
            try
            {
                _cancelableAction.Invoke(cancellationToken);
                _actionTcs?.SetResult();
            }
            catch (Exception e)
            {
                _actionTcs?.SetException(e);
            }
        }
    }
}
