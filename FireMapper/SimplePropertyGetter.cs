using System.Reflection;
using System;
using System.Collections.Generic;
namespace FireMapper
{


    public class SimplePropertyGetter : AbstractGetter
    {

        PropertyInfo property;



        public SimplePropertyGetter(PropertyInfo property) : base(property.Name)
        {
            this.property = property;
        }
        


        public override object GetValue(object obj)
        {
            if(obj.GetType()== property.PropertyType)
                return obj;

            return property.GetValue(obj, null);
        }

        public override Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj)
        {
            dictionary.Add(GetName(), GetValue(obj));
            return dictionary;
        }

        


    }
}