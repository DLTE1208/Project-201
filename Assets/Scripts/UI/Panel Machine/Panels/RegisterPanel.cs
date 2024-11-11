using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 注册面板
/// </summary>
public class RegisterPanel : BasePanel
{
    private Button backButton;
    private Button RegisterButton;
    private TMP_InputField pwdInput;
    private TMP_InputField nameInput;
    private TMP_InputField contactWayInput;
    private ToggleGroup sexToggleGroup;
    private Button exitButton;

    public RegisterPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
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
            if (_item.name == "Register Button") RegisterButton = _item.GetComponent<Button>();
            else if (_item.name == "Back Button") backButton = _item.GetComponent<Button>();
            else if (_item.name == "Pwd InputField") pwdInput = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Name InputField") nameInput = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Contact Way InputField") contactWayInput = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Sex Toggle Group") sexToggleGroup = _item.GetComponent<ToggleGroup>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        RegisterButton.onClick.AddListener(Register);
        backButton.onClick.AddListener(Back);
        exitButton.onClick.AddListener(Restart);
    }

    /// <summary>
    /// 尝试注册
    /// </summary>
    private void Register()
    {
        string _sex = sexToggleGroup.GetFirstActiveToggle().name;
        if (_sex == "Male Toggle") _sex = "男";
        else _sex = "女";

        if (pwdInput.text == "")
        {
            PP.MessagePanel.MessageText = "未填写密码";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
        }
        else if (nameInput.text == "")
        {
            PP.MessagePanel.MessageText = "未填写姓名";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
        }
        else if (contactWayInput.text == "")
        {
            PP.MessagePanel.MessageText = "未填写联系方式";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
        }
        else
        {
            string _CNO = CustomerFunctions.TryRegisterCustomerAccount(PP.ConnectionString, nameInput.text, pwdInput.text, _sex, contactWayInput.text);
            if (_CNO != "-1")
            {
                Debug.Log("Register success");

                PP.CNO = _CNO;
                CustomerFunctions.UpdatePersonalInfo(PP.ConnectionString, PP.CNO, ref PP.Name, ref PP.Password, ref PP.Sex, ref PP.ContactWay, ref PP.VIPRank);

                _CNO = _CNO.Remove(0, 1);
                PP.WelcomePanel.TitleText = "账户创建完成";
                PP.WelcomePanel.InfoText = $"尊敬的顾客，欢迎您加入RestaurantOfDLT！\n您的用户ID是{_CNO}，\n请记住此用户ID，用于以后登录。";
                PP.WelcomePanel.NextPanel = PP.LoadingPanel;
                PP.LoadingPanel.NextPanel = PP.MainMenuPanel;
                PP.MyPanelMachine.PopPanel();
                PP.MyPanelMachine.ChangePanel(PP.WelcomePanel);
            }
            else Debug.Log("Register fail");
        }
    }

    //返回上一面板
    private void Back()
    {
        PP.MyPanelMachine.PopPanel();
    }

    private void Restart()
    {
        PP.MyPanelMachine.ClearPanel();
        PP.LoadingPanel.NextPanel = PP.LoginPanel;
        PP.MyPanelMachine.PushPanel(PP.LoadingPanel);
        PP.Reset();
    }
}
