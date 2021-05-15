using System;
using FireMapper;
using FireSource;
using System.Reflection;
using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            string CredentialsPath = "D:/2Ano/2 semestre/AVE/Trab1/FireMapper/App/Resources/ave-trab1-g02-firebase-adminsdk-3f705-1ab19a5fb2.json";
            string ProjectId = "ave-trab1-g02";
            string Collection_Students = "Students";
            //string Collection_Classrooms = "Classrooms";
            //IDataMapper dataMapper = new FireDataMapper(typeof(Student), ProjectId, Collection_Students, CredentialsPath,typeof(FireDataSource));
            IDataMapper dynamicDataMapper = new DynamicFireMapper(typeof(Student), ProjectId, Collection_Students, CredentialsPath,typeof(FireDataSource));
            //IDataMapper dataMapper2 = new FireDataMapper(typeof(ClassroomInfo), ProjectId, Collection_Classrooms, CredentialsPath,typeof(FireDataSource));
            //ClassroomInfo c1 = new ClassroomInfo("LI42N", "Jorge Coise");
            //Student s1 = new Student("99876","Joana", c1);
            Student s2 = (Student)dynamicDataMapper.GetById("12345");
            //dynamicDataMapper.Add(s1);
            //dataMapper.Delete("12345");
            //dataMapper.Update(s1);
            // foreach (Student item in dataMapper.GetAll())
            // {
            //     Console.WriteLine(item);
            // }
        }
    }
}