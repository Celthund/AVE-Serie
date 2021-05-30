using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FireMapper;
using FireSource;

namespace FireMapper{
    public abstract class AbstractDataMapper : IDataMapper
    {
        //Object Type
        protected Type domain;
        //Collection name
        protected string Collection;
        //IDatasource
        protected IDataSource dataSource;
        // Type that defines the dataSource to be used.
        protected Type dataSourceType;
        //Credentials .json path
        protected string CredentialsPath;
        //Project Id
        protected string ProjectId;
        //Store object FireKey
        protected IGetter FireKey;
        //Stores the property getters
        protected List<IGetter> propertyGetters;
        
        protected AbstractDataMapper(Type domain, string projectId, string collection, string credentialsPath, Type dataSourceType)
        {
            this.domain = domain;
            this.ProjectId = projectId;
            this.Collection = collection;
            this.CredentialsPath = credentialsPath;
            this.dataSourceType = dataSourceType;
            setProperties();
        }

        //Abstract method setProperties to be implemented on descending classes
        protected abstract void setProperties();
        
        /*
        Add new object to DB
        */
        public void Add(object obj)
        {
             Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //Iterates over the properties list
            foreach (IGetter p in propertyGetters)
            {
                dictionary.Add(p.GetName(), p.GetKeyValue(obj));
            }
            //Updates DB with new value
            dataSource.Add(dictionary);
        }

        /*
        Deletes the object related to the given key 
        */
        public void Delete(object keyValue)
        {
            dataSource.Delete(keyValue);
        }

        /*
        Gets all database data in object form
        */
        public IEnumerable GetAll()
        {
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

        /*
        Returns the object from the given key
        */
        public object GetById(object keyValue)
        {
            Dictionary<string, object> dictionary;
            dictionary = dataSource.GetById(keyValue);
            return CreateObject(dictionary);
        }
        /*
        Get key getter
        */
        public IGetter GetFireKey()
        {
            return FireKey;
        }
        /*
        Update object in db
        */
        public void Update(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //Iterates over the properties list
            foreach (IGetter p in propertyGetters)
            {
                dictionary.Add(p.GetName(),p.GetKeyValue(obj));
            }
            //Updates DB with new value
            dataSource.Update(dictionary);
        }

        /*
        Defines dataSource
        */
        protected void setDataSource(PropertyInfo p) 
        {
            object[] args = { ProjectId, Collection, p.Name, CredentialsPath };
            dataSource = (IDataSource)Activator.CreateInstance(dataSourceType, args);
        }

        /*
        Creates and returns a new object from a given dictionary
        */
        object CreateObject(Dictionary<string, object> dictionary) 
        {
            if (dictionary is null) return null;
            //Array with object constructor arguments
            object[] newObjProperties = new object[propertyGetters.Count];
            int i = 0;

            //Iterates over the properties list
            foreach (IGetter p in propertyGetters)
            {
                if (dictionary.ContainsKey(p.GetName()))
                {
                    object o = p.GetValue(dictionary[p.GetName()]);
                    newObjProperties[i] = o ;
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