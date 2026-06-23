using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace SuchByte.MacroDeck.Folders;

/// <summary>
/// Macro Deck 文件夹类，表示按钮布局中的一个文件夹页面。
/// 每个文件夹包含一组动作按钮，支持子文件夹和应用程序焦点触发。
/// 分配了非托管内存缓冲区用于高效数据传输。
/// </summary>
public class MacroDeckFolder : IDisposable
{
    /// <summary>
    /// 非托管内存缓冲区指针，用于高效数据传输
    /// </summary>
    private IntPtr _bufferPtr;

    /// <summary>
    /// 缓冲区大小，默认 1MB
    /// </summary>
    public int BUFFER_SIZE = 1024 * 1024;

    private bool _disposed;

    /// <summary>
    /// 构造函数，分配非托管内存缓冲区
    /// </summary>
    public MacroDeckFolder()
    {
        _bufferPtr = Marshal.AllocHGlobal(BUFFER_SIZE);
    }

    /// <summary>
    /// 释放资源的核心方法，释放所有动作按钮和非托管内存
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
        }

        // 释放所有动作按钮
        foreach (var actionButton in ActionButtons)
        {
            actionButton.Dispose();
        }

        // 释放非托管内存缓冲区
        Marshal.FreeHGlobal(_bufferPtr);
        _disposed = true;
    }

    /// <summary>
    /// 释放所有资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 析构函数，确保非托管内存被释放
    /// </summary>
    ~MacroDeckFolder()
    {
        Dispose(false);
    }

    /// <summary>
    /// 是否为根文件夹（显示名为 "*Root*"）
    /// </summary>
    [JsonIgnore] public bool IsRootFolder => DisplayName.Equals("*Root*");

    /// <summary>文件夹唯一标识</summary>
    public string FolderId { get; set; }

    /// <summary>文件夹显示名称</summary>
    public string DisplayName { get; set; }

    /// <summary>子文件夹 ID 列表</summary>
    public List<string> Childs { get; set; } = new();

    /// <summary>文件夹中包含的动作按钮列表</summary>
    public List<ActionButton.ActionButton> ActionButtons { get; set; }

    /// <summary>应用程序焦点设备列表，当指定应用获得焦点时自动切换到此文件夹</summary>
    public List<string> ApplicationsFocusDevices { get; set; } = new();

    /// <summary>触发文件夹切换的目标应用程序名称</summary>
    public string ApplicationToTrigger { get; set; } = "";
}
