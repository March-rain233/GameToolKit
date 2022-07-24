using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFrame.Dialog
{
    public class DialogTree : CustomGraph<Node>
    {
        public event Action DialogBeginning;
        public event Action DialogClosed;
    }
}
