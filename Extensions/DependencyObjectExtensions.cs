using AsyncUI.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AsyncUI.Extensions;

public static class DependencyObjectExtensions
{
    /// <summary>
    /// Получить все родительские элементы
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static IEnumerable<DependencyObject> GetChildrenObjects(this DependencyObject instance)
    {
        if (instance != null)
        {
            var count = VisualTreeHelper.GetChildrenCount(instance);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(instance, i);
                yield return child;

                foreach(var childNext in child.GetChildrenObjects())
                {
                    yield return childNext;
                }
            }
        }
    }
}
