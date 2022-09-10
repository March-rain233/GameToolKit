using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace GameToolKit
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
        /// ��ǰ����״̬
        /// </summary>

        public bool IsShowing { get; private set; } = false;

        protected virtual void Reset()
        {
            _canvasGroup = _canvasGroup ?? GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// ��������ݳ�ʼ��
        /// </summary>
        protected abstract void OnInit();

        /// <summary>
        /// ����屻��ʱ
        /// </summary>
        protected abstract void OnOpen();

        internal void Open()
        {
            IsShowing = true;
            OnInit();
            OnOpen();
        }

        /// <summary>
        /// ����屻�ر�ʱ
        /// </summary>
        /// <remarks>
        /// ���ڶ������������DeathImmediately
        /// </remarks>
        protected abstract void OnClose();

        internal void Close()
        {
            IsShowing = false;
            OnClose();
        }

        /// <summary>
        /// �������ʾʱ
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
        /// ���������ʱ
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
        /// ����屻�ݻٻ��������ʱ
        /// </summary>
        /// <remarks>
        /// ����ر���������ݴ���
        /// </remarks>
        protected abstract void OnDispose();

        /// <summary>
        /// �����ݻ�
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
        /// ����������
        /// </summary>
        Normal = 0,
        /// <summary>
        /// �����л�
        /// </summary>
        /// <remarks>
        /// ���ڿ���ʱ������һ��ӵ�иñ�ǩ����壬�ڹر�ʱ�ָ���һ��ӵ�иñ�ǩ�����
        /// </remarks>
        ReverseChange = 1,
        /// <summary>
        /// ��������
        /// </summary>
        /// <remarks>
        /// �ڿ���ʱ�����شӴ���嵽��һ��ӵ�иñ�ǩ�����֮���������壬�ر�ʱ����
        /// </remarks>
        HideOther = ReverseChange << 1,
    }
}
