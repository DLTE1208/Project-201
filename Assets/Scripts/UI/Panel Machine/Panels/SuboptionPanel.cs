using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

//子选项面板
public class SuboptionPanel : BasePanel
{
    public string FNO;
    public float Price;

    private Button decisionButton;
    private Button cancelButton;
    private Button addButton;
    private Button decreaseButton;
    private TMP_Text amountText;
    private GameObject BGImage2;
    private List<List<string>> avlSuboptionList;
    private int amount;
    private string SNO;

    public SuboptionPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
    }

    public override void Enter()
    {
        amount = 1;

        base.Enter();

        BGImage2.SetActive(true);
        CreateSuboption();
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
            if (_item.name == "Cancel Button") cancelButton = _item.GetComponent<Button>();
            else if (_item.name == "Decision Button") decisionButton = _item.GetComponent<Button>();
            else if (_item.name == "Add Button") addButton = _item.GetComponent<Button>();
            else if (_item.name == "Decrease Button") decreaseButton = _item.GetComponent<Button>();
            else if (_item.name == "Amount Text") amountText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "BG Image 2") BGImage2 = _item.gameObject;
        }

        cancelButton.onClick.AddListener(CancelOrder);
        decisionButton.onClick.AddListener(DecideOrder);
        amountText.text = amount.ToString();
        addButton.onClick.AddListener(() => { if (amount < 10) amount++; amountText.text = amount.ToString(); });
        decreaseButton.onClick.AddListener(() => { if (amount > 1) amount--; amountText.text = amount.ToString(); });
    }

    /// <summary>
    /// 创建子选项开关组
    /// </summary>
    private void CreateSuboption()
    {
        GameObject _suboption = null;
        TMP_Text _choice2Text = null, _choice3Text = null, _choice4Text = null, _choice5Text = null, _kindText = null;
        Toggle _choice1Toggle = null, _choice2Toggle = null, _choice3Toggle = null, _choice4Toggle = null, _choice5Toggle = null;

        avlSuboptionList = CustomerFunctions.GetAvlSuboption(PP.ConnectionString, FNO);

        if (avlSuboptionList.Count == 2)
        {
            _suboption = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/3 Choices Suboption Region"), Panel.transform);

            Transform[] _trans = _suboption.GetComponentsInChildren<Transform>();
            foreach (Transform _item in _trans)
            {
                if (_item.name == "Kind Text") _kindText = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 2 Text") _choice2Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 3 Text") _choice3Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 1 Toggle") _choice1Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 2 Toggle") _choice2Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 3 Toggle") _choice3Toggle = _item.GetComponent<Toggle>();
            }

            _kindText.text = avlSuboptionList[0][1];
            _choice2Text.text = avlSuboptionList[0][2];
            _choice3Text.text = avlSuboptionList[1][2];
            _choice1Toggle.onValueChanged.AddListener((isON) => { SNO = "s0000"; });
            _choice2Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[0][0]; Price += float.Parse(avlSuboptionList[0][3]); });
            _choice3Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[1][0]; Price += float.Parse(avlSuboptionList[1][3]); });
            SNO = avlSuboptionList[1][0];
        }
        else if (avlSuboptionList.Count == 3)
        {
            _suboption = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/4 Choices Suboption Region"), Panel.transform);

            Transform[] _trans = _suboption.GetComponentsInChildren<Transform>();
            foreach (Transform _item in _trans)
            {
                if (_item.name == "Kind Text") _kindText = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 2 Text") _choice2Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 3 Text") _choice3Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 4 Text") _choice4Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 1 Toggle") _choice1Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 2 Toggle") _choice2Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 3 Toggle") _choice3Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 4 Toggle") _choice4Toggle = _item.GetComponent<Toggle>();
            }

            _kindText.text = avlSuboptionList[0][1];
            _choice2Text.text = avlSuboptionList[0][2];
            _choice3Text.text = avlSuboptionList[1][2];
            _choice4Text.text = avlSuboptionList[2][2];
            _choice1Toggle.onValueChanged.AddListener((isON) => { SNO = "s0000"; });
            _choice2Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[0][0]; Price += float.Parse(avlSuboptionList[0][3]); });
            _choice3Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[1][0]; Price += float.Parse(avlSuboptionList[1][3]); });
            _choice4Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[2][0]; Price += float.Parse(avlSuboptionList[2][3]); });
            SNO = avlSuboptionList[2][0];
        }
        else if (avlSuboptionList.Count == 4)
        {
            _suboption = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/UI Components/5 Choices Suboption Region"), Panel.transform);

            Transform[] _trans = _suboption.GetComponentsInChildren<Transform>();
            foreach (Transform _item in _trans)
            {
                if (_item.name == "Kind Text") _kindText = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 2 Text") _choice2Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 3 Text") _choice3Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 4 Text") _choice4Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 5 Text") _choice5Text = _item.GetComponent<TMP_Text>();
                else if (_item.name == "Choice 1 Toggle") _choice1Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 2 Toggle") _choice2Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 3 Toggle") _choice3Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 4 Toggle") _choice4Toggle = _item.GetComponent<Toggle>();
                else if (_item.name == "Choice 5 Toggle") _choice5Toggle = _item.GetComponent<Toggle>();
            }

            _kindText.text = avlSuboptionList[0][1];
            _choice2Text.text = avlSuboptionList[0][2];
            _choice3Text.text = avlSuboptionList[1][2];
            _choice4Text.text = avlSuboptionList[2][2];
            _choice5Text.text = avlSuboptionList[3][2];
            _choice1Toggle.onValueChanged.AddListener((isON) => { SNO = "s0000"; });
            _choice2Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[0][0]; Price += float.Parse(avlSuboptionList[0][3]); });
            _choice3Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[1][0]; Price += float.Parse(avlSuboptionList[1][3]); });
            _choice4Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[2][0]; Price += float.Parse(avlSuboptionList[2][3]); });
            _choice5Toggle.onValueChanged.AddListener((isON) => { SNO = avlSuboptionList[3][0]; Price += float.Parse(avlSuboptionList[3][3]); });
            SNO = avlSuboptionList[3][0];
        }
        else if (avlSuboptionList.Count == 0)
        {
            SNO = "s0000";
            BGImage2.SetActive(false);
        }
        else Debug.Log("严重错误：子选项超出范围");
    }
    

    private void CancelOrder()
    {
        PP.MyPanelMachine.PopPanel();
    }

    private void DecideOrder()
    {
        CustomerFunctions.AddFood(PP.ConnectionString, PP.ONO, FNO, SNO, amount);
        Debug.Log("Add food success");
        List<string> _tmp = CustomerFunctions.GetFoodDetailInfo(PP.ConnectionString, FNO, SNO);
        PP.FoodMenuPanel.AddMyOrder(_tmp[0], _tmp[1], _tmp[2], _tmp[3], amount.ToString(), _tmp[4]);
        PP.MyPanelMachine.PopPanel();
    }
}
