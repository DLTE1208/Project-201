using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 选桌面板
/// </summary>
public class ChooseDeskPanel : BasePanel
{
    private Button backButton;
    private Toggle chooseDeskToggle;
    private Button decisionButton;
    private Image deskContent;
    private ToggleGroup deskToggleGroup;
    private Button queueButton;
    private bool contentIsChoosingDesk;
    private List<List<string>> deskInfoList;
    private List<bool> deskUsageList;
    private string choosedDNO;
    private List<Toggle> deskToggleList;
    private Button exitButton;

    public ChooseDeskPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
        deskInfoList = CustomerFunctions.GetDesksInfo(PP.ConnectionString);
        deskUsageList = new();
        deskToggleList = new();
    }

    public override void Enter()
    {
        base.Enter();

        choosedDNO = "";
        CustomerFunctions.UpdateDeskUsageInfo(PP.ConnectionString, ref deskUsageList);
        CreateDesk();

        contentIsChoosingDesk = false;
        UpdateContent();
    }

    public override void Exit()
    {
        base.Exit();

        deskToggleList.Clear();
        deskUsageList.Clear();
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
            else if (_item.name == "Decision Button") decisionButton = _item.GetComponent<Button>();
            else if (_item.name == "Choose Desk Toggle") chooseDeskToggle = _item.GetComponent<Toggle>();
            else if (_item.name == "Desk Content") deskContent = _item.GetComponent<Image>();
            else if (_item.name == "Desk Toggle Group") deskToggleGroup = _item.GetComponent<ToggleGroup>();
            else if (_item.name == "Queue Button") queueButton = _item.GetComponent<Button>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        backButton.onClick.AddListener(Back);
        decisionButton.onClick.AddListener(DecideDesk);
        chooseDeskToggle.onValueChanged.AddListener(UpdateContent);
        queueButton.onClick.AddListener(() => { PP.MyPanelMachine.PushPanel(PP.QueuePanel); });
        exitButton.onClick.AddListener(Restart);
    }

    /// <summary>
    /// 更新面板内的内容，“选择餐桌”会使所有没有人使用的餐桌可选，而“已有餐桌”则相反，调用这个函数将切换这两个内容（如果允许更新）
    /// </summary>
    /// <param name="_isChoosingDesk">是否是“选择餐桌”</param>
    private void UpdateContent(bool _isChoosingDesk = true)
    {
        if (_isChoosingDesk && !contentIsChoosingDesk)
        {
            contentIsChoosingDesk = true;
            choosedDNO = "";

            for (int _index = 0; _index < deskUsageList.Count; _index++)
            {
                if (deskUsageList[_index]) deskToggleList[_index].interactable = false;
                else deskToggleList[_index].interactable = true;
            }
        }
        else if (!_isChoosingDesk && contentIsChoosingDesk)
        {
            contentIsChoosingDesk = false;
            choosedDNO = "";

            for (int _index = 0; _index < deskUsageList.Count; _index++)
            {
                if (deskUsageList[_index]) deskToggleList[_index].interactable = true;
                else deskToggleList[_index].interactable = false;
            }
        }
    }

    /// <summary>
    /// 创建餐桌，根据二维餐桌表，在对应位置生成规格不同的餐桌，并将孩子开关组件添加进开关组，赋予函数功能
    /// </summary>
    private void CreateDesk()
    {
        foreach (List<string> _item in deskInfoList)
        {
            Vector2 _position = GetDeskPositon(_item[2]);
            _position.x = _position.x * 12;
            _position.y = _position.y * -12;

            GameObject _desk;
            if (_item[1] == "4") _desk = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/4 People Desk"), deskContent.transform);
            else if (_item[1] == "2") _desk = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/2 People Desk"), deskContent.transform);
            else _desk = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/8 People Desk"), deskContent.transform);

            _desk.name = _item[0];
            RectTransform _myTrans = _desk.GetComponent<RectTransform>();
            Toggle _myToggle = _desk.GetComponentInChildren<Toggle>();
            _myTrans.anchoredPosition = _position;
            _myToggle.group = deskToggleGroup;
            _myToggle.onValueChanged.AddListener((isOn) => { if (isOn) { choosedDNO = _item[0]; } else { choosedDNO = ""; } });
            _myToggle.isOn = false;
            deskToggleList.Add(_myToggle);
        }
    }

    /// <summary>
    /// 得到餐桌坐标，将输入的坐标进行换算，返回在面板上的锚点坐标
    /// </summary>
    /// <param name="_positionStr">数据库中的字符串坐标</param>
    /// <returns>面板的锚点坐标</returns>
    private Vector2 GetDeskPositon(string _positionStr)
    {
        int _x = int.Parse(_positionStr.Substring(0, 2));
        int _y = int.Parse(_positionStr.Substring(2, 2));
        Vector2 _position = new(_x, _y);
        return _position;
    }

    /// <summary>
    /// 返回上一页
    /// </summary>
    private void Back()
    {
        PP.MyPanelMachine.PopPanel();
    }

    /// <summary>
    /// 确认餐桌
    /// </summary>
    private void DecideDesk()
    {
        if (choosedDNO != "")
        {
            if (contentIsChoosingDesk)// && CustomerFunctions.CanChooseDesk(PP.ConnectionString, choosedDNO))
            {
                PP.DNO = choosedDNO;
                PP.HaveOrder = false;
                CustomerFunctions.ChooseDesk(PP.ConnectionString, PP.CNO, choosedDNO);
                PP.ONO = CustomerFunctions.CreateOrder(PP.ConnectionString, choosedDNO);
                Debug.Log("Choose desk is done");

                PP.MyPanelMachine.ChangePanel(PP.FoodMenuPanel);
            }
            else if (!contentIsChoosingDesk)
            {
                PP.DNO = choosedDNO;
                PP.HaveOrder = true;
                CustomerFunctions.ChooseDesk(PP.ConnectionString, PP.CNO, choosedDNO);
                PP.ONO = CustomerFunctions.FindOrder(PP.ConnectionString, choosedDNO);
                Debug.Log("Choose desk is done");

                PP.MyPanelMachine.ChangePanel(PP.FoodMenuPanel);
            }
            else
            {
                PP.MessagePanel.MessageText = "您不能选择这张餐桌";
                PP.MyPanelMachine.PushPanel(PP.MessagePanel);
            }
        }
        else
        {
            PP.MessagePanel.MessageText = "您还没有选择餐桌";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel); 
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
