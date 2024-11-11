using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//一定要引入下面这两个命名空间
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 加载面板
/// </summary>
public class LoadingPanel : BasePanel
{
    public BasePanel NextPanel;
    public float LoadingDuration;

    private Image loadingImage;
    private float loadingTime;
    private bool isDone;
    private float defaultLoadingDuration;

    public LoadingPanel(string _path, PanelProcessor _PP) : base(_path, _PP)
    {
        NextPanel = null;
        defaultLoadingDuration = 1f;
        LoadingDuration = defaultLoadingDuration;
        loadingTime = 0f;
        isDone = false;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        NextPanel = null;
        LoadingDuration = defaultLoadingDuration;
        loadingTime = 0f;
        isDone = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isDone)
        {
            PP.MyPanelMachine.ChangePanel(NextPanel);
        }

        loadingTime += Time.deltaTime;
        if (loadingTime >= LoadingDuration)
        {
            loadingImage.fillAmount = 1f;
            isDone = true;
        }
        else loadingImage.fillAmount = loadingTime / LoadingDuration;

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
            if (_item.name == "Loading Image") loadingImage = _item.GetComponent<Image>();
        }
    }
}
