using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using ListenTools.Views.Pages;

namespace ListenTools.Views;

public partial class MainWindow : Window
{
    public static MainWindow? GlobalMainWindow { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        GlobalMainWindow = this;
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // this.ViewContent.Content = new RoomListView();
        this.ViewContent.Content = new ChatRoomPages.ChatRoomView();
    }

    #region 窗口操作

    /// <summary>
    /// 窗口拖拽
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        this.BeginMoveDrag(e);
    }

    /// <summary>
    /// 窗口关闭
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WindowCloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// 窗口最小化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WindowMinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// 窗口最大化和恢复
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WindowMaxToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton button)
            WindowState = button.IsChecked == true ? WindowState.Maximized : WindowState.Normal;
    }

    /// <summary>
    /// 双击全屏
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        WindowMaxToggleButton.IsChecked = WindowMaxToggleButton.IsChecked != true;
    }

    #endregion
}