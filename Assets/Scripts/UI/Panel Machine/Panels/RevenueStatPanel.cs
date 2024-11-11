using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 营收统计面板
/// </summary>
public class RevenueStatPanel : BasePanel
{
    private Button backButton;
    private Button UpdateButton;
    private TMP_Dropdown modeDropdown;
    private TMP_Dropdown yearBeginDropdown;
    private TMP_Dropdown monthBeginDropdown;
    private TMP_Dropdown dayBeginDropdown;
    private TMP_Dropdown yearEndDropdown;
    private TMP_Dropdown monthEndDropdown;
    private TMP_Dropdown dayEndDropdown;
    private Transform content;
    private Button exitButton;
    private int yb, mb, db, ye, me, de;

    public RevenueStatPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
    }

    public override void Enter()
    {
        base.Enter();

        yb = 0;
        mb = 0;
        db = 0;
        ye = 1;
        me = 0;
        de = 0;
        UpdateTable(0);
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
            if (_item.name == "Back Button") backButton = _item.GetComponent<Button>();
            else if (_item.name == "Mode Dropdown") modeDropdown = _item.GetComponent<TMP_Dropdown>();
            else if (_item.name == "Year Begin Dropdown") yearBeginDropdown = _item.GetComponent<TMP_Dropdown>();
            else if (_item.name == "Month Begin Dropdown") monthBeginDropdown = _item.GetComponent<TMP_Dropdown>();
            else if (_item.name == "Day Begin Dropdown") dayBeginDropdown = _item.GetComponent<TMP_Dropdown>();
            else if (_item.name == "Year End Dropdown") yearEndDropdown = _item.GetComponent<TMP_Dropdown>();
            else if (_item.name == "Month End Dropdown") monthEndDropdown = _item.GetComponent<TMP_Dropdown>();
            else if (_item.name == "Day End Dropdown") dayEndDropdown = _item.GetComponent<TMP_Dropdown>();
            else if (_item.name == "Content") content = _item;
            else if (_item.name == "Update Button") UpdateButton = _item.GetComponent<Button>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        InstantiateDropdown();
        backButton.onClick.AddListener(() => { PP.MyPanelMachine.PopPanel(); });
        modeDropdown.onValueChanged.AddListener((call) => { UpdateTable(call); });
        yearBeginDropdown.onValueChanged.AddListener((call) => { yb = call; });
        monthBeginDropdown.onValueChanged.AddListener((call) => { mb = call; });
        dayBeginDropdown.onValueChanged.AddListener((call) => { db = call; });
        yearEndDropdown.onValueChanged.AddListener((call) => { ye = call; });
        monthEndDropdown.onValueChanged.AddListener((call) => { me = call; });
        dayEndDropdown.onValueChanged.AddListener((call) => { de = call; });
        UpdateButton.onClick.AddListener(UpdateTableByButton);
        exitButton.onClick.AddListener(Restart);
    }

    private void InstantiateDropdown()
    {
        List<TMP_Dropdown.OptionData> _yearOptionList = new();
        for (int _year = 2024; _year <= 2030; _year++) _yearOptionList.Add(new TMP_Dropdown.OptionData(_year.ToString() + "年"));
        yearBeginDropdown.AddOptions(_yearOptionList);
        yearEndDropdown.AddOptions(_yearOptionList);

        List<TMP_Dropdown.OptionData> _monthOptionList = new();
        for (int _month = 1; _month <= 12; _month++) _monthOptionList.Add(new TMP_Dropdown.OptionData(_month.ToString() + "月"));
        monthBeginDropdown.AddOptions(_monthOptionList);
        monthEndDropdown.AddOptions(_monthOptionList);

        List<TMP_Dropdown.OptionData> _dayOptionList = new();
        for (int _day = 1; _day <= 31; _day++) _dayOptionList.Add(new TMP_Dropdown.OptionData(_day.ToString() + "号"));
        dayBeginDropdown.AddOptions(_dayOptionList);
        dayEndDropdown.AddOptions(_dayOptionList);
    }

    private void CreateTable(int _mode = 0)
    {
        AddRow("日期\t\t\t\t金额\t\t\t\t销量", content);
        AddRow("--------------------------------------------------------------------------", content);

        if (_mode != 3)
        {
            UpdateButton.interactable = false;
            yearBeginDropdown.interactable = false;
            monthBeginDropdown.interactable = false;
            dayBeginDropdown.interactable = false;
            yearEndDropdown.interactable = false;
            monthEndDropdown.interactable = false;
            dayEndDropdown.interactable = false;
            List<List<string>> revenueInfo = ShopkeeperFunctions.GetRevenueInfo(PP.ConnectionString, _mode);
            string _tmpStr;

            foreach (List<string> _row in revenueInfo)
            {
                string _tmpDate = _row[0];
                if (_tmpDate.LastIndexOf(' ') > 0) _tmpDate = _tmpDate.Substring(0, _tmpDate.LastIndexOf(' '));

                string _tmpSumIncome = _row[1];
                _tmpSumIncome = _tmpSumIncome.Substring(0, _tmpSumIncome.LastIndexOf('.') + 3);

                _tmpStr = $"{_tmpDate,-14}\t\t{_tmpSumIncome,-10}\t\t\t{_row[2],-10}";

                AddRow(_tmpStr, content);
            }
            AddRow("--------------------------------------------------------------------------", content);
        }
        else
        {
            for (int _i = 0; _i < content.transform.childCount; _i++) GameObject.Destroy((content.transform.GetChild(_i).gameObject));
            UpdateButton.interactable = true;
            yearBeginDropdown.interactable = true;
            monthBeginDropdown.interactable = true;
            dayBeginDropdown.interactable = true;
            yearEndDropdown.interactable = true;
            monthEndDropdown.interactable = true;
            dayEndDropdown.interactable = true;
        }
    }

    private void AddRow(string _text, Transform _targetContent)
    {
        TMP_Text _row = GameObject.Instantiate(Resources.Load<TMP_Text>("Prefabs/UI/UI Components/Row Box For Revenue"), _targetContent);
        _row.text = _text;
    }

    private void UpdateTable(int _mode)
    {
        for (int _i = 0; _i < content.transform.childCount; _i++) GameObject.Destroy((content.transform.GetChild(_i).gameObject));
        CreateTable(_mode);
    }

    private void UpdateTableByButton()
    {
        try
        {
            for (int _i = 0; _i < content.transform.childCount; _i++) GameObject.Destroy((content.transform.GetChild(_i).gameObject));

            int _yb = yb + 2024, _ye = ye + 2024;
            int _mb = mb + 1, _me = me + 1;
            int _db = db + 1, _de = de + 1;
            System.DateTime _beginTime = new(_yb, _mb, _db);
            System.DateTime _endTime = new(_ye, _me, _de);
            if (_beginTime <= _endTime)
            {
                AddRow("日期\t\t\t\t金额\t\t\t\t销量", content);
                AddRow("--------------------------------------------------------------------------", content);

                List<List<string>> _revenueInfo = ShopkeeperFunctions.GetRevenueInfo(PP.ConnectionString, _beginTime.ToShortDateString(), _endTime.ToShortDateString());
                string _tmpStr;

                if (_revenueInfo != null)
                {
                    foreach (List<string> _row in _revenueInfo)
                    {
                        string _tmpDate = _row[0];
                        if (_tmpDate.LastIndexOf(' ') > 0) _tmpDate = _tmpDate.Substring(0, _tmpDate.LastIndexOf(' '));

                        string _tmpSumIncome = _row[1];
                        _tmpSumIncome = _tmpSumIncome.Substring(0, _tmpSumIncome.LastIndexOf('.') + 3);

                        _tmpStr = $"{_tmpDate,-14}\t\t{_tmpSumIncome,-10}\t\t\t{_row[2],-10}";

                        AddRow(_tmpStr, content);
                    }
                }
                AddRow("--------------------------------------------------------------------------", content);
            }
            else
            {
                PP.MessagePanel.MessageText = "未输入合法时间范围";
                PP.MyPanelMachine.PushPanel(PP.MessagePanel);
            }
        }
        catch (System.Exception _ex)
        {
            Debug.Log("捕获错误：" + _ex.Message);
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