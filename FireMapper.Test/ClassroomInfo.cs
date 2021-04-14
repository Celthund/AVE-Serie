
namespace FireMapper.Test{

[FireCollection("Classrooms")]
public record ClassroomInfo([property:FireKey] string token, string teacher) {}
        
}