using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    public class PanelManager : IService
    {
        /// <summary>
        /// UI���ڵ�
        /// </summary>
        public Canvas Root { get; private set; }

        /// <summary>
        /// ���ֿ���״̬�������ڵ�
        /// </summary>
        public RectTransform ActiveRoot { get; private set; }

        /// <summary>
        /// ����ر�״̬�������ڵ�
        /// </summary>
        public RectTransform DeathRoot { get; private set; }

        /// <summary>
        /// UI�����
        /// </summary>
        public Camera UICamera { get; private set; }

        Stack<PanelBase> _panelStack = new Stack<PanelBase>();

        /// <summary>
        /// ��ǰ�ö����
        /// </summary>
        public PanelBase Top => _panelStack.Peek();

        void IService.Init()
        {
            //���ڵ�����
            Root = new GameObject("UIRoot", typeof(Canvas)).GetComponent<Canvas>();
            DeathRoot = new GameObject("DeathRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            DeathRoot.SetParent(Root.transform, false);
            ActiveRoot = new GameObject("ActiveRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            ActiveRoot.SetParent(Root.transform, false);

            //����趨
            UICamera = new GameObject("UICamera", typeof(Camera)).GetComponent<Camera>();
            UICamera.cullingMask = LayerMask.GetMask("UI");
            UICamera.orthographic = true;

            //�������趨
            Root.worldCamera = UICamera;
            Root.renderMode = RenderMode.ScreenSpaceCamera;

            DeathRoot.gameObject.SetActive(false);

            //����Ϊ�־û�����
            Object.DontDestroyOnLoad(Root.gameObject);
            Object.DontDestroyOnLoad(UICamera);
        }

        /// <summary>
        /// ѹ�����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PanelBase Push(string name)
        {
            //todo:Ŀǰ��ʹ��ջ�Ľṹ��δ���������ٿ��ǻụ����ӵ�������ν��
            PanelBase panel = CreatePanel(name);
            if(_panelStack.TryPeek(out var peek))
            {
                peek.OnHide();
            }
            _panelStack.Push(panel);
            panel.OnOpen();
            return panel;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <returns></returns>
        public void Pop()
        {
            _panelStack.Pop().OnClose();
            if (_panelStack.TryPeek(out var peek))
            {
                peek.OnShow();
            }
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        PanelBase CreatePanel(string name)
        {
            var res = DeathRoot.Find(name);
            if(res == null)
            {
                res = Object.Instantiate(UISetting.Instance.PrefabsDic[name].gameObject, ActiveRoot).transform;
                res.name = name;
            }
            else
            {
                res.SetParent(ActiveRoot, false);
            }
            return res.GetComponent<PanelBase>();
        }
    }
}
