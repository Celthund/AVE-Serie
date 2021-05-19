using System.Reflection;
using System;
using System.Collections.Generic;
namespace FireMapper
{
    public class SimplePropertyGetter : AbstractGetter
    {

        

        PropertyInfo property;

        bool IsValueType;

        public SimplePropertyGetter(PropertyInfo property, bool isKey) : base(property.Name, isKey)
        {
            this.property = property;
            IsValueType = property.PropertyType.IsValueType;
        }

        public override object GetValue(object obj)
        {
            if (obj.GetType() == property.PropertyType)
                return obj;

            return property.GetValue(obj, null);
        }

        public override object GetDefaultValue()
        {
            return IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
        }

        public override object GetKeyValue(object obj)
        {
            return GetValue(obj);
        }
    }
}