using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����࣬���״̬�Ļ���
/// </summary>
public class BasePanel
{
    public GameObject Panel { get; private set; }   //��屾��
    public string Name { get; protected set; }      //�������
    public string Path { get; protected set; }      //���·��

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
    /// ��������ʱ����
    /// </summary>
    public virtual void Enter()
    {
        if (!isInstantiated) Instantiate();
    }

    /// <summary>
    /// �˳�����ʱ����
    /// </summary>
    public virtual void Exit()
    {
        if (isInstantiated) Destroy();
    }

    /// <summary>
    /// ������������ʱ����
    /// </summary>
    public virtual void LogicUpdate()
    {
    }

    /// <summary>
    /// ����������ͣʱ����
    /// </summary>
    public virtual void Pause()
    {
    }

    /// <summary>
    /// �������ڻָ�����ʱ����
    /// </summary>
    public virtual void Resume()
    {
    }

    /// <summary>
    /// ʵ������壬�������²��裺�ȴ���Դ�ļ���ʵ������壬Ȼ��������ֱ������Һ��ӣ����󶨺���
    /// </summary>
    protected virtual void Instantiate()
    {
        Panel = GameObject.Instantiate(Resources.Load<GameObject>(Path), PP.transform);
        isInstantiated = true;
    }

    /// <summary>
    /// �ݻ�����ʵ��
    /// </summary>
    protected void Destroy()
    {
        GameObject.Destroy(Panel);
        isInstantiated = false;
    }
}
