using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Cloud.Firestore;

namespace FireMapper.Test
{
    /// <summary>
    /// A single test context shared among all the tests.
    /// Test classes should implement IClassFixture<FireStoreFixture> and
    /// provide a constructor to inject a Fixture object.
    /// </summary>
    public class FireMapperFixture : IDisposable
    {
        const string CredentialsPath = "C:\\Faculdade\\4 Semestre\\AVE\\trabalho1\\FireMapper.Test\\Resources\\fire-students-e339a-firebase-adminsdk-2y9k9-adae1534d9.json";
        const string ProjectId = "fire-students-e339a";
        const string SOURCE_ITEMS="Resources/isel-AVE-2021.txt";
        
        public readonly IDataMapper studentsDb = new FireDataMapper(
            typeof(Student),
            ProjectId,
            "student",
            CredentialsPath
            );
        public readonly IDataMapper classroomsDb = new FireDataMapper(
            typeof(ClassroomInfo),
            ProjectId,
            "classroom",
            CredentialsPath
            );

        public void Dispose()
        {
            ///
            /// ... clean up test data from the database ...
            /// 
            Clear(studentsDb, "number");
            Clear(classroomsDb, "token");
        }
        private static void Clear(IDataMapper source, string key)
        {
            IEnumerable docs = source.GetAll();
            foreach (var pairs in docs)
            {
                source.Delete(pairs);
            }
        }

        public FireMapperFixture()
        {
            CreateClassrooms();
            AddToFirestoreFrom(SOURCE_ITEMS);
        }
        void CreateClassrooms()
        {
            InsertClassroomFor("TLI41D", "Miguel Gamboa");
            InsertClassroomFor("TLI42D", "Luís Falcão");
            InsertClassroomFor("TLI41N", "Miguel Gamboa");
            InsertClassroomFor("TLI4NXST", "NA");
            InsertClassroomFor("TLI4DXST", "NA");
        }
        void InsertClassroomFor(string token, string teacher)
        {
            classroomsDb.Add(new ClassroomInfo(token, teacher));
        }

        void AddToFirestoreFrom(string path)
        {
            foreach (string line in Lines(path))
            {
                Student st = Student.Parse(line);
                studentsDb.Add(st);
            }
        }

        static IEnumerable<string> Lines(string path)
        {
            string line;
            IList<string> res = new List<string>();
            using (StreamReader file = new StreamReader(path)) // <=> try-with resources do Java >= 7
            {
                while ((line = file.ReadLine()) != null)
                {
                    res.Add(line);
                }
            }
            return res;
        }
    }
}