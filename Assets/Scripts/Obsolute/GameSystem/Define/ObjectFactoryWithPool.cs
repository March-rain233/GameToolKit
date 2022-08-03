using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ������صĶ��󹤳�
/// </summary>
/// <typeparam name="T">��������</typeparam>
/// <remarks>
/// �����ж�ȡ�򵥵�ԭʼ���ݶ�ȡ�������������ɫ�ȸ��ӽṹ����
/// </remarks>
public class ObjectFactoryWithPool
{
    private Dictionary<string, Counter> _pool;

    /// <summary>
    /// ������������
    /// </summary>
    private int _maxNum;

    /// <summary>
    /// ��������ί��
    /// </summary>
    /// <remarks>
    /// ��������ڲ�ѯ��������ʱ�����ø�ί�д�������
    /// </remarks>
    private Func<string, IProduct> CreatObject;

    /// <param name="creatObject">��������ķ���</param>
    /// <param name="capacity">���������</param>
    public ObjectFactoryWithPool(Func<string, IProduct> creatObject, int capacity = int.MaxValue)
    {
        CreatObject = creatObject;
        _maxNum = capacity;
        _pool = new Dictionary<string, Counter>(capacity);
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IProduct Create(string name)
    {
        if (_pool.ContainsKey(name))
        {
            _pool[name].Time += 1;
            return _pool[name].Resource.Clone();
        }

        IProduct res = CreatObject.Invoke(name);
        Push(name, res);
        return res.Clone();
    }

    /// <summary>
    /// ��ն����
    /// </summary>
    public void Clear()
    {
        _pool = new Dictionary<string, Counter>();
    }

    /// <summary>
    /// �������������ָ����Ŀ
    /// </summary>
    /// <remarks>
    /// ����ͨ����д���ı��������
    /// </remarks>
    protected virtual void ResetPool(int capacity)
    {

        if (capacity >= _pool.Count)
        {
            return;
        }
        if (capacity == 0)
        {
            Clear();
            return;
        }

        //TODO: �Ż�ɾ����ɸѡ
        int toDelete = _pool.Count - capacity;
        var sorters = new List<Sorter>(toDelete);
        int num = 0;

        string[] keys = new string[_pool.Count];
        _pool.Keys.CopyTo(keys, 0);
        //ɾ��ʹ�����ٵ����������Ķ���
        for (int i = _pool.Count - 1; i >= 0; --i)
        {
            if (num < toDelete)
            {
                sorters[num++] = new Sorter
                {
                    Key = keys[i],
                    Time = _pool[keys[i]].Time
                };
            }
            else
            {
                //�滻���������������ݣ������Կ�Ч����
                sorters.Sort((Sorter a, Sorter b) => {
                    if (a.Time > b.Time) return 1;
                    else if (a.Time == b.Time) return 0;
                    else return -1;
                });
                if (sorters[0].Time >= _pool[keys[i]].Time)
                {
                    sorters[0] = new Sorter
                    {
                        Key = keys[i],
                        Time = _pool[keys[i]].Time
                    };
                }
            }

            //��ֹ���ݳ�������ÿһ�α���������һ����������
            if (_pool[keys[i]].Time > 0)
            {
                _pool[keys[i]].Time -= 1;
            }
        }

        //�����Ӧ��ֵ��
        foreach (var p in sorters)
        {
            _pool.Remove(p.Key);
        }
    }

    /// <summary>
    /// ��ָ�������������
    /// </summary>
    /// <param name="name"></param>
    public virtual void Push(string name, IProduct obj)
    {
        if (_pool.Count + 1 > _maxNum)
        {
            ResetPool(_maxNum - 1);
        }
        _pool.Add(name, new Counter { Resource = obj, Time = 1 });
    }

    /// <summary>
    /// ��ָ������ӳ���ȡ��
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <remarks>
    /// ���ϣ�������Ķ��󲻴洢�ڶ���أ�����ø÷���
    /// </remarks>
    public virtual IProduct Pull(string name)
    {
        if (_pool.ContainsKey(name))
        {
            var temp = _pool[name].Resource;
            _pool.Remove(name);
            return temp;
        }
        return CreatObject.Invoke(name);
    }

    /// <summary>
    /// ������
    /// </summary>
    private class Counter
    {
        public IProduct Resource;
        public int Time;
    }

    /// <summary>
    /// ��������Դ����
    /// </summary>
    private struct Sorter
    {
        public int Time;
        public string Key;
    }
}
