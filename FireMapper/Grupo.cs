using System;

namespace FireMapper{
    [FireCollection("Grupos")]
    public record Grupo( [property:FireKey] string nome)  {

    }
}