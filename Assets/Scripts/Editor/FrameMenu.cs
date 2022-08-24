using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;

namespace GameFrame
{
    public class FrameMenu : OdinMenuEditorWindow
    {
        [MenuItem("«≥≤÷”Í§Œπ§æﬂ/Frame Menu")]
        public static void ShowMenu()
        {
            FrameMenu wnd = GetWindow<FrameMenu>();
            wnd.titleContent = new GUIContent("Frame Menu");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var menu = new OdinMenuTree();
            if (EditorApplication.isPlaying)
            {
                menu.Add("EventManager", ServiceFactory.Instance.GetService<EventManager>(), EditorIcons.SmartPhone);
            }
            else
            {

            }
            return menu;
        }
    }
}
