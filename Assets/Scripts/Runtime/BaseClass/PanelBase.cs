using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame
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
        public virtual bool IsDestoryOnClosed => true;


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

        private void Reset()
        {
            _canvasGroup = _canvasGroup ?? GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// 当面板被打开时
        /// </summary>
        internal protected abstract void OnOpen();

        /// <summary>
        /// 当面板被关闭时
        /// </summary>
        /// <remarks>
        /// 请在动画结束后调用DeathImmediately
        /// </remarks>
        internal protected abstract void OnClose();

        /// <summary>
        /// 当面板显示时
        /// </summary>
        internal protected abstract void OnShow();

        /// <summary>
        /// 当面板隐藏时
        /// </summary>
        internal protected abstract void OnHide();

        /// <summary>
        /// 当面板被摧毁或移入池中时
        /// </summary>
        /// <remarks>
        /// 负责关闭面板后的数据处理
        /// </remarks>
        protected abstract void OnDeath();

        /// <summary>
        /// 立即摧毁
        /// </summary>
        public void DeathImmediately()
        {
            OnDeath();
            if (IsDestoryOnClosed)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                transform.SetParent(ServiceFactory.Instance.GetService<PanelManager>().DeathRoot, false);
            }
        }
    }
}
