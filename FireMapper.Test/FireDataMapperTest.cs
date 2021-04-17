using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FireMapper.Test
{
    [Collection("FireMapperFixture collection")]
    public class FireDataMapperTest
    {
        readonly ITestOutputHelper output;
        private readonly FireMapperFixture fix;
        private readonly IDataMapper studentsDb;
        private readonly IDataMapper classroomsDb;

        public FireDataMapperTest(ITestOutputHelper output, FireMapperFixture fix)
        {
            this.output = output;
            this.fix = fix;
            this.studentsDb = fix.studentsDb;
            this.classroomsDb = fix.classroomsDb;
        }
            
        [Fact]
        public void GetAll()
        {
            int count = 0;
            foreach(var dic in studentsDb.GetAll()) {
                Console.WriteLine(dic);
                count++;
            }
            Assert.Equal(9, count);
        }
        
        [Fact]
        public void GetById()
        {
            Student st = (Student)studentsDb.GetById(44999);
            Assert.Equal("Bartiskovley Navriska Bratsha Sverilev", st.name);
        }

        [Fact]
        public void UpdateStudent()
        {
            studentsDb.Update(new Student(55999,"Nuwanda Dead Poets Society",new ClassroomInfo("TLI41D","Joao Leandro")));
            Student st = (Student) studentsDb.GetById(55999);
            Assert.Equal("Nuwanda Dead Poets Society",st.name);
        }
        [Fact]
        public void AddGetAndDeleteAndGetAgain()
        {
            ///
            /// Arrange and Insert new Student
            /// 
            ClassroomInfo cl = new ClassroomInfo("TLI41D","Joao Leandro");
            Student st = new Student(823648, "Ze Manel", cl);
            studentsDb.Add(st);
            /// 
            /// Get newby Student
            /// 
            Student actual = (Student) studentsDb.GetById(st.number);
            Assert.Equal(st.name, actual.name );
            Assert.Equal(st.number, actual.number);
            Assert.Equal(st.classroom.token, actual.classroom.token);
            /// 
            /// Remove Student
            /// 
            studentsDb.Delete(st.number);
            Assert.Null(studentsDb.GetById(st.number));
        }
        /*
        static string ToString(Dictionary<string, object> source) {
            StringBuilder buffer = new StringBuilder();
            buffer.Append('{');
            foreach(var pair in source) {
                buffer.Append($"{pair.Key} : {pair.Value},");
            }
            if(buffer.Length > 1) buffer.Length--; // Remove extra comma
            buffer.Append('}');
            return buffer.ToString();
        }
        */
    }
}
