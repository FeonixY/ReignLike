using System;

/// <summary>
/// 事件前置条件：用于描述当前事件触发前，必须完成的其他事件及其选项。
/// </summary>
[Serializable]
public class EventPrerequisite
{
    /// <summary>
    /// 依赖的前置事件（必须先完成该事件）。
    /// </summary>
    public Event Event;

    /// <summary>
    /// 前置事件中必须选择的选项索引（如需特定选项才算满足条件）。
    /// </summary>
    public int RequiredOption;
}