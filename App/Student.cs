
namespace FireMapper{

    [FireCollection("Students")]
    public record Student(
    [property:FireKey] long number,
    string name,
    [property:FireIgnore] int Classroom)  
{}
        
     }