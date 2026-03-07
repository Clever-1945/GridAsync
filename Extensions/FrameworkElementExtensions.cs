using System.Windows;
using System.Windows.Media;

namespace AsyncUI.Extensions;

public static class FrameworkElementExtensions
{
    /// <summary> Найти асинхронную операцию </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static Lazy<AsyncAction> GetAsyncAction(this FrameworkElement instance)
    {
        return new Lazy<AsyncAction>(() =>
        {
            return instance.FindAllGrids().FirstOrDefault()?.AsyncAction;
        });
    }
    
    private static IEnumerable<GridAsync.Controls.GridAsync> FindAllGrids(this DependencyObject parent)
    {
        if (parent == null)
        {
            yield break;
        }

        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            
            if (child is GridAsync.Controls.GridAsync grid)
            {
                yield return grid;
            }

            foreach (var innerGrid in FindAllGrids(child))
            {
                yield return innerGrid;
            }
        }
    }
}