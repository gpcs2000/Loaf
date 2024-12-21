using Prism.Events;

namespace Loaf.Event
{
    /// <summary>
    /// 控制BackButton的显示状态的通知事件
    /// </summary>
    public class ChangeViewEvent : PubSubEvent<bool>;

    /// <summary>
    /// 系统主题发生变化的通知事件
    /// </summary>
    public class ChangeThemeEvent : PubSubEvent;

    /// <summary>
    /// 关闭窗口的通知事件
    /// </summary>
    public class CloseEvent : PubSubEvent;
}