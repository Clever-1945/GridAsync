using System.Windows;
using System.Windows.Controls;
using AsyncUI;
using AsyncUI.Controls;

namespace GridAsync.Controls;

public class GridAsync: Grid
{
    private int _countRow = 0;
    private int _countColumn = 0;
    private int _countProcess = 0;
    private ControlLoadingMask _controlMask = null;

    public AsyncAction AsyncAction { get; }

    public GridAsync()
    {
        this.LayoutUpdated += OnLayoutUpdated;
        AsyncAction = new AsyncAction();
        ApplyEvents();
    }

    private void OnLayoutUpdated(object? sender, EventArgs e)
    {
        int _countRow = this.RowDefinitions?.Count ?? 0;
        int _countColumn = this.ColumnDefinitions?.Count ?? 0;
        _countRow = _countRow < 1 ? 1 : _countRow;
        _countColumn = _countColumn < 1 ? 1 : _countColumn;

        if (this._countRow != _countRow || this._countColumn != _countColumn)
        {
            this._countRow = _countRow;
            this._countColumn = _countColumn;
            OnChangeGrid();
        }
    }

    private void OnChangeGrid()
    {
        
    }

    private ControlLoadingMask CreateControlMask()
    {
        var mask = new ControlLoadingMask();
        mask.SetValue(Panel.ZIndexProperty, GetMaxZIndex() + 1);
        mask.SetValue(Grid.ColumnProperty, 0);
        mask.SetValue(Grid.RowProperty, 0);
        
        mask.SetValue(Grid.RowSpanProperty, _countRow);
        mask.SetValue(Grid.ColumnSpanProperty, _countColumn);

        return mask;
    }

    private ControlAlert CreateControlAlert(string message, string description, TypeAlert typeAlert)
    {
        var control = new ControlAlert(message, description, typeAlert, (c) =>
        {
            if (this.Children.Contains(c))
            {
                this.Children.Remove(c);
            }
        });
        control.SetValue(Grid.RowProperty, 0);
        control.SetValue(Grid.ColumnProperty, 0);
        control.SetValue(Grid.RowSpanProperty, _countRow);
        control.SetValue(Grid.ColumnSpanProperty, _countColumn);
        control.SetValue(Panel.ZIndexProperty, GetMaxZIndex() + 1);
        
        int optionalWidth = 400;
        int actualWidth = (int)this.ActualWidth;
        int controlWidth = actualWidth < optionalWidth
            ? actualWidth
            : optionalWidth;
        control.Width = controlWidth;
        // int margins = actualWidth - controlWidth;
        // if (margins > 0)
        // {
        //     control.Margin = new Thickness(margins / 2, 0, 0, 0);
        // }
        return control;
    }

    private int GetMaxZIndex()
    {
        var index = 0;
        foreach (var child in this.Children)
        {
            var element = (child as UIElement);
            if (element != null)
            {
                var elementIndex = Panel.GetZIndex(element);
                index = elementIndex > index
                    ? elementIndex
                    : index;
            }
        }

        return index;
    }
    
    private void ApplyEvents()
    {
        AsyncAction.OnAlert = (m, d, ta) =>
        {
            var control = CreateControlAlert(m, d, ta);
            this.Children.Add(control);
        };

        AsyncAction.OnStart = () =>
        {
            ApplyMask(Interlocked.Add(ref _countProcess, 1));
        };

        AsyncAction.OnDone = (ex) =>
        {
            ApplyMask(Interlocked.Add(ref _countProcess, -1));
            if (ex != null)
            {
                AsyncAction.Alert(ex.Message, ex.StackTrace, TypeAlert.Error);
            }
        };
    }
    
    private void ApplyMask(int countProcess)
    {
        if (_countProcess < 1)
        {
            IsEnabled = true;
            var _controlMask = this._controlMask;
            this._controlMask = null;
            if (_controlMask != null)
            {
                if (this.Children.Contains(_controlMask))
                {
                    this.Children.Remove(_controlMask);
                }
            }
        }
        else
        {
            IsEnabled = false;
            _controlMask = _controlMask ?? CreateControlMask();
            if (!this.Children.Contains(_controlMask))
            {
                this.Children.Add(_controlMask);                
            }
        }
    }
}