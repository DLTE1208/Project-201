using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 主菜单面板
/// </summary>
public class MainMenuPanel : BasePanel
{ 
    public bool isPayed;

    private Button personalInfoButton;
    private Button buyVIPButton;
    private Button chooseDeskButton;
    private Button orderFoodButton;
    private Button payButton;
    private Button exitButton;

    public MainMenuPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
    }

    public override void Enter()
    {
        base.Enter();

        orderFoodButton.interactable = false;
        payButton.interactable = false;
        isPayed = false;
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

        if (isPayed)
        {
            personalInfoButton.interactable = false;
            buyVIPButton.interactable = false;
            chooseDeskButton.interactable = false;
            orderFoodButton.interactable = false;
            payButton.interactable = false;

        }
        else if (PP.DNO == "")
        {
            chooseDeskButton.interactable = true;
            orderFoodButton.interactable = false;
            payButton.interactable = false;
        }
        else
        {
            chooseDeskButton.interactable = false;
            orderFoodButton.interactable = true;
            payButton.interactable = true;
        }
    }

    protected override void Instantiate()
    {
        base.Instantiate();

        Transform[] _trans = Panel.GetComponentsInChildren<Transform>();
        foreach (Transform _item in _trans)
        {
            if (_item.name == "Personal Info Button") personalInfoButton = _item.GetComponent<Button>();
            else if (_item.name == "Buy VIP Button") buyVIPButton = _item.GetComponent<Button>();
            else if (_item.name == "Choose Desk Button") chooseDeskButton = _item.GetComponent<Button>();
            else if (_item.name == "Order Food Button") orderFoodButton = _item.GetComponent<Button>();
            else if (_item.name == "Pay Button") payButton = _item.GetComponent<Button>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();

        }

        personalInfoButton.onClick.AddListener(ShowPersonalInfo);
        buyVIPButton.onClick.AddListener(GoToBuyVIP);
        chooseDeskButton.onClick.AddListener(GoToChooseDesk);
        orderFoodButton.onClick.AddListener(GoToOrderFood);
        payButton.onClick.AddListener(Pay);
        exitButton.onClick.AddListener(Restart);
    }

    private void ShowPersonalInfo()
    {
        PP.InfoPanel.IsPopPanel = true;
        PP.InfoPanel.TitleText = "个人信息";
        PP.InfoPanel.InfoText =    $"账户名：{PP.CNO}\n" +
                                   $"VIP等级：{PP.VIPRank}\n" +
                                   $"就座餐桌：{PP.DNO}\n" +
                                   $"当前订单：{PP.ONO}\n" +
                                   $"姓名：{PP.Name}\n" +
                                   $"性别：{PP.Sex}\n" +
                                   $"联系方式：{PP.ContactWay}\n";
        PP.MyPanelMachine.PushPanel(PP.InfoPanel);
    }

    private void GoToBuyVIP()
    {
        PP.MyPanelMachine.PushPanel(PP.BuyVIPPanel);
    }

    private void GoToChooseDesk()
    {
        if (PP.DNO == "") PP.MyPanelMachine.PushPanel(PP.ChooseDeskPanel);
        else
        {
            PP.MessagePanel.MessageText = "您已经选择了餐桌，\n无需再次选择";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
        }
    }

    private void GoToOrderFood()
    {
        if (PP.DNO != "")PP.MyPanelMachine.PushPanel(PP.FoodMenuPanel);
        else
        {
            PP.MessagePanel.MessageText = "您还没有选择餐桌，\n快去选择吧";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);
        }
    }

    private void Pay()
    {
        PP.MainMenuPanel.isPayed = true;
        string _discount = CustomerFunctions.CalculateDiscount(PP.ConnectionString, PP.DNO);
        float _amountOfMoney = CustomerFunctions.FinishOrderByCustomer(PP.ConnectionString, PP.ONO);
        CustomerFunctions.LeaveDesk(PP.ConnectionString, PP.DNO);

        ShowBill(_amountOfMoney, _discount);
        PP.BillPanel.IsPopPanel = true;
        PP.LoadingPanel.NextPanel = PP.BillPanel;
        PP.LoadingPanel.LoadingDuration = 1.5f;
        PP.MyPanelMachine.PushPanel(PP.LoadingPanel);
        Debug.Log("Program is done");
    }

    public void ShowBill(float _amountOfMoney, string _discount)
    {
        List<List<string>> billInfo = CustomerFunctions.GetBillInfo(PP.ConnectionString, PP.ONO);
        if (_discount == "1") _discount = "1.0";
        else _discount = _discount.Substring(0, _discount.LastIndexOf('.') + 2);
        string _infoText = "序\t菜品名称\t\t\t数量\t\t单价\t\t金额\n----------------------------------------------------------------------\n";
        int _count = 1;
        
        foreach (List<string> _item in billInfo)
        {
            string _tmpName;
            _tmpName = _item[0];
            int _y = 5 - (_tmpName.Length / 2);
            if (_tmpName.Length % 2 == 1) _y -= 1;
            for (int _i = 0; _i < _y; _i++) _tmpName += '\t';

            string _tmpPerPrice;
            _tmpPerPrice = _item[2];
            _tmpPerPrice = _tmpPerPrice.Substring(0, _tmpPerPrice.LastIndexOf('.') + 2);
            string _tmpSumPrice;
            _tmpSumPrice = _item[3];
            _tmpSumPrice = _tmpSumPrice.Substring(0, _tmpSumPrice.LastIndexOf('.') + 3);

            string _tmp = $"{_count.ToString()}\t{_tmpName}{_item[1]}\t\t{_tmpPerPrice.PadRight(8, ' ')}\t{_tmpSumPrice}\n";
            _infoText += _tmp;
            _count++;
        }
        _infoText += "----------------------------------------------------------------------\n";
        _infoText += $"总折扣：\t{_discount}\n应收：\t{_amountOfMoney}\n实收：\t{_amountOfMoney}\n";
        PP.BillPanel.TitleText = "您的账单";
        PP.BillPanel.InfoText = _infoText;
    }

    private void Restart()
    {
        PP.MyPanelMachine.ClearPanel();
        PP.LoadingPanel.NextPanel = PP.LoginPanel;
        PP.MyPanelMachine.PushPanel(PP.LoadingPanel);
        PP.Reset();
    }
}
