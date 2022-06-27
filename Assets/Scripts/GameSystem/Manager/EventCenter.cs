using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Map;

/// <summary>
/// �¼����ģ������ύ������Ϸ���¼�
/// </summary>
[System.Serializable]
public class EventCenter : SerializedMonoBehaviour
{
    /// <summary>
    /// �����¼�����
    /// </summary>
    [System.Serializable]
    public struct EventArgs
    {
        public float Float;
        public int Int;
        public bool Boolean;
        public string String;
        public object Object;
    }

    /// <summary>
    /// ��Ϣͨ��
    /// </summary>
    [TabGroup("���ݿ�", "�¼�"), DictionaryDrawerSettings(KeyLabel = "�¼���", ValueLabel = "�¼�����", DisplayMode = DictionaryDisplayOptions.OneLine), SerializeField,]
    private Dictionary<string, EventArgs> _events = new Dictionary<string, EventArgs>();

    /// <summary>
    /// ��Ϣ����
    /// </summary>
    public event System.Action<string, EventArgs> EventChanged;

    /// <summary>
    /// �������б�
    /// </summary>
    private Dictionary<string, List<System.Action<EventArgs>>> _listeners = new Dictionary<string, List<System.Action<EventArgs>>>();

    public void Init(Dictionary<string, EventArgs> save)
    {
        _events = save;
    }

    /// <summary>
    /// ��ȡ�¼��Ĳ���
    /// </summary>
    /// <param name="eventName"></param>
    public bool TryGetEventArgs(string eventName, out EventArgs eventArgs)
    {
        if(_events.ContainsKey(eventName))
        {
            eventArgs = _events[eventName];
            return true;
        }
        eventArgs = default;
        return false;
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventArgs"></param>
    public void SendEvent(string eventName, EventArgs eventArgs)
    {
        if (!_events.ContainsKey(eventName))
        {
            _events.Add(eventName, eventArgs);
        }
        else
        {
            _events[eventName] = eventArgs;
        }

        EventChanged?.Invoke(eventName, eventArgs);
        if (!_listeners.ContainsKey(eventName)) { return; }

        foreach(var h in _listeners[eventName])
        {
            h.Invoke(eventArgs);
        }
    }

    /// <summary>
    /// ��Ӽ�����
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="callback"></param>
    public void AddListener(string eventName, System.Action<EventArgs> callback)
    {
        if (!_listeners.ContainsKey(eventName))
        {
            _listeners.Add(eventName, new List<System.Action<EventArgs>>());
        }
        _listeners[eventName].Add(callback);
    }

    /// <summary>
    /// �Ƴ�������
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="callback"></param>
    public void RemoveListener(string eventName, System.Action<EventArgs> callback)
    {
        _listeners[eventName].Remove(callback);
    }
}
