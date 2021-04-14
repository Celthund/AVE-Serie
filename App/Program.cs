using System;
using FireMapper;
using System.Reflection;
using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            string CredentialsPath = "D:/2Ano/2 semestre/AVE/FireMapper/App/ave-trab1-g02-firebase-adminsdk-3f705-1ab19a5fb2.json";
            string ProjectId = "ave-trab1-g02";
            string Collection = "students";
            IDataMapper dataMapper = new FireDataMapper(typeof(Student), ProjectId, Collection, CredentialsPath);
            //Student s1 = new Student(2534,"Serafim", 2);
            Student s2 = (Student)dataMapper.GetById(12345);
            //dataMapper.Add(s1);
            //dataMapper.Delete(2534);
            //dataMapper.Update(s1);
            foreach (Student item in dataMapper.GetAll())
            {
                Console.WriteLine(item);
            }
        }
    }
}