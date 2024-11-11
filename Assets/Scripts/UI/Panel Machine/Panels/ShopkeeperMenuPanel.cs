using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 店家菜单面板
/// </summary>
public class ShopkeeperMenuPanel : BasePanel
{
    private Button deskManageButton;
    private Button foodManageButton;
    private Button unfinishedOrderButton;
    private Button OrderManageButton;
    private Button hotStatButton;
    private Button revenueStatButton;
    private Button exitButton;

    public ShopkeeperMenuPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
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
            if (_item.name == "Desk Manage Button") deskManageButton = _item.GetComponent<Button>();
            else if (_item.name == "Food Manage Button") foodManageButton = _item.GetComponent<Button>();
            else if (_item.name == "Unfinished Order Button") unfinishedOrderButton = _item.GetComponent<Button>();
            else if (_item.name == "Order Manage Button") OrderManageButton = _item.GetComponent<Button>();
            else if (_item.name == "Hot Stat Button") hotStatButton = _item.GetComponent<Button>();
            else if (_item.name == "Revenue Stat Button") revenueStatButton = _item.GetComponent<Button>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        deskManageButton.onClick.AddListener(() => { PP.MyPanelMachine.PushPanel(PP.DeskManagePanel); });
        foodManageButton.onClick.AddListener(() => { PP.MyPanelMachine.PushPanel(PP.FoodManagePanel); });
        OrderManageButton.onClick.AddListener(() => { PP.MyPanelMachine.PushPanel(PP.OrderManagePanel); });
        unfinishedOrderButton.onClick.AddListener(ShowUnfinishedOrder);
        hotStatButton.onClick.AddListener(RecalculateAndShowHotStat);
        revenueStatButton.onClick.AddListener(() => { PP.MyPanelMachine.PushPanel(PP.RevenueStatPanel); });
        exitButton.onClick.AddListener(Restart);
    }

    private void ShowUnfinishedOrder()
    {
        List<List<string>> _unfinishedBillInfo = ShopkeeperFunctions.GetUnfinishedOrderInfo(PP.ConnectionString);
        string _infoText = "序\t菜品名称\t\t\t数量\t\t桌号\t\t下单时间\n----------------------------------------------------------------------\n";
        int _count = 1;

        foreach (List<string> _item in _unfinishedBillInfo)
        {
            string _tmpName;
            _tmpName = _item[0];
            int _y = 5 - (_tmpName.Length / 2);
            if (_tmpName.Length % 2 == 1) _y -= 1;
            for (int _i = 0; _i < _y; _i++) _tmpName += '\t';

            string _tmpTime;
            _tmpTime = _item[3];
            _tmpTime = _tmpTime.Substring(0, _tmpTime.LastIndexOf(':'));
            _tmpTime =  _tmpTime.Remove(0, 5);

            string _tmp = $"{_count.ToString()}\t{_tmpName}{_item[1]}\t\t{_item[2].PadRight(8, ' ')}\t{_tmpTime}\n";
            _infoText += _tmp;
            _count++;
        }
        _infoText += "----------------------------------------------------------------------\n";
        PP.BillPanel.TitleText = "待完成订单";
        PP.BillPanel.InfoText = _infoText;
        PP.BillPanel.IsPopPanel = true;
        PP.MyPanelMachine.PushPanel(PP.BillPanel);
    }

    private void RecalculateAndShowHotStat()
    {
        ShopkeeperFunctions.RecalculateHotRank(PP.ConnectionString);
        List<List<string>> _hotFoodInfo = ShopkeeperFunctions.GetHotFoodAndRank(PP.ConnectionString);
        string _infoText = "菜品编号\t\t菜品名称\t\t\t排名\n----------------------------------------------------------------------\n";

        foreach (List<string> _row in _hotFoodInfo)
        {
            string _tmpName;
            _tmpName = _row[1];
            int _y = 5 - (_tmpName.Length / 2);
            if (_tmpName.Length % 2 == 1) _y -= 1;
            for (int _i = 0; _i < _y; _i++) _tmpName += '\t';

            string _tmp = $"{_row[0]}\t\t{_tmpName}{_row[2]}\n";
            _infoText += _tmp;
        }
        _infoText += "----------------------------------------------------------------------\n";
        PP.BillPanel.TitleText = "热门菜品已更新";
        PP.BillPanel.InfoText = _infoText;
        PP.BillPanel.IsPopPanel = true;
        PP.MyPanelMachine.PushPanel(PP.BillPanel);
    }

    private void Restart()
    {
        PP.MyPanelMachine.ClearPanel();
        PP.LoadingPanel.NextPanel = PP.LoginPanel;
        PP.MyPanelMachine.PushPanel(PP.LoadingPanel);
        PP.Reset();
    }
}
