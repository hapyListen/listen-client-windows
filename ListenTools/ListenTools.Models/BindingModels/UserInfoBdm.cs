using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ListenTools.Models.BindingModels;

public class UserInfoBdm : ObservableObject
{
    public uint UserId { get; set; }

    private string? _userName;
    public string? UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    private string? _headImage;

    public string? HeadImage
    {
        get => _headImage;
        set => SetProperty(ref _headImage, value);
    }

    [DefaultValue("#F1F2F3")] private string? _userNameColor;

    public string? UserNameColor
    {
        get => _userNameColor;
        set => SetProperty(ref _userNameColor, value);
    }
    
    private string? _summary;
    /// <summary>
    /// 简介
    /// </summary>
    public string? Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }
    
    
    private bool _isOnline;

    public bool IsOnline
    {
        get => _isOnline;
        set => SetProperty(ref _isOnline, value);
    }
}