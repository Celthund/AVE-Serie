using System;
using System.Collections;
using System.Collections.Generic;
using FireSource;
using System.Reflection;

namespace FireMapper
{
    public class FireDataMapper : IDataMapper
    {
        //Object Type
        Type objType;
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

        IGetter FireKey;
        

        /*
        Constructor
        */
        public FireDataMapper(Type objType, string ProjectId, string Collection, string CredentialsPath, Type dataSourceType)
        {
            this.dataSourceType = dataSourceType;
            this.objType = objType;
            this.Collection = Collection;
            this.CredentialsPath = CredentialsPath;
            this.ProjectId = ProjectId;
            setProperties();
        }

        /*
        Add new object to DB
        */
        void IDataMapper.Add(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //Iterates over the properties list
            foreach (IGetter p in properties)
            {
                
                //dictionary.Add(p.GetName(),p.GetValue(obj));
                dictionary.Add(p.GetName(), p.GetKeyValue(obj));

            }
            //Updates DB with new value
            dataSource.Add(dictionary);
        }

        /*
        Deletes the object related to the given key 
        */
        void IDataMapper.Delete(object keyValue)
        {
            dataSource.Delete(keyValue);
        }

        /*
        Gets all database data in object form
        */
        IEnumerable IDataMapper.GetAll()
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

        /*
        Returns the object from the given key
        */
        object IDataMapper.GetById(object keyValue)
        {
            Dictionary<string, object> dictionary;
            dictionary = dataSource.GetById(keyValue);
            return CreateObject(dictionary);
        }

        /*
        Updates a DB object
        */
        void IDataMapper.Update(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //Iterates over the properties list
            foreach (IGetter p in properties)
            {
                dictionary.Add(p.GetName(),p.GetValue(obj));
            }
            //Updates DB with new value
            dataSource.Update(dictionary);
        }
        /*
        Defines dataSource
        */
        void setDataSource(PropertyInfo p)
        {
            //Iterates over the object properties
            object[] args = { ProjectId, Collection, p.Name, CredentialsPath };
            dataSource = (IDataSource)Activator.CreateInstance(dataSourceType, args);
        }

        /*
        Defines and populates properties list
        */
        void setProperties()
        {
            List<IGetter> properties = new List<IGetter>();
            //Iterates over the object properties
            foreach (PropertyInfo p in objType.GetProperties())
            {

                //Checks that the property is not ignored
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    bool isKey=false;
                    if (p.IsDefined(typeof(FireKey)))
                    {
                        isKey =true;
                        setDataSource(p);
                    }
                    IGetter getter;
                    //Checks if the property is a collection
                    if (p.PropertyType.IsDefined(typeof(FireCollection)))
                    {
                        //New FireDataMapper with the property collection
                        IDataMapper db = new FireDataMapper(
                            p.PropertyType,
                            ProjectId,
                            ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection, // Value of collection propriety of Record.
                            CredentialsPath, dataSourceType);
                        getter = new ComplexPropertyGetter(p, db);
                    }
                    else
                    {
                        getter = new SimplePropertyGetter(p);
                        if(isKey)
                            FireKey=getter;
                        
                    }
                    //Adds property to properties list
                    properties.Add(getter);
                }
            }
            //Defines properties list
            this.properties = properties;
        }


        /*
        Creates and returns a new object from a given dictionary
        */

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
                    //newObjProperties[i] = p.PropertyType().IsValueType ? Activator.CreateInstance(p.PropertyType()) : null;
                }
                i++;
            }
            //New object instance
            object newObj = Activator.CreateInstance(objType, newObjProperties);
            return newObj;
        }
        List<IGetter> IDataMapper.GetPropertiesList()
        {
            return properties;
        }

        public IGetter GetFireKey()
        {
            return FireKey;
        }
    }

}