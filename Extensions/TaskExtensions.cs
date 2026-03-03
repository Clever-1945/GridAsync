using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AsyncUI.Extensions;

public static class TaskExtensions
{
    /// <summary>
    /// Выполнить действие, если задача считается выполненой
    /// Фильтр решает выполнена ли задача
    /// </summary>
    /// <typeparam name="RT"></typeparam>
    /// <param name="task"></param>
    /// <param name="filter"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task<RT> OnCompleted<RT>(this Task<RT> task, Func<Task<RT>, bool> filter, Action<Task<RT>> action)
    {
        task.GetAwaiter().OnCompleted(() =>
        {
            if (filter(task))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    action?.Invoke(task);
                });
            }
        });
        return task;
    }

    /// <summary>
    /// Выполнить действие, если задача считается выполненой
    /// Фильтр решает выполнена ли задача
    /// </summary>
    /// <param name="task"></param>
    /// <param name="filter"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task OnCompleted(this Task task, Func<Task, bool> filter, Action<Task> action)
    {
        task.GetAwaiter().OnCompleted(() =>
        {
            if (filter(task))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    action?.Invoke(task);
                });
            }
        });
        return task;
    }

    /// <summary>
    /// Вызвать действие после успешного выполнения задачи
    /// </summary>
    /// <typeparam name="RT"></typeparam>
    /// <param name="task"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task<RT> Then<RT>(this Task<RT> task, Action<RT> action)
    {
        return task.OnCompleted((t) => t.IsCompletedSuccessfully, (t) => action?.Invoke(t.Result));
    }

    /// <summary>
    /// Вызвать действие после успешного выполнения задачи
    /// </summary>
    /// <param name="task"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task Then(this Task task, Action action)
    {
        return task.OnCompleted((t) => t.IsCompletedSuccessfully, (t) => action?.Invoke());
    }

    /// <summary>
    /// Вызвать действие если задача завершилась аварийно
    /// </summary>
    /// <typeparam name="RT"></typeparam>
    /// <param name="task"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task<RT> Catch<RT>(this Task<RT> task, Action<Exception> action)
    {
        return task.OnCompleted((t) => t.Status == TaskStatus.Faulted, (t) =>
        {
            action?.Invoke(t.Exception?.InnerException);
        });
    }

    /// <summary>
    /// Вызвать действие если задача завершилась аварийно
    /// </summary>
    /// <param name="task"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task Catch(this Task task, Action<Exception> action)
    {
        return task.OnCompleted((t) => t.Status == TaskStatus.Faulted, (t) =>
        {
            action?.Invoke(t.Exception?.InnerException);
        });
    }

    /// <summary>
    /// Выполнить действие в любом случии. После аварийного или успешного завершения задачи
    /// </summary>
    /// <typeparam name="RT"></typeparam>
    /// <param name="task"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task<RT> Done<RT>(this Task<RT> task, Action<TaskDoneInfo<RT>> action)
    {
        return task.OnCompleted((t) => t.Status == TaskStatus.Faulted || t.IsCompletedSuccessfully, (t) =>
        {
            action?.Invoke(new TaskDoneInfo<RT>
            {
                Success = t.IsCompletedSuccessfully,
                Exception = t.Status == TaskStatus.Faulted ? t.Exception?.InnerException : null,
                Data = t.IsCompletedSuccessfully ? task.Result : default(RT)
            });
        });
    }

    /// <summary>
    /// Выполнить действие в любом случии. После аварийного или успешного завершения задачи
    /// </summary>
    /// <param name="task"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task Done(this Task task, Action<Exception> action)
    {
        return task.OnCompleted((t) => t.Status == TaskStatus.Faulted || t.IsCompletedSuccessfully, (t) =>
        {
            action?.Invoke(t.Status == TaskStatus.Faulted ? t.Exception?.InnerException : null);
        });
    }
}
