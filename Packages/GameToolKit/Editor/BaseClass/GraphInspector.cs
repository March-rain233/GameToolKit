using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;

namespace GameToolKit.Editor
{
    /// <summary>
    /// ͼ������
    /// </summary>
    public class GraphInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<GraphInspector> { }

        public VisualTreeAsset VisualTreeAsset;
        private VisualElement _mainContainer;
        private VisualElement _contentContainer;
        private Label _title;
        private TabbedView _tabbedView;

        private Dictionary<string, TabButton> _tabDic = new Dictionary<string, TabButton>();
        public override VisualElement contentContainer => _contentContainer;

        public string Tittle { get=>_title.text; set => _title.text = value; }

        public GraphInspector():this(null)
        {

        }


        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="tabs">��ҳ��</param>
        /// <param name="width">���</param>
        /// <param name="height">�߶�</param>
        public GraphInspector(string[] tabs = null)
        {
            //���ؽṹ
            var visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.march_rain.gametoolkit/Editor/BaseClass/UXML/GraphInspector.uxml");
            _mainContainer = visualAsset.Instantiate();
            var titleContainer = this.Q("TitleContainer");
            base.hierarchy.Add(_mainContainer);
            this.Q<TemplateContainer>().style.flexGrow = 1;

            //��ȡ���
            _contentContainer = this.Q("contentContainer");
            _title = this.Q<Label>("title");

            //��ӹ���
            RegisterCallback<WheelEvent>(e=>e.StopImmediatePropagation());//��ֹ����Ӱ�쵽��һ������

            //��ӷ�ҳ
            if(tabs != null && tabs.Length > 0)
            {
                _tabbedView = new TabbedView();
                foreach(var tab in tabs)
                {
                    var container = new VisualElement();
                    var button = new TabButton(tab, container);
                    _tabbedView.AddTab(button, true);
                    _tabDic[tab] = button;
                }
                Add(_tabbedView);
            }

        }

        #region ��ҳ����
        /// <summary>
        /// ��ȡ��ҳ����
        /// </summary>
        /// <returns></returns>
        public string[] GetTabs()
        {
            string[] tabs = new string[_tabDic.Count];
            _tabDic.Keys.CopyTo(tabs, 0);
            return tabs;
        }

        /// <summary>
        /// ����������ָ����ҳ
        /// </summary>
        /// <param name="element">���</param>
        /// <param name="tab">��ҳ��������ҳΪnullʱĬ�ϼ��뵽��һ����ҳ��</param>
        public void AddToTab(GraphElementField element, string tab = null)
        {
            if(tab == null)
            {
                if(_tabbedView != null)
                {
                    _tabbedView.Q<TabButton>().Target.Add(element);
                }
                else
                {
                    Add(element);
                }
            }
            else if(_tabDic.TryGetValue(tab, out var button))
            {
                button.Target.Add(element);
            }
        }

        /// <summary>
        /// �������ָ����ҳ�Ƴ�
        /// </summary>
        /// <param name="element">���</param>
        /// <param name="tab">��ҳ��������ҳΪnullʱĬ�ϼ��뵽��һ����ҳ��</param>
        public void RemoveFromTab(GraphElementField element, string tab = null)
        {
            if (tab == null)
            {
                if (_tabbedView != null)
                {
                    _tabbedView.Q<TabButton>().Target.Remove(element);
                }
                else
                {
                    Remove(element);
                }
            }
            else if (_tabDic.TryGetValue(tab, out var button))
            {
                button.Target.Remove(element);
            }
        }

        /// <summary>
        /// ���ָ����ҳ�����
        /// </summary>
        /// <param name="tab">��ҳ��������ҳΪnullʱĬ�ϼ��뵽��һ����ҳ��</param>
        public void ClearTab(string tab = null)
        {
            if (tab == null)
            {
                if (_tabbedView != null)
                {
                    _tabbedView.Q<TabButton>().Target.Clear();
                }
                else
                {
                    Clear();
                }
            }
            else if (_tabDic.TryGetValue(tab, out var button))
            {
                button.Target.Clear();
            }
        }

        /// <summary>
        /// ���ȫ����ҳ�����
        /// </summary>
        public void ClearTabAll()
        {
            if (_tabbedView != null)
            {
                foreach(var tab in _tabDic.Values)
                {
                    tab.Target.Clear();
                }
            }
            else
            {
                Clear();
            }
        }

        /// <summary>
        /// ��ѯ�Ƿ���ڷ�ҳ
        /// </summary>
        /// <param name="tab"></param>
        public bool ContainTab(string tab)
        {
            return _tabDic.ContainsKey(tab);
        }

        /// <summary>
        /// ��ӷ�ҳ
        /// </summary>
        /// <param name="tab">��ҳ��</param>
        /// <returns>��ҳ�󶨵�����</returns>
        public VisualElement AddTab(string tab)
        {
            if(_tabbedView == null)
            {
                Clear();
                _tabbedView = new TabbedView();
                Add(_tabbedView);
            }
            VisualElement container = new VisualElement();
            TabButton button = new TabButton(tab, container);
            _tabbedView.AddTab(button, true);
            _tabDic[tab] = button;
            return container;
        }

        /// <summary>
        /// �Ƴ���ҳ
        /// </summary>
        /// <param name="tab"></param>
        public void RemoveTab(string tab)
        {
            if(_tabDic.TryGetValue(tab, out var button))
            {
                _tabbedView.RemoveTab(button);
                if(_tabbedView.childCount <= 0)
                {
                    Remove(_tabbedView);
                    _tabbedView = null;
                }
            }
        }
        #endregion

        #region ���ҹ����ֶ�
        /// <summary>
        /// ��ȡ�봫������������ĵ�һ���ֶ�
        /// </summary>
        /// <param name="value">����</param>
        /// <param name="tab">�ֶ����ڵķ�ҳ</param>
        /// <param name="field">�ֶ�</param>
        /// <returns></returns>
        public bool TryGetAssociatedField(object value, out string tab, out GraphElementField field)
        {
            if (_tabbedView == null)
            {
                foreach (GraphElementField fieldItem in Children())
                {
                    if (fieldItem.IsAssociatedWith(value))
                    {
                        tab = null;
                        field = fieldItem;
                        return true;
                    }
                }
            }
            else
            {
                foreach (var item in _tabDic)
                {
                    foreach (GraphElementField fieldItem in item.Value.Target.Children())
                    {
                        if (fieldItem.IsAssociatedWith(value))
                        {
                            tab = item.Key;
                            field = fieldItem;
                            return true;
                        }
                    }
                }
            }
            tab = null;
            field = null;
            return false;
        }

        /// <summary>
        /// ��ȡָ����ҳ���봫������������ĵ�һ���ֶ�
        /// </summary>
        /// <param name="value">����</param>
        /// <param name="tab">�ֶ����ڵķ�ҳ</param>
        /// <param name="field">�ֶ�</param>
        /// <returns></returns>
        public bool TryGetAssociatedField(object value, string tab, out GraphElementField field)
        {
            if(_tabbedView == null)
            {
                return TryGetAssociatedField(value, out tab, out field);
            }
            if(_tabDic.TryGetValue(tab, out var tabButton))
            {
                foreach (GraphElementField fieldItem in tabButton.Target.Children())
                {
                    if (fieldItem.IsAssociatedWith(value))
                    {
                        field = fieldItem;
                        return true;
                    }
                }
            }
            field = null;
            return false;
        }

        /// <summary>
        /// ��ȡ�봫������������������ֶ�
        /// </summary>
        /// <param name="value">����</param>
        /// <param name="list">�ֶ��б�</param>
        /// <returns></returns>
        public bool TryGetAssociatedFieldAll(object value, out List<KeyValuePair<string, GraphElementField>> list)
        {
            list = new List<KeyValuePair<string, GraphElementField>>();
            if(_tabbedView == null)
            {
                foreach (GraphElementField fieldItem in Children())
                {
                    if (fieldItem.IsAssociatedWith(value))
                    {
                        list.Add(new KeyValuePair<string, GraphElementField>(null, fieldItem));
                    }
                }
            }
            else
            {
                foreach (var item in _tabDic)
                {
                    foreach (GraphElementField fieldItem in item.Value.Target.Children())
                    {
                        if (fieldItem.IsAssociatedWith(value))
                        {
                            list.Add(new KeyValuePair<string, GraphElementField>(item.Key, fieldItem));
                        }
                    }
                }
            }
            return list.Count > 0;
        }

        /// <summary>
        /// ��ȡָ����ҳ���봫������������������ֶ�
        /// </summary>
        /// <param name="value">����</param>
        /// <param name="tab">�ֶ����ڵķ�ҳ</param>
        /// <param name="list">�ֶ��б�</param>
        /// <returns></returns>
        public bool TryGetAssociatedFieldAll(object value, string tab, out List<GraphElementField> list)
        {
            list = new List<GraphElementField>();
            if (_tabbedView == null)
            {
                foreach (GraphElementField fieldItem in Children())
                {
                    if (fieldItem.IsAssociatedWith(value))
                    {
                        list.Add(fieldItem);
                    }
                }
            }
            else
            {
                if (_tabDic.TryGetValue(tab, out var tabButton))
                {
                    foreach (GraphElementField fieldItem in tabButton.Target.Children())
                    {
                        if (fieldItem.IsAssociatedWith(value))
                        {
                            list.Add(fieldItem);
                        }
                    }
                }
            }
            return list.Count > 0;
        }
        #endregion
    }
}
