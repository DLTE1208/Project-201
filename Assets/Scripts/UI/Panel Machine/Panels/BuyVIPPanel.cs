using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 购买VIP面板
/// </summary>
public class BuyVIPPanel : BasePanel
{
    private Button backButton;
    private Toggle goldToggle;
    private Toggle diamondToggle;
    private Toggle platinumToggle;
    private Button aPackageButton;
    private Button bPackageButton;
    private Button cPackageButton;
    private Button dPackageButton;
    private Button ePackageButton;
    private TMP_Text priceText;
    private Button buyButton;
    private Button exitButton;
    private int package;
    private string newVIPRank;
    private float pricePerDay;
    private int duration;
    private float discount;

    public BuyVIPPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
    }

    public override void Enter()
    {
        base.Enter();

        priceText.text = "";
        newVIPRank = "黄金";
        package = 0;
        duration = 0;
        discount = 0.95f;
        pricePerDay = 1f;
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
            else if (_item.name == "A Package Button") aPackageButton = _item.GetComponent<Button>();
            else if (_item.name == "B Package Button") bPackageButton = _item.GetComponent<Button>();
            else if (_item.name == "C Package Button") cPackageButton = _item.GetComponent<Button>();
            else if (_item.name == "D Package Button") dPackageButton = _item.GetComponent<Button>();
            else if (_item.name == "E Package Button") ePackageButton = _item.GetComponent<Button>();
            else if (_item.name == "Buy Button") buyButton = _item.GetComponent<Button>();
            else if (_item.name == "Gold Toggle") goldToggle = _item.GetComponent<Toggle>();
            else if (_item.name == "Diamond Toggle") diamondToggle = _item.GetComponent<Toggle>();
            else if (_item.name == "Platinum Toggle") platinumToggle = _item.GetComponent<Toggle>();
            else if(_item.name == "Price Text") priceText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Exit Button") exitButton = _item.GetComponent<Button>();
        }

        backButton.onClick.AddListener(Back);
        aPackageButton.onClick.AddListener(() => { SetPackageAndDuration(1, 7); UpdateText(); ResetButtonInteractable(); aPackageButton.interactable = false; });
        bPackageButton.onClick.AddListener(() => { SetPackageAndDuration(2, 30); UpdateText(); ResetButtonInteractable(); bPackageButton.interactable = false; });
        cPackageButton.onClick.AddListener(() => { SetPackageAndDuration(3, 120); UpdateText(); ResetButtonInteractable(); cPackageButton.interactable = false; });
        dPackageButton.onClick.AddListener(() => { SetPackageAndDuration(4, 365); UpdateText(); ResetButtonInteractable(); dPackageButton.interactable = false; });
        ePackageButton.onClick.AddListener(() => { SetPackageAndDuration(5, 50000); UpdateText(); ResetButtonInteractable(); ePackageButton.interactable = false; });
        goldToggle.onValueChanged.AddListener( _isOn => { pricePerDay = 1f; discount = 0.95f; newVIPRank = "黄金"; UpdateText(); });
        diamondToggle.onValueChanged.AddListener(_isOn => { pricePerDay = 2.5f; discount = 0.9f; newVIPRank = "钻石"; UpdateText(); });
        platinumToggle.onValueChanged.AddListener(_isOn => { pricePerDay = 4f; discount = 0.8f; newVIPRank = "白金"; UpdateText(); });
        buyButton.onClick.AddListener(Buy);
        exitButton.onClick.AddListener(Restart);

    }

    /// <summary>
    /// 返回上一面板
    /// </summary>
    private void Back()
    {
        PP.MyPanelMachine.PopPanel();
    }

    /// <summary>
    /// 设置优惠套餐和时间
    /// </summary>
    /// <param name="_package"></param>
    /// <param name="_duration"></param>
    private void SetPackageAndDuration(int _package, int _duration)
    {
        package = _package;
        duration = _duration;
    }

    /// <summary>
    /// 刷新价格文本
    /// </summary>
    private void UpdateText()
    {
        float sumPrice = duration * pricePerDay;
        if (package == 5) sumPrice *= 0.2f;
        else if (package == 4) sumPrice *= 0.8f;
        else if (package == 3) sumPrice *= 0.9f;
        else if (package == 2) sumPrice *= 0.95f;
        if (sumPrice != 0) priceText.text = $"￥{sumPrice}立即开通，享受{discount}的价格折扣";
    }

    /// <summary>
    /// 重设按钮的可选性
    /// </summary>
    private void ResetButtonInteractable()
    {
        aPackageButton.interactable = true;
        bPackageButton.interactable = true;
        cPackageButton.interactable = true;
        dPackageButton.interactable = true;
        ePackageButton.interactable = true;
    }

    /// <summary>
    /// 购买
    /// </summary>
    private void Buy()
    {
        if (package != 0)
        {
            if (CustomerFunctions.CanUpgradeVIP(PP.ConnectionString, PP.CNO, newVIPRank, ref duration))
            {
                CustomerFunctions.UpgradeVIPCustomer(PP.ConnectionString, PP.CNO, newVIPRank, duration);

                PP.WelcomePanel.TitleText = $"{newVIPRank}会员";
                PP.WelcomePanel.InfoText = $"您的{newVIPRank}会员将持续\n{duration}天，\n在此期间，每次在本店消费，\n都将享受{discount}的价格优惠！";
                PP.WelcomePanel.IsPopPanel = true;
                PP.VIPRank = newVIPRank;
                PP.MyPanelMachine.ChangePanel(PP.WelcomePanel);
            }
            else
            {
                PP.MessagePanel.MessageText = $"您是尊贵的{PP.VIPRank}会员，\n您不应该选择更低的VIP等级";
                PP.MyPanelMachine.PushPanel(PP.MessagePanel);
            }
        }
        else
        {
            PP.MessagePanel.MessageText = "您还没有选择时间";
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
