using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//һ��Ҫ�������������������ռ�
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ����VIP���
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
        newVIPRank = "�ƽ�";
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
        goldToggle.onValueChanged.AddListener( _isOn => { pricePerDay = 1f; discount = 0.95f; newVIPRank = "�ƽ�"; UpdateText(); });
        diamondToggle.onValueChanged.AddListener(_isOn => { pricePerDay = 2.5f; discount = 0.9f; newVIPRank = "��ʯ"; UpdateText(); });
        platinumToggle.onValueChanged.AddListener(_isOn => { pricePerDay = 4f; discount = 0.8f; newVIPRank = "�׽�"; UpdateText(); });
        buyButton.onClick.AddListener(Buy);
        exitButton.onClick.AddListener(Restart);

    }

    /// <summary>
    /// ������һ���
    /// </summary>
    private void Back()
    {
        PP.MyPanelMachine.PopPanel();
    }

    /// <summary>
    /// �����Ż��ײͺ�ʱ��
    /// </summary>
    /// <param name="_package"></param>
    /// <param name="_duration"></param>
    private void SetPackageAndDuration(int _package, int _duration)
    {
        package = _package;
        duration = _duration;
    }

    /// <summary>
    /// ˢ�¼۸��ı�
    /// </summary>
    private void UpdateText()
    {
        float sumPrice = duration * pricePerDay;
        if (package == 5) sumPrice *= 0.2f;
        else if (package == 4) sumPrice *= 0.8f;
        else if (package == 3) sumPrice *= 0.9f;
        else if (package == 2) sumPrice *= 0.95f;
        if (sumPrice != 0) priceText.text = $"��{sumPrice}������ͨ������{discount}�ļ۸��ۿ�";
    }

    /// <summary>
    /// ���谴ť�Ŀ�ѡ��
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
    /// ����
    /// </summary>
    private void Buy()
    {
        if (package != 0)
        {
            if (CustomerFunctions.CanUpgradeVIP(PP.ConnectionString, PP.CNO, newVIPRank, ref duration))
            {
                CustomerFunctions.UpgradeVIPCustomer(PP.ConnectionString, PP.CNO, newVIPRank, duration);

                PP.WelcomePanel.TitleText = $"{newVIPRank}��Ա";
                PP.WelcomePanel.InfoText = $"����{newVIPRank}��Ա������\n{duration}�죬\n�ڴ��ڼ䣬ÿ���ڱ������ѣ�\n��������{discount}�ļ۸��Żݣ�";
                PP.WelcomePanel.IsPopPanel = true;
                PP.VIPRank = newVIPRank;
                PP.MyPanelMachine.ChangePanel(PP.WelcomePanel);
            }
            else
            {
                PP.MessagePanel.MessageText = $"��������{PP.VIPRank}��Ա��\n����Ӧ��ѡ����͵�VIP�ȼ�";
                PP.MyPanelMachine.PushPanel(PP.MessagePanel);
            }
        }
        else
        {
            PP.MessagePanel.MessageText = "����û��ѡ��ʱ��";
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
