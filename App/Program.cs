using FireMapper;
using FireSource;
using System;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("SAVE_MODULES", "true");
            Console.WriteLine("HELLO");
            string CredentialsPath = "Resources/ave-campo-ferias-firebase-adminsdk-m26up-d31998ef53.json";
            string ProjectId = "ave-campo-ferias";
            string Collection_Students = "Students";
            //string Collection_Classrooms = "Classrooms";
            IDataMapper dataMapper = new FireDataMapper(typeof(Student), ProjectId, Collection_Students, CredentialsPath, typeof(FireDataSource));
            //IDataMapper dynamicDataMapper = new DynamicFireMapper(typeof(Student), ProjectId, Collection_Students, CredentialsPath, typeof(FireDataSource));
            IDataMapper dynamicDataMapper = new DynamicFireMapper(typeof(Student), ProjectId, Collection_Students, CredentialsPath, typeof(FireDataSource));

            //IDataMapper dataMapper2 = new FireDataMapper(typeof(ClassroomInfo), ProjectId, Collection_Classrooms, CredentialsPath,typeof(FireDataSource));
            //ClassroomInfo c1 = new ClassroomInfo("LI42N", "Jorge Coise");
            Student s1 = new Student(1, "Joana", null);
            //dynamicDataMapper.Add(s1);

            Student s2 = (Student)dataMapper.GetById(1);


            //dynamicDataMapper.Add(s1);

            Student s3 = (Student)dynamicDataMapper.GetById(1);
            //Student s2 = (Student)dynamicDataMapper.GetById("12345");
            //Student s3 = (Student)dataMapper.GetById("12345");
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