using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板机，面板状态的状态机
/// </summary>
public class PanelMachine
{
    public Stack<BasePanel> PanelStack { get; private set; }        //面板栈
    public BasePanel CurrentPanel { get; private set; }             //当前面板，当前正在运行的面板，即面板栈的栈顶面板

    public PanelMachine()
    {
        PanelStack = new Stack<BasePanel>();
    }
    
    /// <summary>
    /// 初始化，将初始面板入栈
    /// </summary>
    /// <param name="_initialPanel">初始面板</param>
    public void Initialize(BasePanel _initialPanel)
    {
        PanelStack.Push(_initialPanel);
        UpdateCurrentPanel();
        CurrentPanel.Enter();
    }

    /// <summary>
    /// 交换面板，将栈顶的面板替换为新面板，不触发栈内其它面板的恢复函数
    /// </summary>
    /// <param name="_newPanel">替换的新面板</param>
    public void ChangePanel(BasePanel _newPanel)
    {
        if (CurrentPanel != null)
        {
            _newPanel.Enter();
            CurrentPanel.Exit();
            PanelStack.Pop();
            PanelStack.Push(_newPanel);
            UpdateCurrentPanel();
        }
        else PushPanel(_newPanel);
    }

    /// <summary>
    /// 面板入栈
    /// </summary>
    /// <param name="_newPanel">入栈的新面板</param>
    public void PushPanel(BasePanel _newPanel)
    {
        if (CurrentPanel != null)
        {
            CurrentPanel.Pause();
        }
        PanelStack.Push(_newPanel);
        UpdateCurrentPanel();
        CurrentPanel.Enter();
    }

    /// <summary>
    /// 面板出栈
    /// </summary>
    public void PopPanel()
    {
        if (CurrentPanel != null)
        {
            CurrentPanel.Exit();
            PanelStack.Pop();
            UpdateCurrentPanel();
            if (CurrentPanel != null) CurrentPanel.Resume();
        }
    }

    /// <summary>
    /// 清空面板栈
    /// </summary>
    public void ClearPanel()
    {
        while (PanelStack.Count > 0) PopPanel();
    }

    /// <summary>
    /// 更新当前面板，即栈顶面板，如果栈内为空，则返回null
    /// </summary>
    private void UpdateCurrentPanel()
    {
        if (PanelStack.Count > 0) CurrentPanel = PanelStack.Peek();
        else CurrentPanel = null;
    }
}
