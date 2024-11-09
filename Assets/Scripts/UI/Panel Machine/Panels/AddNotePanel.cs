using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//һ��Ҫ�������������������ռ�
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ��ӱ�ע���
/// </summary>
public class AddNotePanel : BasePanel
{
    private Button continueButton;
    private TMP_InputField noteInputField;
    private Button backButton;

    public AddNotePanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
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
            else if (_item.name == "Note InputField") noteInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Back Button") backButton = _item.GetComponent<Button>();
        }

        continueButton.onClick.AddListener(Continue);
        backButton.onClick.AddListener(() => { PP.MyPanelMachine.PopPanel(); });
    }

    /// <summary>
    /// ������ť�����������һҳ������
    /// </summary>
    private void Continue()
    {
        CustomerFunctions.AddNote(PP.ConnectionString, PP.ONO, noteInputField.text);
        PP.MyPanelMachine.PopPanel();
    }
}
