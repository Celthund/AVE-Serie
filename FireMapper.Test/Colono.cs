using System;

namespace FireMapper
{
    [FireCollection("Colonos")]
    public record Colono([property: FireKey] int id,
        string nome,
        string dtnasc,
        int contacto,
        int ccidadao,
        int cutente,
        Pessoa eeducacao,
        Grupo grupo,
        Campo campoid)
    {

    }
}