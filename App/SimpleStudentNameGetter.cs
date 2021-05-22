using System;
using System.Collections.Generic;

namespace App
{


    public class SimpleStudentNameGetter : AbstractGetter
    {
        public SimpleStudentNameGetter() : base("name")
        {
        }

        public override object GetDefaultValue()
        {
            throw new NotImplementedException();
        }

        public override object GetKeyValue(object obj)
        {
           return GetValue(obj);
        }

        public override object GetValue(object obj)
        {
            if(obj is Student){
                Student st = (Student)obj;
                return st.name;

            }
            else{
                return obj;
            }
            
        }

        

        
    }
}