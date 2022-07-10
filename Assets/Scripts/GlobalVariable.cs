using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Cinemachine;

namespace GameFrame
{
    [CreateAssetMenu(fileName = "GlobalVariable", menuName = "Config/GlobalVariable")]
    public class GlobalVariable : SerializedScriptableObject, IBlackboard
    {
#if UNITY_EDITOR
        private Dictionary<string, Hashtable> _callbacks = new Dictionary<string, Hashtable>();
        public void RenameValue(string name, string newName)
        {
            {
                var temp = _variables[name];
                _variables.Remove(name);
                _variables.Add(newName, temp);
            }
            var e = new IBlackboard.NameChangedEvent(this, name, newName);
            foreach (var callback in _callbacks[name][typeof(IBlackboard.NameChangedEvent)] as List<Action<IBlackboard.NameChangedEvent>>)
            {
                callback(e);
            }
            {
                var temp = _callbacks[name];
                _callbacks.Remove(name);
                _callbacks.Add(name, temp);
            }
        }
        public void RegisterCallback<T>(string name, Action<T> callback) where T : IBlackboard.BlackboardEventBase
        {
            (_callbacks[name][typeof(T)] as List<Action<T>>).Add(callback);
        }
        public Dictionary<string, BlackboardVariable> GetVariable()
        {
            return new Dictionary<string, BlackboardVariable>(_variables);
        }
        public BlackboardVariable GetVariable(string name)
        {
            return _variables[name];
        }
#endif
        static public GlobalVariable Instance { get; private set; }
        [NoSaveDuringPlay]
        [SerializeField]
        private Dictionary<string, BlackboardVariable> _variables = new Dictionary<string, BlackboardVariable>();
        private void OnEnable()
        {
            if (Instance)
            {
                Debug.LogError($"Singleton initialization error: an instance {Instance} already exists");
                return;
            }
            Instance = this;
        }
        public T GetValue<T>(string name)
        {
            return (T)_variables[name].Value;
        }
        public bool HasValue(string name)
        {
            return _variables.ContainsKey(name);
        }
        public void RemoveValue(string name)
        {
            _variables.Remove(name);
#if UNITY_EDITOR
            var e = new IBlackboard.ValueRemoveEvent(this, name);
            foreach(var callback in _callbacks[name][typeof(IBlackboard.ValueRemoveEvent)] as List<Action<IBlackboard.ValueRemoveEvent>>)
            {
                callback(e);
            }
            _callbacks.Remove(name);
#endif
        }
        public void SetValue(string name, object value)
        {
            if (HasValue(name))
            {
                _variables[name].Value = value;
            }
            else
            {
                _variables.Add(name, new ObjectVariable() { Value = value });
#if UNITY_EDITOR
                _callbacks.Add(name, new Hashtable()
                {
                    {typeof(IBlackboard.NameChangedEvent),new List<Action<IBlackboard.NameChangedEvent>>()},
                    {typeof(IBlackboard.ValueRemoveEvent),new List<Action<IBlackboard.ValueRemoveEvent>>()},
                });
#endif
            }
        }
    }
}