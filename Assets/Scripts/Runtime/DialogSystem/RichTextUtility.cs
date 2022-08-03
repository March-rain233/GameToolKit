using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameFrame.Utility
{
    public static class RichTextUtility
    {
        /// <summary>
        /// 获取纯文本
        /// </summary>
        /// <param name="rawText"></param>
        /// <returns></returns>
        public static string GetPlainText(string rawText)
        {
            return Regex.Replace(rawText, @"<[^<>]*>", "");
        }
    }
}