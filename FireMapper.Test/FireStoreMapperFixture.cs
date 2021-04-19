using System;
using System.Collections.Generic;
using System.IO;
using FireSource;

namespace FireMapper.Test
{
    /// <summary>
    /// A single test context shared among all the tests.
    /// Test classes should implement IClassFixture<FireStoreFixture> and
    /// provide a constructor to inject a Fixture object.
    /// </summary>
    public class FireStoreMapperFixture : IDisposable
    {
        const string CredentialsPath = "Resources/ave-trab1-g02-firebase-adminsdk-3f705-1ab19a5fb2.json";
        const string ProjectId = "ave-trab1-g02";
        const string CollectionCa = "Campos";
        const string CollectionP = "Pessoas";
        const string CollectionG = "Grupos";
        const string CollectionCo = "Colonos";
        const string CollectionM = "Monitores";
        const string SOURCE_ITEMS = "Resources/isel-AVE-2021.txt";

        private static Type type = typeof(WeakDataSource);
        public readonly IDataMapper dataMapperCa = new FireDataMapper(typeof(Campo), ProjectId, CollectionCa, CredentialsPath, type);
        public readonly IDataMapper dataMapperP = new FireDataMapper(typeof(Pessoa), ProjectId, CollectionP, CredentialsPath, type);
        public readonly IDataMapper dataMapperG = new FireDataMapper(typeof(Grupo), ProjectId, CollectionG, CredentialsPath, type);
        public readonly IDataMapper dataMapperCo = new FireDataMapper(typeof(Colono), ProjectId, CollectionCo, CredentialsPath, type);
        public readonly IDataMapper dataMapperM = new FireDataMapper(typeof(Monitor), ProjectId, CollectionM, CredentialsPath, type);

        public void Dispose()
        {
            ///
            /// ... clean up test data from the database ...
            /// 
            ClearColonos(dataMapperCo);
            ClearCampos(dataMapperCa);
            ClearGrupos(dataMapperG);
            ClearMonitores(dataMapperM);
            ClearPessoas(dataMapperP);
        }
        

        private static void ClearColonos(IDataMapper source)
        {

            foreach (Colono colono in source.GetAll())
            {
                source.Delete(colono.id);
            }
        }
        private static void ClearCampos(IDataMapper source)
        {

            foreach (Campo campo in source.GetAll())
            {
                source.Delete(campo.id);
            }
        }
        private static void ClearPessoas(IDataMapper source)
        {

            foreach (Pessoa pessoa in source.GetAll())
            {
                source.Delete(pessoa.id);
            }
        }
        private static void ClearGrupos(IDataMapper source)
        {

            foreach (Grupo grupo in source.GetAll())
            {
                source.Delete(grupo.nome);
            }
        }
        private static void ClearMonitores(IDataMapper source)
        {

            foreach (Monitor monitor in source.GetAll())
            {
                source.Delete(monitor.id.id);
            }
        }


        public FireStoreMapperFixture()
        {
            AddToFireStore();
        }
        void AddToFireStore()
        {
            Campo campo1 = new Campo(123, "Campo Ferias 1", "Rua da Liberdade", "Lisboa", "2341-123", "www.CF1.pt", "[90° N, 90° W]");
            Campo campo2 = new Campo(456, "Campo Ferias 2", "Avenida Central", "Porto", "4564-456", "www.CF2.pt", "[45° N, 12° W]");
            dataMapperCa.Add(campo1);
            dataMapperCa.Add(campo2);

            Grupo g1 = new Grupo("iniciados");
            Grupo g2 = new Grupo("juvenis");
            Grupo g3 = new Grupo("seniores");
            dataMapperG.Add(g1);
            dataMapperG.Add(g2);
            dataMapperG.Add(g3);

            Pessoa p1 = new Pessoa(111, "Tiago Ribeiro", "Porto", 929938476, "ribeiro@gmail.com");
            Pessoa p2 = new Pessoa(222, "Tiago Silva", "Lisboa", 93766876, "silva@gmail.com");
            Pessoa p3 = new Pessoa(333, "Diogo Fernandes", "Almada", 96999585, "fernandes@gmail.com");
            dataMapperP.Add(p1);
            dataMapperP.Add(p2);
            dataMapperP.Add(p3);

            Colono c1 = new Colono(1, "Manuel Costa", "12-09-2003", 929992836, 19987, 192456, p1, g1, campo1);
            Colono c2 = new Colono(2, "João Silva", "01-04-2012", 91827453, 18722, 1233231, p1, g1, campo2);
            Colono c3 = new Colono(3, "Joana Martins", "17-07-2004", 91998635, 22928, 365363, p2, g3, campo1);
            Colono c4 = new Colono(4, "Ines Fontes", "24-02-2008", 91112223, 44884, 388636, p3, g2, campo2);
            dataMapperCo.Add(c1);
            dataMapperCo.Add(c2);
            dataMapperCo.Add(c3);
            dataMapperCo.Add(c4);

            Monitor m1 = new Monitor(p1, campo1, g1);
            dataMapperM.Add(m1);

        }


        static IEnumerable<string> Lines(string path)
        {
            string line;
            IList<string> res = new List<string>();
            using (StreamReader file = new StreamReader(path)) // <=> try-with resources do Java >= 7
            {
                while ((line = file.ReadLine()) != null)
                {
                    res.Add(line);
                }
            }
            return res;
        }
    }
}