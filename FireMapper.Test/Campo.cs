using System;

namespace FireMapper.Test{
    [FireCollection("Campos")]
    public record Campo( [property:FireKey] int id, string nome, string endereco, string localidade, string codpostal, string enderecoweb, string coordenadas)  {

    }
}