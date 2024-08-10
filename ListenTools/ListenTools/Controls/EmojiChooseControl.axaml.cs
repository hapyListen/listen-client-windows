using System;
using System.Collections.Generic;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using ListenTools.Models;

namespace ListenTools.Controls;

public class EmojiCheckedEventArgs(RoutedEvent? routedEvent, object? source, string checkedEmoji)
    : RoutedEventArgs(routedEvent, source)
{
    public readonly string CheckedEmoji = checkedEmoji;
    public readonly string EmojiName = System.IO.Path.GetFileNameWithoutExtension(checkedEmoji);
}

public partial class EmojiChooseControl : UserControl
{
    private static readonly List<EmojiModel> AllEmojis = new();

    static EmojiChooseControl()
    {
        LoadEmoji();
    }

    public EmojiChooseControl()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<IEnumerable<Models.EmojiModel>> EmojiSourceProperty =
        AvaloniaProperty.Register<EmojiChooseControl, IEnumerable<Models.EmojiModel>>(nameof(EmojiSource),
            defaultValue: AllEmojis);

    public static readonly RoutedEvent<EmojiCheckedEventArgs> CheckEmojiEvent =
        RoutedEvent.Register<EmojiChooseControl, EmojiCheckedEventArgs>(nameof(CheckEmoji), RoutingStrategies.Bubble);

    public IEnumerable<EmojiModel> EmojiSource
    {
        get => GetValue(EmojiSourceProperty);
        private set => SetValue(EmojiSourceProperty, value);
    }

    public event EventHandler<EmojiCheckedEventArgs>? CheckEmoji
    {
        add => this.AddHandler(CheckEmojiEvent, value);
        remove => this.RemoveHandler(CheckEmojiEvent, value);
    }


    #region 加载Emoji表情

    private static void LoadEmoji()
    {
        var emojis = GetEmojiTips();

        foreach (var emoji in emojis)
        {
            AllEmojis.Add(new()
            {
                EmojiPath = $"avares://ListenTools/Resources/Images/Emoji/{emoji.Key}.png",
                EmojiName = emoji.Value
            });
        }
    }


    /// <summary>
    /// 读取所有表情包的注释内容
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, string> GetEmojiTips()
    {
        using var stream = AssetLoader.Open(new Uri("avares://ListenTools/Resources/Images/Emoji/Docs/tips_zh_cn"));
        var readData = new byte[stream.Length];
        _ = stream.Read(readData, 0, readData.Length);
        var listStr = Encoding.UTF8.GetString(readData);
        var emojiList = listStr.Split(Environment.NewLine);

        var result = new Dictionary<string, string>();
        foreach (var tips in emojiList)
        {
            var tipsArr = tips.Split("=");
            if (result.ContainsKey(tipsArr[0]))
                result[tipsArr[0]] = tipsArr[1];
            else
                result.Add(tipsArr[0], tipsArr[1]);
        }

        return result;
    }

    #endregion

    /// <summary>
    /// 选中事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
            this.OnEmojiChecked(btn.Tag.ToString());
    }

    public void OnEmojiChecked(string checkedEmoji) =>
        this.RaiseEvent(new EmojiCheckedEventArgs(CheckEmojiEvent, this, checkedEmoji));
}