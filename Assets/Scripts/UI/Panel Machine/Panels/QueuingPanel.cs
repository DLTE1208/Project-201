using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

public class QueuingPanel : BasePanel
{
    private Button leaveButton;
    private TMP_Text timeText;
    private TMP_Text seqText;
    private Image timeCircleImage;
    private System.DateTime queueTime;
    private System.TimeSpan waitTime;
    private float loopTime;
    private float loopDuration;
    private bool loopIsDone;

    public QueuingPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
        loopDuration = 1f;
    }

    public override void Enter()
    {
        base.Enter();

        CustomerFunctions.EnterQueue(PP.ConnectionString, PP.CNO);
        queueTime = System.DateTime.Now;
        loopTime = 0f;
        loopIsDone = false;
    }

    public override void Exit()
    {
        base.Exit();

        CustomerFunctions.LeaveQueue(PP.ConnectionString, PP.CNO);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        UpdateTime();
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
            if (_item.name == "Leave Button") leaveButton = _item.GetComponent<Button>();
            else if (_item.name == "Time Text") timeText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Seq Text") seqText = _item.GetComponent<TMP_Text>();
            else if (_item.name == "Time Circle Image") timeCircleImage = _item.GetComponent<Image>();
        }

        leaveButton.onClick.AddListener(() => { PP.MyPanelMachine.PopPanel(); });
    }

    private void UpdateTime()
    {
        waitTime = System.DateTime.Now - queueTime;
        timeText.text = waitTime.Seconds.ToString();
        seqText.text = CustomerFunctions.GetQueueSeq(PP.ConnectionString, PP.CNO);

        if (loopIsDone)
        {
            loopTime = 0f;
            loopIsDone = false;
        }
        else
        {
            loopTime += Time.deltaTime;
            if (loopTime >= loopDuration)
            {
                timeCircleImage.fillAmount = 1f;
                loopIsDone = true;
            }
        }
        timeCircleImage.fillAmount = loopTime / loopDuration;
    }
}
