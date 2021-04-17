using System;
using System.Collections;
using System.Collections.Generic;
using FireSource;
using System.Reflection;

namespace FireMapper
{
    public class FireDataMapper : IDataMapper
    {
        //object Type
        Type objType;
        //collection name
        string Collection;
        //IDatasource
        IDataSource dataSource;
        //credentials .json path
        string CredentialsPath;
        //project Id
        string ProjectId;
        //stores the collection properties
        List<PropertyInfo> properties;
        //stores collections related to the current
        Dictionary<Type, IDataMapper> collections = new Dictionary<Type, IDataMapper>();
        
        //constructor
        public FireDataMapper(Type objType, string ProjectId, string Collection, string CredentialsPath)
        {
            this.objType = objType;
            this.Collection = Collection;
            this.CredentialsPath = CredentialsPath;
            this.ProjectId = ProjectId;
            setProperties();
            setDataSource();
        }

        //Add new object to DB
        void IDataMapper.Add(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //iterates over the properties list
            foreach (PropertyInfo p in properties)
            {
                //checks if the property is linked to another collection and is value is not null
                if (collections != null && collections.ContainsKey(p.PropertyType) && p.GetValue(obj) != null)
                {
                    //iterates over the collection object properties of the linked collection
                    foreach (PropertyInfo item in p.PropertyType.GetProperties())
                    {
                        //checks if the property is a key
                        if (item.IsDefined(typeof(FireKey)))
                        {
                            object propertyObj = p.GetValue(obj, null);
                            //key value from the linked collection
                            object key = collections[p.PropertyType].GetById(item.GetValue(propertyObj, null));
                            //checks if the key exists in the linked collection
                            if (key != null)
                            //adds property name and key value to the dictionary
                                dictionary.Add(p.Name, item.GetValue(propertyObj, null));
                            else return;
                        }
                    }
                }
                else
                {
                    //add key- value to the dictionary
                    dictionary.Add(p.Name, p.GetValue(obj, null));
                }
            }
            //add dictionary value to DB
            dataSource.Add(dictionary);
        }

        //deletes the object related to the given key 
        void IDataMapper.Delete(object keyValue)
        {
            dataSource.Delete(keyValue);
        }

        //gets all database data in object form
        IEnumerable IDataMapper.GetAll()
        {
            //objects list
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

        //returns the object from the given key
        object IDataMapper.GetById(object keyValue)
        {
            Dictionary<string, object> dictionary;
            dictionary = dataSource.GetById(keyValue);
            return CreateObject(dictionary);
        }

        //updates an object in the DB
        void IDataMapper.Update(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //iterates over the properties list
            foreach (PropertyInfo p in properties)
            { 
                //checks if the property is linked to another collection and is value is not null
                if (collections != null && collections.ContainsKey(p.PropertyType) && p.GetValue(obj) != null)
                {
                    //iterates over the collection object properties of the linked collection
                    foreach (PropertyInfo item in p.PropertyType.GetProperties())
                    {
                        //checks if the property is a key
                        if (item.IsDefined(typeof(FireKey)))
                        {
                            object propertyObj = p.GetValue(obj, null);
                            //key value from the linked collection
                            object key = collections[p.PropertyType].GetById(item.GetValue(propertyObj, null));
                            //checks if the key exists in the linked collection
                            if (key != null)
                                //adds property name and key value to the dictionary
                                dictionary.Add(p.Name, item.GetValue(propertyObj, null));
                        }
                    }
                }
                else
                {
                    //add key - value to the dictionary
                    dictionary.Add(p.Name, p.GetValue(obj, null));
                }
            }
            //update DB with dictionary value
            dataSource.Update(dictionary);
        }

        //defines dataSource
        void setDataSource()
        {
            //iterates over the object properties
            foreach (PropertyInfo p in properties)
            {
                //checks if the property is a key
                if (p.IsDefined(typeof(FireKey)))
                {
                    //new FireDataSource instance
                    dataSource = new FireDataSource(ProjectId, Collection, p.Name, CredentialsPath);
                    break;
                }
            }
        }

        //defines and populates properties list
        void setProperties()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            //iterates over the object properties
            foreach (PropertyInfo p in objType.GetProperties())
            {
                //checks that the property is not ignored
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    //checks if the property is a collection
                    if (p.PropertyType.IsDefined(typeof(FireCollection)))
                    {
                        //new FireDataMapper with the property collection
                        IDataMapper db = new FireDataMapper(
                            p.PropertyType,
                            ProjectId,
                            ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection, // Value of colletion propriety of Record.
                            CredentialsPath);
                        //add the new FireMapper to the collections list
                        collections[p.PropertyType] = db;
                    }
                    //add property to properties list
                    properties.Add(p);
                }
            }
            //defines properties list
            this.properties = properties;
        }


        //creates and returns a new object from a given dictionary
        object CreateObject(Dictionary<string, object> dictionary)
        {
            if (dictionary is null) return null;
            //array with object constructor arguments
            object[] newObjProperties = new object[properties.Count];
            int i = 0;
            //iterates over the properties list
            foreach (PropertyInfo p in properties)
            {
                //checks if dictionary a property
                if (dictionary.ContainsKey(p.Name))
                {
                    //checks whether the property is of the primitive type or is not a collection
                    if (p.PropertyType.IsPrimitive || !collections.ContainsKey(p.PropertyType))
                    {
                        //obtains and converts the type of the property value in the dictionary to its original type. dictionary stores only Int64
                        object value = Convert.ChangeType(dictionary[p.Name], p.PropertyType);
                        //add constructor argument
                        newObjProperties[i] = value;
                    }
                    else
                    {
                        //adds to the constructor argument array in case the property is a collection
                        newObjProperties[i] = collections[p.PropertyType].GetById(dictionary[p.Name]);
                    }
                }
                else
                {
                    //adds a new instance or null to the constructor argument array in case a property is not present in properties list (with FireIgnore Attr) 
                    newObjProperties[i] = p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null;
                }
                i++;
            }
            //new object instance
            object newObj = Activator.CreateInstance(objType, newObjProperties);
            return newObj;
        }

    }
}