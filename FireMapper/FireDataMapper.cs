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
        //Credentials .json path
        string CredentialsPath;
        //Project Id
        string ProjectId;
        //Stores the collection properties
        List<PropertyInfo> properties;
        //Stores collections related to the current
        Dictionary<Type, IDataMapper> collections = new Dictionary<Type, IDataMapper>();
        
        /*
        Constructor
        */
        public FireDataMapper(Type objType, string ProjectId, string Collection, string CredentialsPath)
        {
            this.objType = objType;
            this.Collection = Collection;
            this.CredentialsPath = CredentialsPath;
            this.ProjectId = ProjectId;
            setProperties();
            setDataSource();
        }

        /*
        Add new object to DB
        */
        void IDataMapper.Add(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //Iterates over the properties list
            foreach (PropertyInfo p in properties)
            {
                //Checks if the property is linked to another collection and is value is not null
                if (collections != null && collections.ContainsKey(p.PropertyType) && p.GetValue(obj) != null)
                {
                    //Iterates over the collection object properties of the linked collection
                    foreach (PropertyInfo item in p.PropertyType.GetProperties())
                    {
                        //Checks if the property is a key
                        if (item.IsDefined(typeof(FireKey)))
                        {
                            object propertyObj = p.GetValue(obj, null);
                            //Key value from the linked collection
                            object key = collections[p.PropertyType].GetById(item.GetValue(propertyObj, null));
                            //Checks if the key exists in the linked collection
                            if (key != null)
                            //Adds property name and value to the dictionary
                                dictionary.Add(p.Name, item.GetValue(propertyObj, null));
                            else return;
                        }
                    }
                }
                else
                {
                    //Adds property name and value to the dictionary
                    dictionary.Add(p.Name, p.GetValue(obj, null));
                }
            }
            //Adds the dictionary value to the DB
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
            foreach (PropertyInfo p in properties)
            { 
                //Checks if the property is linked to another collection and is value is not null
                if (collections != null && collections.ContainsKey(p.PropertyType) && p.GetValue(obj) != null)
                {
                    //Iterates over the collection object properties of the linked collection
                    foreach (PropertyInfo item in p.PropertyType.GetProperties())
                    {
                        //Checks if the property is a key
                        if (item.IsDefined(typeof(FireKey)))
                        {
                            object propertyObj = p.GetValue(obj, null);
                            //Key value from the linked collection
                            object key = collections[p.PropertyType].GetById(item.GetValue(propertyObj, null));
                            //Checks if the key exists in the linked collection
                            if (key != null)
                                //Adds property name and value to the dictionary
                                dictionary.Add(p.Name, item.GetValue(propertyObj, null));
                        }
                    }
                }
                else
                {
                    //Adds property name and value to the dictionary
                    dictionary.Add(p.Name, p.GetValue(obj, null));
                }
            }
            //Updates DB with new value
            dataSource.Update(dictionary);
        }

        /*
        Defines dataSource
        */
        void setDataSource()
        {
            //Iterates over the object properties
            foreach (PropertyInfo p in properties)
            {
                //Checks if the property is a key
                if (p.IsDefined(typeof(FireKey)))
                {
                    //New FireDataSource instance
                    dataSource = new FireDataSource(ProjectId, Collection, p.Name, CredentialsPath);
                    break;
                }
            }
        }

        /*
        Defines and populates properties list
        */
        void setProperties()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            //Iterates over the object properties
            foreach (PropertyInfo p in objType.GetProperties())
            {
                //Checks that the property is not ignored
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    //Checks if the property is a collection
                    if (p.PropertyType.IsDefined(typeof(FireCollection)))
                    {
                        //New FireDataMapper with the property collection
                        IDataMapper db = new FireDataMapper(
                            p.PropertyType,
                            ProjectId,
                            ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection, // Value of colletion propriety of Record.
                            CredentialsPath);
                        //Adds the new FireMapper to the collections list
                        collections[p.PropertyType] = db;
                    }
                    //Adds property to properties list
                    properties.Add(p);
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
            foreach (PropertyInfo p in properties)
            {
                //Checks if dictionary a property
                if (dictionary.ContainsKey(p.Name))
                {
                    //Checks whether the property is of the primitive type or is not a collection
                    if (p.PropertyType.IsPrimitive || !collections.ContainsKey(p.PropertyType))
                    {
                        //Obtains and converts the type of the property value in the dictionary to its original type. (Dictionary stores Int64)
                        object value = Convert.ChangeType(dictionary[p.Name], p.PropertyType);
                        //Add constructor argument
                        newObjProperties[i] = value;
                    }
                    else
                    {
                        //Adds to the constructor argument array in case the property is a collection ( Recursive call over GetByID )
                        newObjProperties[i] = collections[p.PropertyType].GetById(dictionary[p.Name]);
                    }
                }
                else
                {
                    //Adds a new instance or null to the constructor argument array in case a property is not present in properties list (with FireIgnore Attr) 
                    newObjProperties[i] = p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null;
                }
                i++;
            }
            //New object instance
            object newObj = Activator.CreateInstance(objType, newObjProperties);
            return newObj;
        }

    }
}