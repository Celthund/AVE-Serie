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


        public override object GetDefaultValue()
        {
            return null;
        }

        public override object GetKeyValue(object obj)
        {
            return db.GetFireKey().GetValue(property.GetValue(obj));
                    //Adds property name and value to the dictionary
        }
    }
}