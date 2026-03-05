using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AsyncUI.Controls;

/// <summary>
/// Логика взаимодействия для ControlAlert.xaml
/// </summary>
public partial class ControlAlert : UserControl
{
    public string Message { set; get; }
    public string Description { set; get; }
    public TypeAlert TypeAlert { set; get; }

    public Brush BorderBorderBrush { set; get; }
    public Brush BorderBackground { set; get; }
    public Action<UserControl> OnClose { set; get; }

    public string TextIcon { set; get; }

    public ControlAlert(string message, string description, TypeAlert typeAlert, Action<UserControl> onClose)
    {
        InitializeComponent();
        Message = message;
        Description = description;
        TypeAlert = typeAlert;
        OnClose = onClose;
        ApplyColors();
        this.DataContext = this;
    }

    public void ApplyColors()
    {
        var converter = new BrushConverter();
        if (TypeAlert == TypeAlert.Warning)
        {
            BorderBorderBrush = (Brush)converter.ConvertFromString("Orange");
            BorderBackground = (Brush)converter.ConvertFromString("#FFFDF0");
            TextIcon = "⚠";
        }
        else if (TypeAlert == TypeAlert.Error)
        {
            BorderBorderBrush = (Brush)converter.ConvertFromString("#FF0000");
            BorderBackground = (Brush)converter.ConvertFromString("#FFF0F0");
            TextIcon = "❌";
        }
        else if (TypeAlert == TypeAlert.Success)
        {
            BorderBorderBrush = (Brush)converter.ConvertFromString("#28A745");
            BorderBackground = (Brush)converter.ConvertFromString("#F0FFF4");
            TextIcon = "✔";
        }
        else
        {
            BorderBorderBrush = (Brush)converter.ConvertFromString("Blue");
            BorderBackground = (Brush)converter.ConvertFromString("#E8F9FD");
            TextIcon = "ℹ";
        }
    }

    private void OnClickClose(object sender, RoutedEventArgs e)
    {
        OnClose?.Invoke(this);
    }
}

public enum TypeAlert
{
    Error,
    Warning,
    Success,
    Info
}