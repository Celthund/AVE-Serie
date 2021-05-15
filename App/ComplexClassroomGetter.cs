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
                                ,typeof(FireDataSource)) )
        {

        }

        public override Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object target)
        {
            return db.GetById(target);
        }

        public override bool IsDefined()
        {
            throw new NotImplementedException();
        }

        public override Type PropertyType()
        {
            throw new NotImplementedException();
        }
    }
}