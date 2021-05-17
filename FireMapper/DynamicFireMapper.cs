using System.Collections;
using System.Collections.Generic;
using FireSource;
using System.Reflection;


using System;
namespace FireMapper
{
    public class DynamicFireMapper : IDataMapper
    {
        Type domain;
        //Collection name
        string Collection;
        //IDatasource
        IDataSource dataSource;
        // Type that defines the dataSource to be used.
        Type dataSourceType;
        //Credentials .json path
        string CredentialsPath;
        //Project Id
        string ProjectId;
        //Stores the collection properties
        List<IGetter> properties;
        DynamicGetterBuilder getterBuilder;
        /*
        Constructor
        */
        public DynamicFireMapper(Type domain, string ProjectId, string Collection, string CredentialsPath, Type dataSourceType)
        {
            this.dataSourceType = dataSourceType;
            this.domain = domain;
            this.Collection = Collection;
            this.CredentialsPath = CredentialsPath;
            this.ProjectId = ProjectId;
            getterBuilder = new DynamicGetterBuilder(domain);
            setProperties();
        }
        void setProperties()
        {
            List<IGetter> properties = new List<IGetter>();
            
            //Iterates over the object properties
            foreach (PropertyInfo p in domain.GetProperties())
            {
                bool isKey=false;   
                //Checks that the property is not ignored
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    if (p.IsDefined(typeof(FireKey)))
                    {
                        setDataSource(p);
                        isKey=true;
                        
                    }
                    IGetter getter;

                    //Checks if the property is a collection
                    if (p.PropertyType.IsDefined(typeof(FireCollection)))
                    {
                        IDataMapper db = new DynamicFireMapper(
                            p.PropertyType,
                            ProjectId,
                            ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection, // Value of collection propriety of Record.
                            CredentialsPath, dataSourceType);
                        //New FireDataMapper with the property collection                       
                        //string OtherCollection = ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection;
                        Type getterType = getterBuilder.GenerateComplexGetter(p);
                        getter = (IGetter)Activator.CreateInstance(getterType, new object[]{p.Name, db});
                    }
                    else
                    {
                        Type getterType = getterBuilder.GenerateSimpleGetter(p,isKey);
                        getter = (IGetter)Activator.CreateInstance(getterType);
                        
                    }
                    //Adds property to properties list
                    properties.Add(getter);
                }
            }
            //Defines properties list
            this.properties = properties;
        }
        void setDataSource(PropertyInfo p)
        {
            //Iterates over the object properties
            object[] args = { ProjectId, Collection, p.Name, CredentialsPath };
            dataSource = (IDataSource)Activator.CreateInstance(dataSourceType, args);
        }
        public void Add(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //Iterates over the properties list
            foreach (IGetter p in properties)
            {

                //dictionary = p.FillDictionary(dictionary, obj);
                dictionary.Add(p.GetName(), p.GetKeyValue(obj));

            }
            //Updates DB with new value
            dataSource.Add(dictionary);

        }

        public void Delete(object keyValue)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetAll()
        {
            //Objects list
            List<object> objs = new List<object>();
            object obj;
            foreach (var item in dataSource.GetAll())
            {
                obj = CreateObject(item);
                if (obj is not null)
                    objs.Add(obj);
            }
            return objs;
        }

        object IDataMapper.GetById(object keyValue)
        {
            Dictionary<string, object> dictionary;
            dictionary = dataSource.GetById(keyValue);
            return CreateObject(dictionary);
        }

        public List<IGetter> GetPropertiesList()
        {
            return properties;
        }

        public void Update(object obj)
        {
            throw new System.NotImplementedException();
        }
        object CreateObject(Dictionary<string, object> dictionary)
        {
            if (dictionary is null) return null;
            //Array with object constructor arguments
            object[] newObjProperties = new object[properties.Count];
            int i = 0;

            //Iterates over the properties list
            foreach (IGetter p in properties)
            {
                if (dictionary.ContainsKey(p.GetName()))
                {
                    object o = p.GetValue(dictionary[p.GetName()]);
                    newObjProperties[i]=o;
                }
                else
                {
                    //Adds a new instance or null to the constructor argument array in case a property is not present in properties list (with FireIgnore Attr) 
                    newObjProperties[i] = p.GetDefaultValue();
                }
                i++;
            }
            //New object instance
            object newObj = Activator.CreateInstance(domain, newObjProperties);
            return newObj;
        }
    }
}