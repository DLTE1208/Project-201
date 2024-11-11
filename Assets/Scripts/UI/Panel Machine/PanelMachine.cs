using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������״̬��״̬��
/// </summary>
public class PanelMachine
{
    public Stack<BasePanel> PanelStack { get; private set; }        //���ջ
    public BasePanel CurrentPanel { get; private set; }             //��ǰ��壬��ǰ�������е���壬�����ջ��ջ�����

    public PanelMachine()
    {
        PanelStack = new Stack<BasePanel>();
    }
    
    /// <summary>
    /// ��ʼ��������ʼ�����ջ
    /// </summary>
    /// <param name="_initialPanel">��ʼ���</param>
    public void Initialize(BasePanel _initialPanel)
    {
        PanelStack.Push(_initialPanel);
        UpdateCurrentPanel();
        CurrentPanel.Enter();
    }

    /// <summary>
    /// ������壬��ջ��������滻Ϊ����壬������ջ���������Ļָ�����
    /// </summary>
    /// <param name="_newPanel">�滻�������</param>
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
    /// �����ջ
    /// </summary>
    /// <param name="_newPanel">��ջ�������</param>
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
    /// ����ջ
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
    /// ������ջ
    /// </summary>
    public void ClearPanel()
    {
        while (PanelStack.Count > 0) PopPanel();
    }

    /// <summary>
    /// ���µ�ǰ��壬��ջ����壬���ջ��Ϊ�գ��򷵻�null
    /// </summary>
    private void UpdateCurrentPanel()
    {
        if (PanelStack.Count > 0) CurrentPanel = PanelStack.Peek();
        else CurrentPanel = null;
    }
}
