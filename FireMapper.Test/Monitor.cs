using System;

namespace FireMapper.Test{
    [FireCollection("Monitores")]
    public record Monitor( [property:FireKey] Pessoa id, [property:FireKey] Campo campoid, [property:FireKey] Grupo gruponome)  {

    }
}