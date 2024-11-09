using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 消息面板，找个面板只能通过面板机的PushPanel调用
/// </summary>
public class MessagePanel : BasePanel
{
    public string MessageText;

    private TMP_Text messageText;
    private Button continueButton;

    public MessagePanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
        MessageText = "没有给消息赋值";
    }

    public override void Enter()
    {
        base.Enter();

        messageText.text = MessageText;
    }

    public override void Exit()
    {
        base.Exit();

        MessageText = "没有给消息赋值";
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void Pause()
    {
        base.Pause();
    }

    public override void Resume()
    {
        base.Resume();
    }

    protected override void Instantiate()
    {
        base.Instantiate();

        Transform[] _trans = Panel.GetComponentsInChildren<Transform>();
        foreach (Transform _item in _trans)
        {
            if (_item.name == "Continue Button") continueButton = _item.GetComponent<Button>();
            else if (_item.name == "Message Text") messageText = _item.GetComponent<TMP_Text>();
        }

        continueButton.onClick.AddListener(Continue);
    }

    /// <summary>
    /// 继续按钮，点击加入下一页（步）
    /// </summary>
    private void Continue()
    {
        PP.MyPanelMachine.PopPanel();
    }
}
