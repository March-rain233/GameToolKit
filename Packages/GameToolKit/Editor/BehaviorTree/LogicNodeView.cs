using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameToolKit.Editor;

namespace GameToolKit.Behavior.Tree.Editor
{
    public class LogicNodeView : NodeView<Node>
    {
        public LogicNodeView(LogicNode node) : base(node)
        {
        }
    }
}
