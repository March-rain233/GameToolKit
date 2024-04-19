using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameToolKit.Editor
{
    public class BlackboardSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        GraphBlackboard _blackboard;
        public void Init(GraphBlackboard blackboard)
        {
            _blackboard = blackboard;
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>();
            var type = typeof(GenericVariable<>);
            var typeList = GameToolKitConfig.Instance.TypeGroup["BaseValue"];
            tree.Add(new SearchTreeGroupEntry(new GUIContent("BlackboardVariable")));
            foreach(var parameter in typeList)
                tree.Add(new SearchTreeEntry(new GUIContent("    " + parameter.Name)) 
                { 
                    level = 1, 
                    userData = type.MakeGenericType(parameter)
                });
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var type = SearchTreeEntry.userData as Type;
            var variable = Activator.CreateInstance(type, true) as BlackboardVariable;
            string name = type.GenericTypeArguments[0].Name;
            int i = 1;
            while (_blackboard.GUIDManager.ContainName(name))
                name = $"{type.GenericTypeArguments[0].Name}({i})";
            _blackboard.AddVariable(name, variable, Domain.Local);
            return true;
        }
    }
}
