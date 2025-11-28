using ITPortal.Extension.System.Collections.Generic;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace ITPortal.Extension.System
{
    /// <summary>
    /// 动态可扩展对象，支持在运行时添加、修改和访问属性
    /// </summary>
    public class ExpandableObject : DynamicObject
    {
        // 存储动态属性的字典
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        // 存储属性变更事件的委托
        public event Action<string, object, object> PropertyChanged;

        /// <summary>
        /// 获取所有属性名称
        /// </summary>
        public IEnumerable<string> PropertyNames => _properties.Keys;

        /// <summary>
        /// 获取属性数量
        /// </summary>
        public int PropertyCount => _properties.Count;

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _properties.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// 尝试设置属性值
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var oldValue = GetValue(binder.Name);
            _properties[binder.Name] = value;

            // 触发属性变更事件
            PropertyChanged?.Invoke(binder.Name, oldValue, value);
            return true;
        }

        /// <summary>
        /// 尝试调用方法（支持简单的方法调用）
        /// </summary>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            // 检查是否有对应的方法属性
            if (_properties.TryGetValue(binder.Name, out var method) && method is Delegate del)
            {
                result = del.DynamicInvoke(args);
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        public object GetValue(string propertyName)
        {
            _properties.TryGetValue(propertyName, out var value);
            return value;
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        public void SetValue(string propertyName, object value)
        {
            var oldValue = GetValue(propertyName);
            _properties[propertyName] = value;

            // 触发属性变更事件
            PropertyChanged?.Invoke(propertyName, oldValue, value);
        }

        /// <summary>
        /// 检查是否包含指定属性
        /// </summary>
        public bool HasProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// 移除属性
        /// </summary>
        public bool RemoveProperty(string propertyName)
        {
            return _properties.Remove(propertyName);
        }

        /// <summary>
        /// 清除所有属性
        /// </summary>
        public void ClearProperties()
        {
            _properties.Clear();
        }

        /// <summary>
        /// 复制属性到另一个可扩展对象
        /// </summary>
        public void CopyTo(ExpandableObject target, bool overwrite = true)
        {
            foreach (var prop in _properties)
            {
                if (!target.HasProperty(prop.Key) || overwrite)
                {
                    target.SetValue(prop.Key, prop.Value);
                }
            }
        }

        /// <summary>
        /// 合并另一个对象的属性
        /// </summary>
        public void Merge(ExpandableObject source, bool overwrite = true)
        {
            source.CopyTo(this, overwrite);
        }

        /// <summary>
        /// 转换为字典
        /// </summary>
        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>(_properties);
        }

        /// <summary>
        /// 从字典加载属性
        /// </summary>
        public void LoadFromDictionary(IDictionary<string, object> dictionary, bool clearExisting = false)
        {
            if (clearExisting)
            {
                ClearProperties();
            }

            foreach (var item in dictionary)
            {
                SetValue(item.Key, item.Value);
            }
        }
    }
}
