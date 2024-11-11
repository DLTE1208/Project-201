using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//һ��Ҫ�������������������ռ�
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ��Ϣ��壬�Ҹ����ֻ��ͨ��������PushPanel����
/// </summary>
public class MessagePanel : BasePanel
{
    public string MessageText;

    private TMP_Text messageText;
    private Button continueButton;

    public MessagePanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
        MessageText = "û�и���Ϣ��ֵ";
    }

    public override void Enter()
    {
        base.Enter();

        messageText.text = MessageText;
    }

    public override void Exit()
    {
        base.Exit();

        MessageText = "û�и���Ϣ��ֵ";
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
    /// ������ť�����������һҳ������
    /// </summary>
    private void Continue()
    {
        PP.MyPanelMachine.PopPanel();
    }
}
