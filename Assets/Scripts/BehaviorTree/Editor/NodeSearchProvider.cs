using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameFrame.Behavior.Tree.Editor
{
    public class NodeSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            var typeList = TypeCache.GetTypesDerivedFrom<Node>();
            var groups = TypeCache.GetTypesWithAttribute<NodeCategoryAttribute>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Node"), 0));
            foreach (var g in groups)
            {
                var attr = g.GetCustomAttributes(typeof(NodeCategoryAttribute), false)[0] as NodeCategoryAttribute;
                if (attr.Category == "NULL")
                    continue;
                int index = 0;
                var path = attr.Category.Split('/');
                for (int i = 0; i < path.Length; i++)
                {
                    var find = tree.FindIndex(item => item.content.text == path[i]);
                    if (find == -1)
                    {
                        tree.Insert(index + 1, new SearchTreeGroupEntry(new GUIContent(path[i]), i + 1));
                        find = index + 1;
                    }
                    index = find;
                }
            }
            foreach (var type in typeList)
            {
                if (type.IsAbstract)
                    continue;
                var category = (type.GetCustomAttributes(typeof(NodeCategoryAttribute), true)[0] as NodeCategoryAttribute).Category;
                if (category == "NULL")
                {
                    continue;
                }
                int level = category.Split('/').Length;
                category = category.Split('/')[level - 1];
                int index = tree.FindIndex((item) => item.content.text == category);
                if (index == -1)
                {
                    Debug.LogError("出现了未定义分类的节点");
                }
                else
                {
                    var attr = type.GetCustomAttributes(typeof(NodeNameAttribute), false);
                    string name;
                    if (attr.Length == 0)
                    {
                        name = type.Name;
                    }
                    else
                    {
                        name = ((NodeNameAttribute)attr[0]).Name;
                    }
                    tree.Insert(
                        index + 1, 
                        new SearchTreeEntry(new GUIContent("    " + name)){ 
                            level = level + 1,
                            userData = type,
                        });
                }
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var editor = BehaviorTreeEditor.GetWindow<BehaviorTreeEditor>();
            var tree = editor.TreeView;
            var node = tree.CreateNode(SearchTreeEntry.userData as System.Type);
            var windowRoot = editor.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - editor.position.position);
            var graphMousePosition = tree.contentViewContainer.WorldToLocal(windowMousePosition);
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
            return true;
        }
    }
}