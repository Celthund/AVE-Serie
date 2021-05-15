using System.Reflection;
using System;
using System.Collections.Generic;
namespace FireMapper
{


    public class SimplePropertyGetter : IGetter
    {

        PropertyInfo property;



        public SimplePropertyGetter(PropertyInfo property)
        {
            this.property = property;
        }
        public string GetName()
        {
            return property.Name;
        }


        public object GetValue(object obj)
        {
            //if(obj.GetType()== property.PropertyType)
            //    return obj;

            return property.GetValue(obj, null);
        }

        public Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj)
        {
            dictionary.Add(GetName(), GetValue(obj));
            return dictionary;
        }

        public bool IsDefined()
        {
            return property.IsDefined(typeof(FireKey));
        }

        public Type PropertyType()
        {
            return property.PropertyType;
        }
    }
}