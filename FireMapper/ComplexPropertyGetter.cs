using System.Reflection;
using System;
using System.Collections.Generic;

namespace FireMapper
{


    public class ComplexPropertyGetter : AbstractGetter
    {

        PropertyInfo property;
       
        
        public ComplexPropertyGetter(PropertyInfo property, IDataMapper db) : base(property.Name, db)
        {
            this.property = property;

        }



        public override object GetValue(object obj)
        {

            return db.GetById(obj);
        }


        public override Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj)
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

        public override object GetDefaultValue()
        {
            return null;
        }

        public override object GetKeyValue(object obj)
        {
            foreach (IGetter p in db.GetPropertiesList())
            {
                //Checks if the property is a key
                if (p.IsDefined())
                {
                    //Adds property name and value to the dictionary
                    return p.GetValue(property.GetValue(obj));
                }
            }
            return null;
        }
    }
}