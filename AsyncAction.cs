using AsyncUI.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AsyncUI;

public class AsyncAction
{
    internal Action OnStart;
    internal Action<Exception> OnDone;
    internal Action<string, string, TypeAlert> OnAlert;

    public AsyncAction()
    {
    }

    public void Alert(string message, string description, TypeAlert typeAlert)
    {
        OnAlert?.Invoke(message, description, typeAlert);
    }

    public Task<RT> StartAsync<RT>(Func<Task<RT>> fn)
    {
        var tc = new TaskCompletionSource<RT>();

        Application.Current.Dispatcher.Invoke(OnStart);
        ThreadPool.QueueUserWorkItem(async (s) =>
        {
            try
            {
                var result = await fn();
                Application.Current.Dispatcher.Invoke(() => OnDone?.Invoke(null));
                tc.TrySetResult(result);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => OnDone?.Invoke(ex));
                tc.TrySetException(ex);
            }
        });

        return tc.Task;
    }
    
    public Task StartAsync(Func<Task> fn)
    {
        var tc = new TaskCompletionSource();

        Application.Current.Dispatcher.Invoke(OnStart);
        ThreadPool.QueueUserWorkItem(async (s) =>
        {
            try
            {
                await fn();
                Application.Current.Dispatcher.Invoke(() => OnDone?.Invoke(null));
                tc.TrySetResult();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => OnDone?.Invoke(ex));
                tc.TrySetException(ex);
            }
        });

        return tc.Task;
    }
}
