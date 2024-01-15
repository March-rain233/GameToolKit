using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    public class ExpressionTreeNode : IComparer<ExpressionTreeNode>
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct Data
        {
            [FieldOffset(0)]
            public int Int;
            [FieldOffset(0)]
            public float Float;
            [FieldOffset(0)]
            public double Double;
            [FieldOffset(0)]
            public bool Bool;
            [FieldOffset(8)]
            public string String;
            [FieldOffset(8)]
            public object Object;
        }
        public int Layer;
        public ExpressionTreeNode Parent;
        public ExpressionTreeNode[] Children;
        public Data Value;
        public Type Type;
        public Action<ExpressionTreeNode> Function;

        public int Compare(ExpressionTreeNode x, ExpressionTreeNode y)
        {
            return y.Layer - x.Layer;
        }
    }
}
