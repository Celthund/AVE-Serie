using System;

namespace FireMapper{
    [FireCollection("Monitores")]
    public record Monitor( [property:FireKey] Pessoa id, Campo campoid, Grupo gruponome)  {

    }
}