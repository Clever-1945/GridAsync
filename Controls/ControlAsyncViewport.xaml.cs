using AsyncUI.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncUI.Controls;

/// <summary>
/// Логика взаимодействия для ControlAsyncViewport.xaml
/// </summary>
public partial class ControlAsyncViewport : UserControl
{
    public AsyncAction AsyncAction { get; }

    private int _countProcess = 0;
    private int _zindex = 1;

    private Dictionary<string, Dictionary<Type, DependencyObject>> dictionaryObjects = null;
    private object lock_dictionaryObjects = new object();

    public ControlAsyncViewport()
    {
        AsyncAction = new AsyncAction();
        ApplyEvents();
        InitializeComponent();
        SizeChanged += (object sender, SizeChangedEventArgs e) => 
        {
            NameMiddleColumn.Width = new GridLength(e.NewSize.Width, GridUnitType.Pixel);
        };
    }

    public static readonly DependencyProperty InnerContentProperty = DependencyProperty
        .Register(
            "InnerContent",
            typeof(object),
            typeof(ControlAsyncViewport));

    public object InnerContent
    {
        get => GetValue(InnerContentProperty);
        set => SetValue(InnerContentProperty, value);
    }

    private void ApplyEvents()
    {
        AsyncAction.OnAlert = (m, d, ta) =>
        {
            var control = new ControlAlert(m, d, ta, (c) =>
            {
                if (MainGrid.Children.Contains(c))
                {
                    MainGrid.Children.Remove(c);
                }
            });
            control.SetValue(Grid.RowProperty, 0);
            control.SetValue(Grid.ColumnProperty, 1);
            control.SetValue(Panel.ZIndexProperty, _zindex++);

            MainGrid.Children.Add(control);
        };

        AsyncAction.OnStart = () =>
        {
            Interlocked.Add(ref _countProcess, 1);
            ApplyMask();
        };

        AsyncAction.OnDone = (ex) =>
        {
            Interlocked.Add(ref _countProcess, -1);
            ApplyMask();
            if (ex != null)
            {
                AsyncAction.Alert(ex.Message, ex.StackTrace, TypeAlert.Error);
            }
        };
    }

    private void ApplyMask()
    {
        if (_countProcess < 1)
        {
            IsEnabled = true;
            ProgressBarName.Visibility = Visibility.Collapsed;
        }
        else
        {
            IsEnabled = false;
            ProgressBarName.Visibility = Visibility.Visible;
        }
    }

    private Dictionary<string, Dictionary<Type, DependencyObject>> GetDictionaryObjects()
    {
        if (dictionaryObjects == null)
        {
            lock(lock_dictionaryObjects)
            {
                if (dictionaryObjects == null)
                {
                    var dictionary = new Dictionary<string, Dictionary<Type, DependencyObject>>(StringComparer.OrdinalIgnoreCase);
                    foreach(var dependencyObject in this.GetChildrenObjects())
                    {
                        var element = dependencyObject as UIElement;
                        if (element?.Uid != null)
                        {
                            Dictionary<Type, DependencyObject> dictionaryType = null;
                            if (!dictionary.TryGetValue(element.Uid, out dictionaryType))
                            {
                                dictionaryType = new Dictionary<Type, DependencyObject>();
                                dictionary[element.Uid] = dictionaryType;
                            }

                            dictionaryType[element.GetType()] = element;
                        }
                    }

                    dictionaryObjects = dictionary;
                }
            }
        }

        return dictionaryObjects;
    }

    public T GetElementGyByUid<T>(string uid) where T : class
    {
        if (String.IsNullOrWhiteSpace(uid))
            return default(T);

        var dictionary = GetDictionaryObjects();
        var element = dictionary.GetValueOrDefault(uid)?.GetValueOrDefault(typeof(T));
        return element as T;
    }
}
