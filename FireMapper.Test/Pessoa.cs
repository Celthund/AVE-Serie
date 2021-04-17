using System;

namespace FireMapper.Test{
    [FireCollection("Pessoas")]
    public record Pessoa( [property:FireKey] int id, string nome, string endereco, int contacto, string email)  {

    }
}