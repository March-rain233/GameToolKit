using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
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
