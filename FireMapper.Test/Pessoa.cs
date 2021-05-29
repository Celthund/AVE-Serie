using System;

namespace FireMapper{
    [FireCollection("Pessoas")]
    public record Pessoa([property:FireKey] int id, string nome, string endereco, int contacto, string email)  {

    }
}