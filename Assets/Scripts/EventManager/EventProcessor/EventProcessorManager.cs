using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.EventProcessor
{
    public class EventProcessorManager
    {
        public static EventProcessorManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EventProcessorManager();
                }
                return _instance;
            }
        }
        static EventProcessorManager _instance;
        /// <summary>
        /// �����еĴ�����
        /// </summary>
        List<EventProcessor> _runningProcessorList;
        private EventProcessorManager() { }
        /// <summary>
        /// ���ô�����
        /// </summary>
        /// <param name="processor"></param>
        public void AttachProcessor(EventProcessor processor)
        {
            processor.Attach();
            _runningProcessorList.Add(processor);
        }
        /// <summary>
        /// �Ƴ�������
        /// </summary>
        /// <param name="processor"></param>
        public void DetachProcessor(EventProcessor processor)
        {
            processor.Detach();
            _runningProcessorList.Remove(processor);
        }
        /// <summary>
        /// ��ȡ�����еĴ������б�
        /// </summary>
        /// <returns></returns>
        public List<EventProcessor> GetEventProcessors()
        {
            return new List<EventProcessor>(_runningProcessorList);
        }
    }
}
