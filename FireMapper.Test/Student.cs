using System;

namespace FireMapper.Test{
    [FireCollection("Students")]
    public record Student( [property:FireKey] int number, string name, ClassroomInfo classroom)  {

        public static Student Parse(string input)
        {
            string[] words = input.Split(';');
            return new Student( Int32.Parse(words[0]), words[1], new ClassroomInfo(words[2],""));
        }
    }
}