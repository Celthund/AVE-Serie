using System;
using System.Collections.Generic;
using FireMapper;
using FireSource;
namespace App
{


    public class ComplexClassroomGetter : AbstractGetter
    {
        public ComplexClassroomGetter() : base("classroom", new DynamicFireMapper(typeof(ClassroomInfo),
                                "ave-trab1-g02",
                                "Classrooms",
                                "D:/2Ano/2 semestre/AVE/Trab1/FireMapper/App/Resources/ave-trab1-g02-firebase-adminsdk-3f705-1ab19a5fb2.json"
                                ,typeof(FireDataSource)))
        {

        }

        public override Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj)
        {
            ClassroomInfo c = (ClassroomInfo)obj;
            foreach (IGetter p in db.GetPropertiesList())
            {
                //Checks if the property is a key
                if (p.IsDefined())
                {
                    //Adds property name and value to the dictionary
                    dictionary.Add(name, p.GetValue(c.token));
                }
            }
            return dictionary;
        }

        public override object GetValue(object target)
        {
            return db.GetById(target);
        }

       

    }
}