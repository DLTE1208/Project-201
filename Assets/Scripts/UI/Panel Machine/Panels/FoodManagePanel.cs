using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 菜品管理面板
/// </summary>
public class FoodManagePanel : BasePanel
{
    private Button backButton;
    private Button actionButton;
    private TMP_InputField FNOInputField;
    private TMP_InputField FNameInputField;
    private TMP_InputField kindInputField;
    private TMP_InputField priceInputField;
    private TMP_InputField imgInputField;
    private Transform content;
    private TMP_Text actionButtonText; 
    private Button exitButton;
    private bool isInsert;
    private bool isDelete;
    private bool isUpdate;

    public FoodManagePanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
    }

    public override void Enter()
    {
        base.Enter();

        CreateTable();

        isInsert = false;
        isDelete = false;
        isUpdate = false;
    }

    public override void Exit()
    {
        base.Exit();
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
            else if (_item.name == "FNO InputField") FNOInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "FName InputField") FNameInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Kind InputField") kindInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Price InputField") priceInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Img InputField") imgInputField = _item.GetComponent<TMP_InputField>();
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
        List<List<string>> _foodInfo = CustomerFunctions.GetFoodInfo(PP.ConnectionString);
        string _tmpStr;

        AddRow("菜品编号\t\t菜名\t\t\t\t\t种类\t\t价格\t\t图片", content);
        AddRow("-------------------------------------------------------------------------------------------------------------------\n", content);

        foreach (List<string> _row in _foodInfo)
        {
            string _tmpName;
            _tmpName = _row[2];
            int _y = 5 - (_tmpName.Length / 2);
            if (_tmpName.Length % 2 == 1) _y -= 1;
            for (int _i = 0; _i < _y; _i++) _tmpName += '\t';

            string _tmpPrice;
            _tmpPrice = _row[3];
            _tmpPrice = _tmpPrice.Substring(0, _tmpPrice.LastIndexOf('.') + 2);
            _tmpStr = $"{_row[0]}\t\t{_tmpName}\t{_row[1]}\t\t{_tmpPrice, -5}\t\t{_row[4]}\n";

            AddRow(_tmpStr, content);
        }
        AddRow("-------------------------------------------------------------------------------------------------------------------\n", content);
    }

    private void AddRow(string _text, Transform _targetContent)
    {
        TMP_Text _row = GameObject.Instantiate(Resources.Load<TMP_Text>("Prefabs/UI/UI Components/Row Box For Food"), _targetContent);
        _row.text = _text;
    }

    private void UpdateTable()
    {
        for (int _i = 0; _i < content.transform.childCount; _i++) GameObject.Destroy((content.transform.GetChild(_i).gameObject));
        CreateTable();
    }

    private void Action()
    {
        if (isInsert)
        {
            ShopkeeperFunctions.InsetIntoFood(PP.ConnectionString, FNameInputField.text, kindInputField.text, priceInputField.text, imgInputField.text);
            UpdateTable();
        }
        else if (isDelete)
        {
            ShopkeeperFunctions.DeleteFromFood(PP.ConnectionString, FNOInputField.text);
            UpdateTable();
        }
        else if (isUpdate)
        {
            ShopkeeperFunctions.UpdateFood(PP.ConnectionString, FNOInputField.text, FNameInputField.text, kindInputField.text, priceInputField.text, imgInputField.text);
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
        ChangeFlag(0);
        if (FNOInputField.text != "" && FNOInputField.text.Length == 5 && FNOInputField.text[0] == 'f')
        {
            if (FNameInputField.text == "" && kindInputField.text == "" && priceInputField.text == "" && imgInputField.text == "") ChangeFlag(2);     //如果有fno，并且所有输入框为空，为删除
            else if (!(FNameInputField.text == "" && kindInputField.text == "" && priceInputField.text == "" && imgInputField.text == "")) ChangeFlag(3);   //如果有fno，并且所有输入框为空不成立，即任意一个或以上输入框有值，为修改
        }
        else
        {
            if (FNameInputField.text != "" && kindInputField.text != "" && priceInputField.text != "" && imgInputField.text != "") ChangeFlag(1);  //如果fno为空或无效，并且其它所有一个输入框非空，为插入
        }
    }

    private void ChangeFlag(int _flagNO)
    {
        if (_flagNO == 1)
        {
            isInsert = true;
            isDelete = false;
            isUpdate = false;
        }
        else if (_flagNO == 2)
        {
            isInsert = false;
            isDelete = true;
            isUpdate = false;
        }
        else if (_flagNO == 3)
        {
            isInsert = false;
            isDelete = false;
            isUpdate = true;
        }
        else
        {
            isInsert = false;
            isDelete = false;
            isUpdate = false;
        }
    }

    private void UpdateButtonState()
    {
        if (isInsert)
        {
            actionButtonText.text = "添加菜品";
            actionButton.interactable = true;
        }
        else if (isDelete)
        {
            actionButtonText.text = "删除菜品";
            actionButton.interactable = true;
        }
        else if (isUpdate)
        {
            actionButtonText.text = "修改菜品";
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