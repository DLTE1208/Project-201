using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 订单管理面板
/// </summary>
public class OrderManagePanel : BasePanel
{
    private Button backButton;
    private TMP_InputField ONOInputField;
    private TMP_InputField priceInputField;
    private TMP_InputField noteInputField;
    private Transform content;
    private Button actionButton;
    private TMP_Text actionButtonText;
    private bool isAvailable;
    private Button exitButton;

    public OrderManagePanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
    }

    public override void Enter()
    {
        base.Enter();

        CreateTable();
    }

    public override void Exit()
    {
        base.Exit();

        isAvailable = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CheckInputField();
        UpdateButtonState();
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
            if (_item.name == "Back Button") backButton = _item.GetComponent<Button>();
            else if (_item.name == "ONO InputField") ONOInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Price InputField") priceInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Note InputField") noteInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Content") content = _item;
            else if (_item.name == "Action Button") actionButton = _item.GetComponent<Button>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }
        actionButtonText = actionButton.GetComponentInChildren<TMP_Text>();

        backButton.onClick.AddListener(() => { PP.MyPanelMachine.PopPanel(); });
        actionButton.onClick.AddListener(Action);
        exitButton.onClick.AddListener(Restart);
    }

    private void CreateTable()
    {
        List<List<string>> _orderInfo = ShopkeeperFunctions.GetOrderInfo(PP.ConnectionString);
        string _tmpStr;

        AddRow("订单编号\t\t下单日期\t\t\t应收\t\t完成日期\t\t\t实收\t\t备注\n", content);
        AddRow("-------------------------------------------------------------------------------------------------------------------------------\n", content);

        foreach (List<string> _row in _orderInfo)
        {
            string _tmpBeginDate = "YYYY-MM-DD HH:MM";
            if (_row[1] != "NULL")
            {
                _tmpBeginDate = _row[1];
                _tmpBeginDate = _tmpBeginDate.Substring(0, _tmpBeginDate.LastIndexOf(':'));
            }

            string _tmpSumPrice = "NULL";
            if (_row[2] != "NULL")
            {
                _tmpSumPrice = _row[2];
                _tmpSumPrice = _tmpSumPrice.Substring(0, _tmpSumPrice.LastIndexOf('.') + 3);
            }

            string _tmpFinishDate = "NULL\t";
            if (_row[3] != "")
            {
                _tmpFinishDate = _row[3];
                _tmpFinishDate = _tmpFinishDate.Substring(0, _tmpFinishDate.LastIndexOf(':'));
            }

            string _tmpIncome = "NULL";
            if (_row[4] != "")
            {
                _tmpIncome = _row[4];
                _tmpIncome = _tmpIncome.Substring(0, _tmpIncome.LastIndexOf('.') + 3);
            }

            string _tmpNote = "NULL";
            if (_row[5] != "")
            {
                _tmpNote = _row[5];
            }

            _tmpStr = $"{_row[0]}\t{_tmpBeginDate, -16}\t{_tmpSumPrice, -6}\t{_tmpFinishDate, -16}\t{_tmpIncome, -6}\t{_tmpNote}\n";
            AddRow(_tmpStr, content);
        }
        AddRow("-------------------------------------------------------------------------------------------------------------------------------\n", content);
    }

    private void UpdateTable()
    {
        for (int _i = 0; _i < content.transform.childCount; _i++) GameObject.Destroy((content.transform.GetChild(_i).gameObject));
        CreateTable();
    }

    private void AddRow(string _text, Transform _targetContent)
    {
        TMP_Text _row = GameObject.Instantiate(Resources.Load<TMP_Text>("Prefabs/UI/UI Components/Row Box For Order"), _targetContent);
        _row.text = _text;
    }

    private void Action()
    {
        string _tmpStr = ONOInputField.text;
        if (_tmpStr != "" && _tmpStr.Length == 9 && _tmpStr[0] == 'o')
        {
            CustomerFunctions.LeaveDesk(PP.ConnectionString, ShopkeeperFunctions.GetDNOByONO(PP.ConnectionString, ONOInputField.text));
            if (priceInputField.text == "") ShopkeeperFunctions.FinishOrderByShopkeeper(PP.ConnectionString, ONOInputField.text, noteInputField.text);
            else ShopkeeperFunctions.FinishOrderByShopkeeper(PP.ConnectionString, ONOInputField.text, noteInputField.text, float.Parse(priceInputField.text));
            PP.MessagePanel.MessageText = "该订单已完成";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
            UpdateTable();
        }
        else
        {
            PP.MessagePanel.MessageText = "您未输入任何有效信息";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
        }
    }

    private void CheckInputField()
    {
        if (ONOInputField.text != "" && ONOInputField.text.Length == 9 && ONOInputField.text[0] == 'o') isAvailable = true;
        else isAvailable = false;
    }

    private void UpdateButtonState()
    {
        if (isAvailable)
        {
            actionButtonText.text = "结束该订单";
            actionButton.interactable = true;
        }
        else
        {
            actionButtonText.text = "无法使用";
            actionButton.interactable = false;
        }
    }

    private void Restart()
    {
        PP.MyPanelMachine.ClearPanel();
        PP.LoadingPanel.NextPanel = PP.LoginPanel;
        PP.MyPanelMachine.PushPanel(PP.LoadingPanel);
        PP.Reset();
    }
}
