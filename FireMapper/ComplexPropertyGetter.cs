using System.Reflection;
using System;
using System.Collections.Generic;

namespace FireMapper
{


    public class ComplexPropertyGetter : AbstractGetter
    {

        PropertyInfo property;
        IDataMapper dbc;
        
        public ComplexPropertyGetter(PropertyInfo property, IDataMapper dbc) : base(property.Name, dbc)
        {
            this.property = property;
            this.dbc=dbc;

        }



        public override object GetValue(object obj)
        {

            return dbc.GetById(obj);
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

        

    }
}