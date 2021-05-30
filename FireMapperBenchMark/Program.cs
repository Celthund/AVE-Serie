using System;
using FireMapper;
using FireSource;

namespace FireMapperBenchMark
{
    class Program
    {
        static string CredentialsPath = "D:/2Ano/2 semestre/AVE/FireMapper/FireMapperBenchMark/Resources/ave-trab1-g02-firebase-adminsdk-3f705-1ab19a5fb2.json";
        static string ProjectId = "ave-trab1-g02";
        static string Collection_Campos = "Campos";
        static string Collection_Monitores = "Monitores";
        static string Collection_Colonos = "Colonos";
        static string Collection_Grupos = "Grupos";
        static string Collection_Pessoas = "Pessoas";
        static Pessoa pessoa;
        static Grupo grupo;
        static Campo campo;
        static Monitor monitor;
        static Colono colono1;
        static Colono colono2;
        //AbstractDataMapper reflect Colonos
        static readonly AbstractDataMapper FireMapperColonosDb;
        //AbstractDataMapper dynamic Colonos
        static readonly AbstractDataMapper DynamicFireMapperColonosDb;
        //AbstractDataMapper reflect Pessoas
        static readonly AbstractDataMapper FireMapperPessoasDb;
        //AbstractDataMapper reflect Campos
        static readonly AbstractDataMapper FireMapperCamposDb;
        //AbstractDataMapper reflect Grupos
        static readonly AbstractDataMapper FireMapperGruposDb;
        //AbstractDataMapper reflect Monitores
        static readonly AbstractDataMapper FireMapperMonitoresDb;
        static void Main(string[] args)
        {
            //Performance test Delete and Add methods
            NBench.Bench(BenchDeleteAndAddReflectColono);
            NBench.Bench(BenchDeleteAndAddDynamicColono);
            //Performance test Get by Id method
            NBench.Bench(BenchGetByIdReflectColono);
            NBench.Bench(BenchGetByIdDynamicColono);
            //Performance test Get All method
            NBench.Bench(BenchGetAllReflectColonos);
            NBench.Bench(BenchGetAllDynamicColonos);
            //Performance test Update method
            NBench.Bench(BenchUpdateReflectColono);
            NBench.Bench(BenchUpdateDynamicColono);
            
        }
        /*
        Instantiate AbstractDataMappers and prepare db
        */
        static Program()
        {
            FireMapperColonosDb = new FireDataMapper(typeof(Colono), ProjectId, Collection_Colonos, CredentialsPath, typeof(WeakDataSource));
            DynamicFireMapperColonosDb = new DynamicFireMapper(typeof(Colono), ProjectId, Collection_Colonos, CredentialsPath, typeof(WeakDataSource));
            FireMapperPessoasDb = new FireDataMapper(typeof(Pessoa), ProjectId, Collection_Pessoas, CredentialsPath, typeof(WeakDataSource));
            FireMapperCamposDb = new FireDataMapper(typeof(Campo), ProjectId, Collection_Campos, CredentialsPath, typeof(WeakDataSource));
            FireMapperGruposDb = new FireDataMapper(typeof(Grupo), ProjectId, Collection_Grupos, CredentialsPath, typeof(WeakDataSource));
            FireMapperMonitoresDb = new FireDataMapper(typeof(Monitor), ProjectId, Collection_Monitores, CredentialsPath, typeof(WeakDataSource));
            pessoa = new Pessoa(111, "Tiago Ribeiro", "Porto", 929938476, "ribeiro@gmail.com");
            grupo= new Grupo("iniciados");
            campo = new Campo(123, "Campo Ferias 1", "Rua da Liberdade", "Lisboa", "2341-123", "www.CF1.pt", "[90° N, 90° W]");
            colono1 = new Colono(222, "Serafim", "05-10-2004", 11223344, 12345, 12131415, pessoa, grupo, campo);
            colono2 = new Colono(222, "Serafim Morais", "09-07-2001", 55667788, 678910, 21222324, pessoa, grupo, campo);
            monitor = new Monitor(pessoa, campo, grupo);
            FireMapperPessoasDb.Add(pessoa);
            FireMapperCamposDb.Add(campo);
            FireMapperGruposDb.Add(grupo);
            FireMapperMonitoresDb.Add(monitor);
        }
        /*
        GetById reflect method
        */
        public static void BenchGetByIdReflectColono(){
            FireMapperColonosDb.GetById(colono1.id);
        }
        /*
        GetById dynamic method
        */
        public static void BenchGetByIdDynamicColono(){
            DynamicFireMapperColonosDb.GetById(colono1.id);
        }
        /*
        Update reflect method
        */
        public static void BenchUpdateReflectColono(){
            FireMapperColonosDb.Update(colono2);
        }
        /*
        Update dynamic method
        */
        public static void BenchUpdateDynamicColono(){
            DynamicFireMapperColonosDb.Update(colono2);
        }
        /*
        GetAll reflect method
        */
        public static void BenchGetAllReflectColonos(){
            FireMapperColonosDb.GetAll();
        }
        /*
        GetAll dynamic method
        */
        public static void BenchGetAllDynamicColonos(){
            DynamicFireMapperColonosDb.GetAll();
        }
        /*
        Delete and Add reflect methods
        */
        public static void BenchDeleteAndAddReflectColono(){
            FireMapperColonosDb.Delete(colono1.id);
            FireMapperColonosDb.Add(colono1);
        }
        /*
        Delete and Add dynamic methods
        */
        public static void BenchDeleteAndAddDynamicColono(){
            DynamicFireMapperColonosDb.Delete(colono1.id);
            DynamicFireMapperColonosDb.Add(colono1);
        }
    }
}