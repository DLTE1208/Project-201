using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//һ��Ҫ�������������������ռ�
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �����������
/// </summary>
public class DeskManagePanel : BasePanel
{
    private Button backButton;
    private Button actionButton;
    private TMP_InputField DNOInputField;
    private TMP_InputField capacityInputField;
    private TMP_InputField positionXInputField;
    private TMP_InputField positionYInputField;
    private Transform content;
    private TMP_Text actionButtonText;
    private bool isInsert;
    private bool isDelete;
    private bool isUpdate;
    private Button exitButton;

    public DeskManagePanel(string _path, PanelProcessor _PP) : base(_path, _PP)
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
            else if (_item.name == "DNO InputField") DNOInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "Capacity InputField") capacityInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "PositionX InputField") positionXInputField = _item.GetComponent<TMP_InputField>();
            else if (_item.name == "PositionY InputField") positionYInputField = _item.GetComponent<TMP_InputField>();
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
        List<List<string>> _deskInfo = CustomerFunctions.GetDesksInfo(PP.ConnectionString);
        string _tmpStr;

        AddRow("������\t\t����\t\t\tλ��X\t\tλ��Y", content);
        AddRow("------------------------------------------------------------------------------\n", content);

        foreach (List<string> _row in _deskInfo)
        {
            string _tmpX = _row[2].Substring(0, 2);
            string _tmpY = _row[2].Substring(2, 2);
            _tmpStr = $"{_row[0]}\t\t{_row[1]}\t\t\t{_tmpX}\t\t\t{_tmpY}\n";
            AddRow(_tmpStr, content);
        }
        AddRow("------------------------------------------------------------------------------\n", content);
    }

    private void AddRow(string _text, Transform _targetContent)
    {
        TMP_Text _row = GameObject.Instantiate(Resources.Load<TMP_Text>("Prefabs/UI/UI Components/Row Box For Desk"), _targetContent);
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
            string _position = "";
            if (positionXInputField.text != "" && positionYInputField.text != "")
            {
                _position = positionXInputField.text + positionYInputField.text;
                ShopkeeperFunctions.InsetIntoDesk(PP.ConnectionString, capacityInputField.text, _position);
            }
            else
            {
                PP.MessagePanel.MessageText = "λ��X��λ��Y���벻����";
                PP.MyPanelMachine.PushPanel(PP.MessagePanel);
            }
            UpdateTable();
        }
        else if (isDelete)
        {
            ShopkeeperFunctions.DeleteFromDesk(PP.ConnectionString, DNOInputField.text);
            UpdateTable();
        }
        else if (isUpdate)
        {
            string _position = "";
            if (positionXInputField.text != "" && positionYInputField.text != "")
            {
                _position = positionXInputField.text + positionYInputField.text;
                ShopkeeperFunctions.UpdateDesk(PP.ConnectionString, DNOInputField.text, capacityInputField.text, _position);

            }
            else if (positionXInputField.text == "" && positionYInputField.text != "" || positionXInputField.text != "" && positionYInputField.text == "")
            {
                PP.MessagePanel.MessageText = "λ��X��λ��Y���벻����";
                PP.MyPanelMachine.PushPanel(PP.MessagePanel);
            }
            else ShopkeeperFunctions.UpdateDesk(PP.ConnectionString, DNOInputField.text, capacityInputField.text, _position);

            UpdateTable();
        }
        else
        {
            PP.MessagePanel.MessageText = "��δ�����κ���Ч��Ϣ";
            PP.MyPanelMachine.PushPanel(PP.MessagePanel);   
        }
    }

    private void CheckInputField()
    {
        ChangeFlag(0);
        if (DNOInputField.text != "" && DNOInputField.text.Length == 5 && DNOInputField.text[0] == 'd')
        {
            if (capacityInputField.text == "" && positionXInputField.text == "" && positionYInputField.text == "") ChangeFlag(2);     //�����dno���������������Ϊ�գ�Ϊɾ��
            else if (!(capacityInputField.text == "" && positionXInputField.text == "" && positionYInputField.text == "")) ChangeFlag(3);   //�����dno���������������Ϊ�ղ�������������һ���������������ֵ��Ϊ�޸�
        }
        else
        {
            if (capacityInputField.text != "" && positionXInputField.text != "" && positionYInputField.text != "") ChangeFlag(1);  //���dnoΪ�ջ���Ч�������������������ǿգ�Ϊ����
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
            actionButtonText.text = "��Ӳ���";
            actionButton.interactable = true;
        }
        else if (isDelete)
        {
            actionButtonText.text = "ɾ������";
            actionButton.interactable = true;
        }
        else if (isUpdate)
        {
            actionButtonText.text = "�޸Ĳ���";
            actionButton.interactable = true;
        }
        else
        {
            actionButtonText.text = "�޷�ʹ��";
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
