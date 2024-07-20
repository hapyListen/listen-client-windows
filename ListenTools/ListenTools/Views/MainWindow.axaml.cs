using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace ListenTools.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.PropertyChanged += WindowPropertyChanged;
    }

    private void WindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        // 窗口状态改变时，更新最大化和恢复的按钮图片
        if (e.Property == WindowStateProperty && e.NewValue is WindowState newState)
        {
            if (newState == WindowState.Maximized)
                WindowMaxButton.IconPath = App.Current.FindResource("WindowNormalIcon") as Geometry;
            else if (newState == WindowState.Normal)
                WindowMaxButton.IconPath = App.Current.FindResource("WindowMaxIcon") as Geometry;
        }
    }

    /// <summary>
    /// 窗口拖拽
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TitleBorder_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }

    /// <summary>
    /// 双击全屏
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TitleBorder_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        SwitchWindowMaxed();
    }

    /// <summary>
    /// 窗口最小化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WindowMinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// 窗口最大化和恢复默认
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WindowMaxButton_OnClick(object? sender, RoutedEventArgs e)
    {
        SwitchWindowMaxed();
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseWindowButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    private void SwitchWindowMaxed()
    {
        if (!CanResize)
            return;
        WindowState = WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

  
}