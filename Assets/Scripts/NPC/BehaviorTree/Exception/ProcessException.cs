using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace GameFrame.Behavior.Tree
{
    /// <summary>
    /// ���̵����쳣
    /// </summary>
    [SerializeField]
    public class ProcessException : System.ApplicationException
    {
        public Node Node;
        public ProcessException(Node node, string msg):base(msg)
        {
            Node = node;
        }
    }
}
