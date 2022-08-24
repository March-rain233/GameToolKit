using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame
{
    public class PanelManager : IService
    {
        /// <summary>
        /// UI根节点
        /// </summary>
        public Canvas Root { get; private set; }

        /// <summary>
        /// 保持开启状态的面板根节点
        /// </summary>
        public RectTransform ActiveRoot { get; private set; }

        /// <summary>
        /// 进入关闭状态的面板根节点
        /// </summary>
        public RectTransform DeathRoot { get; private set; }

        /// <summary>
        /// UI摄像机
        /// </summary>
        public Camera UICamera { get; private set; }

        Stack<PanelBase> _panelStack = new Stack<PanelBase>();

        /// <summary>
        /// 当前置顶面板
        /// </summary>
        public PanelBase Top => _panelStack.Peek();

        void IService.Init()
        {
            //根节点生成
            Root = new GameObject("UIRoot", typeof(Canvas)).GetComponent<Canvas>();
            DeathRoot = new GameObject("DeathRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            DeathRoot.SetParent(Root.transform, false);
            ActiveRoot = new GameObject("ActiveRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            ActiveRoot.SetParent(Root.transform, false);

            //相机设定
            UICamera = new GameObject("UICamera", typeof(Camera)).GetComponent<Camera>();
            UICamera.cullingMask = LayerMask.GetMask("UI");
            UICamera.orthographic = true;

            //根画布设定
            Root.worldCamera = UICamera;
            Root.renderMode = RenderMode.ScreenSpaceCamera;

            DeathRoot.gameObject.SetActive(false);

            //设置为持久化物体
            Object.DontDestroyOnLoad(Root.gameObject);
            Object.DontDestroyOnLoad(UICamera);
        }

        /// <summary>
        /// 压入面板
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PanelBase Push(string name)
        {
            //todo:目前先使用栈的结构，未来遇到了再考虑会互相叠加的面板该如何解决
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
        /// 弹出面板
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
        /// 创建面板
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
