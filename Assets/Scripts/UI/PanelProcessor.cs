using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板处理器， 必须挂在Canvas画布组件上
/// </summary>
public class PanelProcessor : MonoBehaviour
{
    public string ConnectionString = "server = DESKTOP-D65RF5L\\MSSQLSERVER2; uid = admin; pwd = 123456; database = RestaurantOfDLT";  ///数据库本地连接字符串
    //public string ConnectionString = "server = 192.168.31.164; prot = 1433; uid = admin; pwd = 123456; database = RestaurantOfDLT";  ///数据库局域网连接字符串
    public string CNO;
    public string Name;
    public string Password;
    public string Sex;
    public string ContactWay;
    public string VIPRank;
    public string DNO;
    public string ONO;
    public bool HaveOrder;  //当选择餐桌时选择“已有餐桌”并完成选择时，则HaveOrder为true，说明已有订单

    #region 面板及面板机
    //面板机
    public PanelMachine MyPanelMachine { get; private set; }

    //所有面板
    public LoginPanel LoginPanel{ get; private set; }
    public RegisterPanel RegisterPanel { get; private set; }
    public LoadingPanel LoadingPanel { get; private set; }
    public WelcomePanel WelcomePanel { get; private set; }
    public MessagePanel MessagePanel { get; private set; }
    public MainMenuPanel MainMenuPanel { get; private set; }
    public BuyVIPPanel BuyVIPPanel { get; private set; }
    public ChooseDeskPanel ChooseDeskPanel { get; private set; }
    public FoodMenuPanel FoodMenuPanel { get; private set; }
    public SuboptionPanel SuboptionPanel { get; private set; }
    public InfoPanel InfoPanel { get; private set; }
    public AddNotePanel AddNotePanel { get; private set; }
    public BillPanel BillPanel { get; private set; }
    public QueuingPanel QueuePanel { get; private set; }
    public ShopkeeperMenuPanel ShopkeeperMenuPanel { get; private set; }
    public OrderManagePanel OrderManagePanel { get; private set; }
    public DeskManagePanel DeskManagePanel { get; private set; }
    public FoodManagePanel FoodManagePanel { get; private set; }
    public RevenueStatPanel RevenueStatPanel { get; private set; }
    #endregion

    private void Awake()
    {
        MyPanelMachine = new PanelMachine();
        CNO = "";
        Password = "";
        Name = "";
        Password = "";
        Sex = "";
        ContactWay = "";
        VIPRank = "";
        DNO = "";
        ONO = "";
        HaveOrder = false;
}

    private void Start()
    {
        InitializePanels();
        MyPanelMachine.Initialize(LoginPanel);
    }

    private void Update()
    {
        MyPanelMachine.CurrentPanel.LogicUpdate();
    }

    /// <summary>
    /// 初始化所有面板，即对所有面板类建立对象
    /// </summary>
    private void InitializePanels()
    {
        LoginPanel = new("Prefabs/UI/Login Panel", this);
        RegisterPanel = new("Prefabs/UI/Register Panel", this);
        LoadingPanel = new("Prefabs/UI/Loading Panel", this);
        WelcomePanel = new("Prefabs/UI/Welcome Panel", this);
        MessagePanel = new("Prefabs/UI/Message Panel", this);
        MainMenuPanel = new("Prefabs/UI/Main Menu Panel", this);
        BuyVIPPanel = new("Prefabs/UI/Buy VIP Panel", this);
        ChooseDeskPanel = new("Prefabs/UI/Choose Desk Panel", this);
        FoodMenuPanel = new("Prefabs/UI/Food Menu Panel", this);
        SuboptionPanel = new("Prefabs/UI/Suboption Panel", this);
        InfoPanel = new("Prefabs/UI/Info Panel", this);
        AddNotePanel = new("Prefabs/UI/Add Note Panel", this);
        BillPanel = new("Prefabs/UI/Bill Panel", this);
        QueuePanel = new("Prefabs/UI/Queuing Panel", this);
        ShopkeeperMenuPanel = new("Prefabs/UI/Shopkeeper Menu Panel", this);
        OrderManagePanel = new("Prefabs/UI/Order Manage Panel", this);
        DeskManagePanel = new("Prefabs/UI/Desk Manage Panel", this);
        FoodManagePanel = new("Prefabs/UI/Food Manage Panel", this);
        RevenueStatPanel = new("Prefabs/UI/Revenue Stat Panel", this);
    }

    public void Reset()
    {
        CNO = "";
        Password = "";
        Name = "";
        Password = "";
        Sex = "";
        ContactWay = "";
        VIPRank = "";
        DNO = "";
        ONO = "";
        HaveOrder = false;
    }
}
