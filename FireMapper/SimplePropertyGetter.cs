using System.Reflection;
using System;
namespace FireMapper
{
    public class SimplePropertyGetter : AbstractGetter
    {
        PropertyInfo property;

        bool IsValueType;

        /*
        Constructor
        */
        public SimplePropertyGetter(PropertyInfo property) : base(property.Name)
        {
            this.property = property;
            IsValueType = property.PropertyType.IsValueType;  //Stores whether the property type is ValueType or not
        }
        /*
        Get value
        */
        public override object GetValue(object obj)
        {
            if (obj == null) return obj;
            //Checks obj type
            if (obj.GetType() == property.PropertyType || obj.GetType().IsValueType)
                return Convert.ChangeType(obj,property.PropertyType); //Changes obj type to property type

            return property.GetValue(obj, null); 
        }
        /*
        Get default value
        */
        public override object GetDefaultValue()
        {
            return IsValueType ? Activator.CreateInstance(property.PropertyType) : null;
        }
        /*
        Get Key Value
        */
        public override object GetKeyValue(object obj)
        {
            return GetValue(obj);
        }
    }
}