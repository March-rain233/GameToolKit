using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameToolKit
{
    /// <summary>
    /// 定义泛型类的模板参数类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GenericSelectorAttribute : Attribute
    {
        /// <summary>
        /// 类型组名
        /// </summary>
        private string _typeGroup;

        /// <summary>
        /// 类型列表
        /// </summary>
        private Type[] _typeList;

        /// <param name="typeGroup">类型组名</param>
        public GenericSelectorAttribute(string typeGroup)
        {
            _typeGroup = typeGroup;
        }

        public GenericSelectorAttribute(Type[] types)
        {
            _typeList = types;
        }

        public List<Type> GetTypes()
        {
            if(string.IsNullOrEmpty(_typeGroup)) return _typeList.ToList();
            return GameToolKitConfig.Instance.TypeGroup[_typeGroup];
        }
    }
}
