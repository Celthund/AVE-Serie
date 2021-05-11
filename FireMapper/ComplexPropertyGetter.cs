using System.Reflection;
using System;
using System.Collections.Generic;

namespace FireMapper
{


    public class ComplexPropertyGetter : IGetter
    {

        PropertyInfo property;

        IDataMapper db;



        public ComplexPropertyGetter(PropertyInfo property, IDataMapper db)
        {
            this.property = property;
            this.db = db;

        }

        public string GetName()
        {
            return property.Name;
        }

        public object GetValue(object obj)
        {

            return db.GetById(obj);
        }

        public object ChangeType(object obj)
        {

            return Convert.ChangeType(obj, property.PropertyType);
        }

        public Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj)
        {
            foreach (IGetter p in db.GetPropertiesList())
            {
                //Checks if the property is a key
                if (p.IsDefined())
                {
                    //Adds property name and value to the dictionary
                    dictionary.Add(GetName(), p.GetValue(property.GetValue(obj)));
                }
            }
            return dictionary;
        }

        public bool IsDefined()
        {
            return property.IsDefined(typeof(FireCollection));
        }
        public Type PropertyType()
        {
            return property.PropertyType;
        }

    }
}