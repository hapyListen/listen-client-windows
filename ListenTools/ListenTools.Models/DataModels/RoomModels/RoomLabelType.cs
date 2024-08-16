namespace ListenTools.Models.DataModels.RoomModels;

/// <summary>
/// 房间标签类
/// </summary>
public class RoomLabelType
{
    /// <summary>
    /// 标签ID
    /// </summary>
    public uint TypeId { get; set; }
    /// <summary>
    /// 标签名称
    /// </summary>
    public string? TypeName { get; set; }
    /// <summary>
    /// 标签颜色
    /// </summary>
    public string? TypeColor { get; set; }
}