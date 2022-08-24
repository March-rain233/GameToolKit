using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameFrame
{
    /// <summary>
    /// ������
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PanelBase : SerializedMonoBehaviour
    {
        /// <summary>
        /// �Ƿ��ڹرյ�ʱ��ݻ�
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
        /// ����屻��ʱ
        /// </summary>
        internal protected abstract void OnOpen();

        /// <summary>
        /// ����屻�ر�ʱ
        /// </summary>
        /// <remarks>
        /// ���ڶ������������DeathImmediately
        /// </remarks>
        internal protected abstract void OnClose();

        /// <summary>
        /// �������ʾʱ
        /// </summary>
        internal protected abstract void OnShow();

        /// <summary>
        /// ���������ʱ
        /// </summary>
        internal protected abstract void OnHide();

        /// <summary>
        /// ����屻�ݻٻ��������ʱ
        /// </summary>
        /// <remarks>
        /// ����ر���������ݴ���
        /// </remarks>
        protected abstract void OnDeath();

        /// <summary>
        /// �����ݻ�
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
