using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//һ��Ҫ�������������������ռ�
using UnityEngine.UI;
using TMPro;

public class BillPanel : BasePanel
{
    public string TitleText;
    public string InfoText;
    public BasePanel NextPanel;
    public bool IsPopPanel;

    private Button continueButton;
    private TMP_Text titleText;
    private TMP_Text infoText;
    private Button exitButton;

    public BillPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
        TitleText = "û�жԱ��⸳ֵ";
        InfoText = "û�ж��ı���ֵ";
        NextPanel = null;
        IsPopPanel = false;
    }

    public override void Enter()
    {
        base.Enter();

        titleText.text = TitleText;
        infoText.text = InfoText;
    }

    public override void Exit()
    {
        base.Exit();

        TitleText = "û�жԱ��⸳ֵ";
        InfoText = "û�ж��ı���ֵ";
        NextPanel = null;
        IsPopPanel = false;
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
            else if (_item.name == "Info Text") infoText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Title Text") titleText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        continueButton.onClick.AddListener(Continue);
        exitButton.onClick.AddListener(Restart);
    }

    /// <summary>
    /// ������ť�����������һҳ������
    /// </summary>
    private void Continue()
    {
        if (IsPopPanel) PP.MyPanelMachine.PopPanel();
        else PP.MyPanelMachine.ChangePanel(NextPanel);
    }

    private void Restart()
    {
        PP.MyPanelMachine.ClearPanel();
        PP.LoadingPanel.NextPanel = PP.LoginPanel;
        PP.MyPanelMachine.PushPanel(PP.LoadingPanel);
        PP.Reset();
    }
}
