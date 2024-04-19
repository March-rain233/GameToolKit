using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Editor;

namespace GameToolKit.Dialog.Editor
{
    public class LogicNodeView : NodeView<Node>
    {
        public LogicNodeView(LogicNode node) : base(node)
        {
        }
    }
}
