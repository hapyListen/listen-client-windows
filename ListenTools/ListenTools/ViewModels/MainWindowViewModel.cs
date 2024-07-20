namespace ListenTools.ViewModels;

public partial class MainWindowViewModel : Base.ViewModelBase
{
    
    public string Greeting => "Welcome to Avalonia!";


    public MainWindowViewModel()
    {
        Media.Instance.TcpClientTest();
    }
}