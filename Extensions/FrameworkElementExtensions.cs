using AsyncUI.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AsyncUI.Extensions;

public static class FrameworkElementExtensions
{
    /// <summary> От текущего элемента найти родителя, и в нем асинхронную операцию </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static AsyncAction GetAsyncAction(this FrameworkElement instance)
    {
        var current = instance;
        do
        {
            if (current is ControlAsyncViewport)
            {
                return (current as ControlAsyncViewport).AsyncAction;
            }

            current = current?.Parent as FrameworkElement ?? GetVisualParent(current);
        } while (current != null);
        return null;
    }

    private static FrameworkElement GetVisualParent(this FrameworkElement instance)
    {
        if (instance == null)
            return null;

        var property = typeof(Visual).GetProperty("VisualParent", BindingFlags.Instance | BindingFlags.NonPublic);
        var element = property.GetValue(instance) as FrameworkElement;
        return element;
    }
}
