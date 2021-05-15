using System;
using System.Collections.Generic;

namespace App
{


    public class SimpleStudentNameGetter : AbstractGetter
    {
        public SimpleStudentNameGetter() : base("name")
        {
        }



        public override Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj)
        {
            Student st = (Student)obj;
            dictionary.Add("name",st.name );
            return dictionary;
        }

        public override object GetValue(object target)
        {
            Student st = (Student)target;
            return st.name;
        }

        public override bool IsDefined()
        {
            return true;
        }

        public override Type PropertyType()
        {
            return typeof(string);
        }
    }
}