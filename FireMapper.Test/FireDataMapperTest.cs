using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace FireMapper.Test
{
    [Collection("FireStoreMapperFixture collection")]
    public class FireDataMapperTest
    {
        readonly ITestOutputHelper output;
        private readonly FireStoreMapperFixture fix;
        private readonly IDataMapper colonosDb;
        private readonly IDataMapper monitoresDb;
        private readonly IDataMapper pessoaDb;
        private readonly IDataMapper grupoDb;
        private readonly IDataMapper campoDb;

        public FireDataMapperTest(ITestOutputHelper output, FireStoreMapperFixture fix)
        {
            this.output = output;
            this.fix = fix;
            this.monitoresDb = fix.dataMapperM;
            this.colonosDb = fix.dataMapperCo;
            this.campoDb = fix.dataMapperCa;
            this.pessoaDb = fix.dataMapperP;
            this.grupoDb = fix.dataMapperG;
        }

        [Fact]
        public void GetAll()
        {
            int count = 0;
            foreach (var item in colonosDb.GetAll())
            {
                output.WriteLine(item.ToString());
                count++;
            }
            Assert.Equal(4, count);
        }

        [Fact]
        public void GetById()
        {
            Colono colono = (Colono)colonosDb.GetById(4);
            Assert.Equal("Ines Fontes", colono.nome);
        }

        [Fact]
        public void UpdateColono()
        {
            Pessoa p1 = new Pessoa(111, "Tiago Ribeiro", "Porto", 929938476, "ribeiro@gmail.com");
            Grupo g1 = new Grupo("iniciados");
            Campo campo1 = new Campo(123, "Campo Ferias 1", "Rua da Liberdade", "Lisboa", "2341-123", "www.CF1.pt", "[90° N, 90° W]");

            colonosDb.Update(new Colono(1, "Ana Maria", "12-09-2003", 929992836, 19987, 192456, p1, g1, campo1));
            Colono colono = (Colono)colonosDb.GetById(1);
            Assert.Equal("Ana Maria", colono.nome);
        }

        [Fact]
        public void GetIvalidColono() {
            Colono colono = (Colono)colonosDb.GetById(500);
            Assert.Null(colono);
        }

        [Fact]
        public void AddGetAndDeleteAndGetAgain()
        {
            ///
            /// Arrange and Insert new Colono
            /// 
            
            Pessoa p1 = new Pessoa(111, "Tiago Ribeiro", "Porto", 929938476, "ribeiro@gmail.com");
            Grupo g1 = new Grupo("iniciados");
            Campo campo1 = new Campo(123, "Campo Ferias 1", "Rua da Liberdade", "Lisboa", "2341-123", "www.CF1.pt", "[90° N, 90° W]");
            Colono colono = new Colono(5, "Paulo Alves", "05-10-2000", 91882255, 99833, 224433, p1, g1, campo1);
            colonosDb.Add(colono);

            Colono actual = (Colono)colonosDb.GetById(colono.id);
            Assert.Equal(colono.id, actual.id);
            
            Assert.Equal(colono.nome, actual.nome);
            Assert.Equal(colono.dtnasc, actual.dtnasc);
            Assert.Equal(colono.contacto, actual.contacto);
            Assert.Equal(colono.ccidadao, actual.ccidadao);
            Assert.Equal(colono.cutente, actual.cutente);
            Assert.Equal(colono.eeducacao.id, actual.eeducacao.id);
            Assert.Equal(colono.grupo.nome, actual.grupo.nome);
            Assert.Equal(colono.campoid.id, actual.campoid.id);
            
            /// 
            /// Remove Colono
            /// 
            
            colonosDb.Delete(colono.id);
            Assert.Null(colonosDb.GetById(colono.id));
        }

        [Fact]
        public void AddAndGetMonitor()
        {
            Pessoa p1 = new Pessoa(111, "Tiago Ribeiro", "Porto", 929938476, "ribeiro@gmail.com");
            Grupo g1 = new Grupo("iniciados");
            Campo campo1 = new Campo(123, "Campo Ferias 1", "Rua da Liberdade", "Lisboa", "2341-123", "www.CF1.pt", "[90° N, 90° W]");
            Monitor monitor = new Monitor(p1,campo1,g1);
            Monitor actualm = (Monitor)monitoresDb.GetById(p1.id);
            Assert.Equal(actualm.id.id, p1.id);  

        }
        static string ToString(Dictionary<string, object> source)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append('{');
            foreach (var pair in source)
            {
                buffer.Append($"{pair.Key} : {pair.Value},");
            }
            if (buffer.Length > 1) buffer.Length--; // Remove extra comma
            buffer.Append('}');
            return buffer.ToString();
        }
    }
}
