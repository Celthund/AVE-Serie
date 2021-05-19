using System;
using System.Collections.Generic;

namespace App
{


    public class SimpleStudentNameGetter : AbstractGetter
    {
        public SimpleStudentNameGetter() : base("name", false)
        {
        }

        public override object GetDefaultValue()
        {
            throw new NotImplementedException();
        }

        public override object GetKeyValue(object obj)
        {
            throw new NotImplementedException();
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