using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//һ��Ҫ�������������������ռ�
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ��¼���
/// </summary>
public class LoginPanel : BasePanel
{
    private TMP_InputField uidInput;
    private TMP_InputField pwdInput;
    private Button loginButton;
    private Button registerButton;
    private Button exitButton;

    public LoginPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
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
            if (_item.name == "Uid InputField") uidInput = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Pwd InputField") pwdInput = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Login Button") loginButton = _item.GetComponent<Button>();
            else if (_item.name == "Register Button") registerButton = _item.GetComponent<Button>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        loginButton.onClick.AddListener(Login);
        registerButton.onClick.AddListener(GoToRegisterPanel);
        exitButton.onClick.AddListener(ExitApp);
    }

    /// <summary>
    /// ���Ե�¼
    /// </summary>
    private void Login()
    {
        string _CNO = uidInput.text;
        if (_CNO.Length != 9) _CNO = "c" + _CNO;
        if (_CNO == "cshopkeeper" || _CNO == "cadmin")
        {
            Debug.Log("Shopkeeper login success");
            PP.LoadingPanel.NextPanel = PP.ShopkeeperMenuPanel;
            PP.MyPanelMachine.ChangePanel(PP.LoadingPanel);
        }
        else if (CustomerFunctions.TryLoginCustomerAccount(PP.ConnectionString, _CNO, pwdInput.text))
        {
            Debug.Log("Customer login success");
            PP.CNO =_CNO;
            CustomerFunctions.UpdatePersonalInfo(PP.ConnectionString, PP.CNO, ref PP.Name, ref PP.Password, ref PP.Sex, ref PP.ContactWay, ref PP.VIPRank);
            PP.LoadingPanel.NextPanel = PP.MainMenuPanel;
            PP.MyPanelMachine.ChangePanel(PP.LoadingPanel);
        }
        else
        {
            Debug.Log("Login fail");
            PP.MessagePanel.MessageText = "��¼ʧ��";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
        }
    }

    /// <summary>
    /// ǰ��ע�����
    /// </summary>
    private void GoToRegisterPanel()
    {
        PP.MyPanelMachine.PushPanel(PP.RegisterPanel);
    }

    private void ExitApp()
    {
        Application.Quit();
    }
}
