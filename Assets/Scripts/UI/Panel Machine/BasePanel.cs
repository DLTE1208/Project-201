using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板基类，面板状态的基类
/// </summary>
public class BasePanel
{
    public GameObject Panel { get; private set; }   //面板本身
    public string Name { get; protected set; }      //面板名字
    public string Path { get; protected set; }      //面板路径

    protected PanelProcessor PP;
    protected bool isInstantiated;

    public BasePanel(string _path, PanelProcessor _PP)
    {
        PP = _PP;
        Path = _path;
        Name = _path.Substring(_path.LastIndexOf('/') + 1);
        isInstantiated = false;
    }

    /// <summary>
    /// 进入面板机时调用
    /// </summary>
    public virtual void Enter()
    {
        if (!isInstantiated) Instantiate();
    }

    /// <summary>
    /// 退出面板机时调用
    /// </summary>
    public virtual void Exit()
    {
        if (isInstantiated) Destroy();
    }

    /// <summary>
    /// 在面板机内运行时调用
    /// </summary>
    public virtual void LogicUpdate()
    {
    }

    /// <summary>
    /// 在面板机内暂停时调用
    /// </summary>
    public virtual void Pause()
    {
    }

    /// <summary>
    /// 在面板机内恢复运行时调用
    /// </summary>
    public virtual void Resume()
    {
    }

    /// <summary>
    /// 实例化面板，包含以下步骤：先从资源文件夹实例化面板，然后根据名字遍历查找孩子，最后绑定函数
    /// </summary>
    protected virtual void Instantiate()
    {
        Panel = GameObject.Instantiate(Resources.Load<GameObject>(Path), PP.transform);
        isInstantiated = true;
    }

    /// <summary>
    /// 摧毁面板的实例
    /// </summary>
    protected void Destroy()
    {
        GameObject.Destroy(Panel);
        isInstantiated = false;
    }
}
