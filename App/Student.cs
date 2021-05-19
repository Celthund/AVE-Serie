namespace App
{
    [FireCollection("Students")]
    public record Student( [property:FireKey] int number, string name, ClassroomInfo classroom, StructTeste Teste)  {}
}