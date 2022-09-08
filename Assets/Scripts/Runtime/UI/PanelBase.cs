using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameToolKit
{
    /// <summary>
    /// 面板基类
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PanelBase : SerializedMonoBehaviour
    {
        /// <summary>
        /// 是否在关闭的时候摧毁
        /// </summary>
        [ReadOnly, ShowInInspector]
        public virtual bool IsDestoryOnClosed => true;

        [ReadOnly, ShowInInspector]
        public virtual PanelShowType ShowType => PanelShowType.Normal;

        protected CanvasGroup _canvasGroup;

        public bool BlocksRaycasts
        {
            get { return _canvasGroup.blocksRaycasts; }
            set { _canvasGroup.blocksRaycasts = value; }
        }

        public bool Interactable
        {
            get { return _canvasGroup.interactable; }
            set { _canvasGroup.interactable = value; }
        }

        public float Alpha
        {
            get { return _canvasGroup.alpha; }
            set { _canvasGroup.alpha = value; }
        }

        /// <summary>
        /// 当前窗口状态
        /// </summary>

        public bool IsShowing { get; private set; } = false;

        protected virtual void Reset()
        {
            _canvasGroup = _canvasGroup ?? GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// 当面板数据初始化
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// 当面板被打开时
        /// </summary>
        protected abstract void OnOpen();

        internal void Open()
        {
            IsShowing = true;
            OnInit();
            OnOpen();
        }

        /// <summary>
        /// 当面板被关闭时
        /// </summary>
        /// <remarks>
        /// 请在动画结束后调用DeathImmediately
        /// </remarks>
        protected abstract void OnClose();

        internal void Close()
        {
            IsShowing = false;
            OnClose();
        }

        /// <summary>
        /// 当面板显示时
        /// </summary>
        protected abstract void OnShow();

        internal void Show()
        {
            if (!IsShowing)
            {
                IsShowing = true;
                OnShow();
            }
        }

        /// <summary>
        /// 当面板隐藏时
        /// </summary>
        protected abstract void OnHide();

        internal void Hide()
        {
            if (IsShowing)
            {
                IsShowing = false;
                OnHide();
            }
        }

        /// <summary>
        /// 当面板被摧毁或移入池中时
        /// </summary>
        /// <remarks>
        /// 负责关闭面板后的数据处理
        /// </remarks>
        protected abstract void OnDispose();

        /// <summary>
        /// 立即摧毁
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            if (IsDestoryOnClosed)
            {
                Destroy(gameObject);
            }
            else
            {
                ServiceFactory.Instance.GetService<PanelManager>().RecyclePanel(this);
            }
        }
    }

    [Flags]
    public enum PanelShowType
    {
        /// <summary>
        /// 与多个面板叠加
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 反向切换
        /// </summary>
        /// <remarks>
        /// 将在开启时隐藏上一个拥有该标签的面板，在关闭时恢复上一个拥有该标签的面板
        /// </remarks>
        ReverseChange = 1,
        /// <summary>
        /// 隐藏所有
        /// </summary>
        /// <remarks>
        /// 在开启时将隐藏从此面板到上一个拥有该标签的面板之间的所有面板，关闭时反向
        /// </remarks>
        HideOther = ReverseChange << 1,
    }
}
