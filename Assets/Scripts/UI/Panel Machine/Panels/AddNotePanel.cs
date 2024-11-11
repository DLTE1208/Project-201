using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 添加备注面板
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
    /// 继续按钮，点击加入下一页（步）
    /// </summary>
    private void Continue()
    {
        CustomerFunctions.AddNote(PP.ConnectionString, PP.ONO, noteInputField.text);
        PP.MyPanelMachine.PopPanel();
    }
}
