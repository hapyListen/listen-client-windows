using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ListenTools.Models.BindingModels;

public partial class ChatRoomUserBdm : ObservableObject
{
    public uint UserId { get; set; }

    [ObservableProperty] private string? _userName;

    [ObservableProperty] private string? _headImage;

    [ObservableProperty]
    [DefaultValue("#F1F2F3")]
    private string? _userNameColor;

    [ObservableProperty] private string? _direction;

    [ObservableProperty] private bool _isOnline;
    
} 