using System;

namespace FireMapper.Test{
    [FireCollection("Grupos")]
    public record Grupo( [property:FireKey] string nome)  {

    }
}