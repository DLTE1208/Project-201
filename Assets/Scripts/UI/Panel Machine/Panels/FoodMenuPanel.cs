using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 菜单面板
/// </summary>
public class FoodMenuPanel : BasePanel
{
    private Button backButton;
    private Toggle menuToggle;
    private GameObject menuScrollView;
    private GameObject orderScrollView;
    private Transform menuContent;
    private Transform orderContent;
    private Button addNoteButton;
    private Button showOrderButton;
    private Button PayButton;
    private bool contentIsMenu;
    private List<List<string>> foodInfoList;
    private List<List<string>> bestFoodInfoList;
    private List<List<string>> orderInfoList;
    private Button exitButton;
    private string ONO;

    public FoodMenuPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
        orderInfoList = new();
        ONO = "";
    }

    public override void Enter()
    {
        base.Enter();

        if (ONO != PP.ONO) orderInfoList.Clear();
        ONO = PP.ONO;
        foodInfoList = CustomerFunctions.GetFoodInfo(PP.ConnectionString);
        bestFoodInfoList = CustomerFunctions.GetBestFoodInfo(PP.ConnectionString);
        CreateMenu();
        CreateOrder();
        contentIsMenu = false;
        UpdateContent();

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
            else if (_item.name == "Menu Toggle") menuToggle = _item.GetComponent<Toggle>();
            else if (_item.name == "Menu Content") menuContent = _item;
            else if (_item.name == "Menu Scroll View") menuScrollView = _item.gameObject;
            else if (_item.name == "Order Content") orderContent = _item;
            else if (_item.name == "Order Scroll View") orderScrollView = _item.gameObject;
            else if (_item.name == "Add Note Button") addNoteButton = _item.GetComponent<Button>();
            else if (_item.name == "Show Order Button") showOrderButton = _item.GetComponent<Button>();
            else if (_item.name == "Pay Button") PayButton = _item.GetComponent<Button>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        backButton.onClick.AddListener(Back);
        menuToggle.onValueChanged.AddListener(UpdateContent);
        addNoteButton.onClick.AddListener(AddNote);
        showOrderButton.onClick.AddListener(ShowOrderPrice);
        PayButton.onClick.AddListener(Pay);
        exitButton.onClick.AddListener(Restart);
    }

    private void UpdateContent(bool _isSelectingMenu = true)
    {
        if (_isSelectingMenu && !contentIsMenu)
        {
            contentIsMenu = true;
            menuScrollView.SetActive(true);
            orderScrollView.SetActive(false);

        }
        else if (!_isSelectingMenu && contentIsMenu)
        {
            contentIsMenu = false;
            menuScrollView.SetActive(false);
            orderScrollView.SetActive(true);
        }
    }

    /// <summary>
    /// 创建菜单，在滑动窗口下的contecnt建立孩子FoodDisplayBox，在为FoodDisplayBox.FoodImage建立孩子Symbol
    /// </summary>
    private void CreateMenu()
    {
        TMP_Text _foodNameText = null;
        TMP_Text _foodPriceText = null;
        TMP_Text _foodKindText = null;
        Image _foodImage = null;
        Button _orderButton = null;

        foreach (List<string> _item in foodInfoList)
        {
            GameObject _food = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/Food Display Box"), menuContent);

            Transform[] _trans = _food.GetComponentsInChildren<Transform>();
            foreach (Transform _subItem in _trans)
            {
                if (_subItem.name == "Food Name Text") _foodNameText = _subItem.GetComponent<TMP_Text>();
                else if (_subItem.name == "Food Price Text") _foodPriceText = _subItem.GetComponent<TMP_Text>();
                else if (_subItem.name == "Food Kind Text") _foodKindText = _subItem.GetComponent<TMP_Text>();
                else if (_subItem.name == "Food Image") _foodImage = _subItem.GetComponent<Image>();
                else if (_subItem.name == "Order Button") _orderButton = _subItem.GetComponent<Button>();
            }

            _food.name = _item[0];
            if (_foodKindText != null) _foodKindText.text = _item[1];
            if (_foodNameText != null) _foodNameText.text = _item[2];
            if (_foodPriceText != null)
            {
                string _tmpStr;
                _tmpStr = _item[3];
                _tmpStr = "￥" + _tmpStr.Substring(0, _tmpStr.LastIndexOf('.') + 2) ;
                _foodPriceText.text = _tmpStr;
            }
            if (_foodImage != null)
            {
                string _tmpStr = "Images/" + _item[4];
                _foodImage.sprite = Resources.Load<Sprite>(_tmpStr);
            }
            if (_orderButton != null) _orderButton.onClick.AddListener(() => { CallSuboption(_item[0], float.Parse(_item[3])); });

            foreach (List<string> _row in bestFoodInfoList)
            {
                GameObject _symbol = null;
                if (_item[0] == _row[0])    //如果foodInfo.fno = bestFoodInfo.fno
                {
                    if (bool.Parse(_row[1]))
                    {
                        TMP_Text _hotRankingText = null;
                        _symbol = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/Hot Symbol"), _foodImage.transform);

                        Transform[] _components = _symbol.GetComponentsInChildren<Transform>();
                        foreach (Transform _component in _components)
                        {
                            if (_component.name == "Hot Ranking Text") _hotRankingText = _component.GetComponent<TMP_Text>();
                        }
                        if (_hotRankingText != null) _hotRankingText.text = $"热门榜第{_row[2]}名";
                    }
                    if (bool.Parse(_row[3])) _symbol = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/Recommend Symbol"), _foodImage.transform);
                    if (bool.Parse(_row[4])) _symbol = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/New Symbol"), _foodImage.transform);
                }
            }
        }
    }

    /// <summary>
    /// 创建我的订单
    /// </summary>
    private void CreateOrder()
    {
        foreach(List<string> _row in orderInfoList)
        {
            AddMyOrder(_row[0], _row[1], _row[2], _row[3], _row[4], _row[5], false);
        }
    }

    /// <summary>
    /// 在“我的菜单”中添加预制体
    /// </summary>
    /// <param name="_fName"></param>
    /// <param name="_price"></param>
    /// <param name="_kind"></param>
    /// <param name="_fImage"></param>
    /// <param name="_fAmount"></param>
    /// <param name="_sName"></param>
    public void AddMyOrder(string _fName, string _price, string _kind, string _fImage, string _fAmount, string _sName, bool _isFirstAdd = true)
    {
        if (_isFirstAdd)
        {
            List<string> _tmp = new();
            _tmp.Add(_fName);
            _tmp.Add(_price);
            _tmp.Add(_kind);
            _tmp.Add(_fImage);
            _tmp.Add(_fAmount);
            _tmp.Add(_sName);
            orderInfoList.Add(_tmp);
        }

        TMP_Text _foodNameText = null;
        TMP_Text _foodPriceText = null;
        TMP_Text _foodKindText = null;
        Image _foodImage = null;
        TMP_Text _foodAmountText = null;

        GameObject _myOrder = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/Order Display Box"), orderContent);

        Transform[] _trans = _myOrder.GetComponentsInChildren<Transform>();
        foreach (Transform _item in _trans)
        {
            if (_item.name == "Food Name Text") _foodNameText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Food Price Text") _foodPriceText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Food Kind Text") _foodKindText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Food Image") _foodImage = _item.GetComponent<Image>();
            else if (_item.name == "Food Amount Text") _foodAmountText = _item.GetComponent<TMP_Text>();
        }

        _myOrder.name = _fName;
        if (_foodKindText != null) _foodKindText.text = _kind;
        if (_foodNameText != null) _foodNameText.text = _fName + "-" + _sName;
        if (_foodPriceText != null)
        {
            string _tmpStr;
            _tmpStr = _price;
            _tmpStr = "￥" + _tmpStr.Substring(0, _tmpStr.LastIndexOf('.') + 2);
            _foodPriceText.text = _tmpStr;
        }
        if (_foodImage != null)
        {
            string _tmpStr = "Images/" + _fImage;
            _foodImage.sprite = Resources.Load<Sprite>(_tmpStr);
        }
        if (_foodAmountText != null) _foodAmountText.text = _fAmount + "份";
    }

    /// <summary>
    /// 呼叫菜品子选项面板
    /// </summary>
    /// <param name="_FNO"></param>
    private void CallSuboption(string _FNO, float _price)
    {
        PP.SuboptionPanel.FNO = _FNO;
        PP.SuboptionPanel.Price = _price;
        PP.MyPanelMachine.PushPanel(PP.SuboptionPanel);
    }

    /// <summary>
    /// 返回上一页
    /// </summary>
    private void Back()
    {
        PP.MyPanelMachine.PopPanel();
    }

    /// <summary>
    /// 订单添加备注
    /// </summary>
    private void AddNote()
    {
        PP.MyPanelMachine.PushPanel(PP.AddNotePanel);
    }

    /// <summary>
    /// 展示当期订单价格
    /// </summary>
    private void ShowOrderPrice()
    {
        string _discount = CustomerFunctions.CalculateDiscount(PP.ConnectionString, PP.DNO);
        List<List<string>> _billInfo = CustomerFunctions.GetBillInfo(PP.ConnectionString, PP.ONO);
        if (_discount == "1") _discount = "1.0";
        else _discount = _discount.Substring(0, _discount.LastIndexOf('.') + 2);
        string _infoText = "序\t菜品名称\t\t\t数量\t\t单价\t\t金额\n----------------------------------------------------------------------\n";
        int _count = 1;

        if (_billInfo != null)
        {
            foreach (List<string> _item in _billInfo)
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
            _infoText += $"总折扣：\t{_discount}\n";
            PP.BillPanel.TitleText = "您的账单";
            PP.BillPanel.InfoText = _infoText;
            PP.BillPanel.IsPopPanel = true;
            PP.MyPanelMachine.PushPanel(PP.BillPanel);
        }
    }

    /// <summary>
    /// 结账
    /// </summary>
    private void Pay()
    {
        PP.MainMenuPanel.isPayed = true;
        string _discount = CustomerFunctions.CalculateDiscount(PP.ConnectionString, PP.DNO);
        float _amountOfMoney = CustomerFunctions.FinishOrderByCustomer(PP.ConnectionString, PP.ONO);
        CustomerFunctions.LeaveDesk(PP.ConnectionString, PP.DNO);

        PP.MainMenuPanel.ShowBill(_amountOfMoney, _discount);
        PP.BillPanel.IsPopPanel = true;
        PP.LoadingPanel.NextPanel = PP.BillPanel;
        PP.LoadingPanel.LoadingDuration = 1.5f;
        PP.MyPanelMachine.ChangePanel(PP.LoadingPanel);
        Debug.Log("Program is done");
    }

    private void Restart()
    {
        PP.MyPanelMachine.ClearPanel();
        PP.LoadingPanel.NextPanel = PP.LoginPanel;
        PP.MyPanelMachine.PushPanel(PP.LoadingPanel);
        PP.Reset();
    }
}