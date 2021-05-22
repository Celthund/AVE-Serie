namespace App
{
    [FireCollection("Students")]
    public record Student( [property:FireKey] string number, string name, ClassroomInfo classroom)  {}
}