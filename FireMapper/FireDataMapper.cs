using System;
using System.Collections;
using System.Collections.Generic;
using FireSource;
using System.Reflection;

namespace FireMapper
{
    public class FireDataMapper : IDataMapper
    {
        Type objType;
        string Collection;
        IDataSource dataSource;
        string CredentialsPath;
        string ProjectId;
        List<PropertyInfo> properties;
        Dictionary<Type, IDataMapper> collections = new Dictionary<Type, IDataMapper>();

        public FireDataMapper(Type objType, string ProjectId, string Collection, string CredentialsPath)
        {
            this.objType = objType;
            this.Collection = Collection;
            this.CredentialsPath = CredentialsPath;
            this.ProjectId = ProjectId;
            setProperties();
            setDataSource();
        }

        void IDataMapper.Add(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo p in properties)
            {   //property classroom é do tipo object. Necessário retirar apenas o valor do campo token como string
                // para introduzir na DB
                if (collections != null && collections.ContainsKey(p.PropertyType) && p.GetValue(obj) != null)
                {
                    foreach (PropertyInfo item in p.PropertyType.GetProperties())
                    {
                        if (item.IsDefined(typeof(FireKey)))
                        {
                            object propertyObj = p.GetValue(obj, null);
                            //verifica se existe a classroom
                            object objct = collections[p.PropertyType].GetById(item.GetValue(propertyObj, null));
                            if (objct != null)
                                dictionary.Add(p.Name, item.GetValue(propertyObj, null));
                            else return;
                        }
                    }
                }
                else
                {
                    dictionary.Add(p.Name, p.GetValue(obj, null));
                }
            }
            dataSource.Add(dictionary);
        }

        void IDataMapper.Delete(object keyValue)
        {
            dataSource.Delete(keyValue);
        }

        IEnumerable IDataMapper.GetAll()
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

        object IDataMapper.GetById(object keyValue)
        {
            Dictionary<string, object> dictionary;
            dictionary = dataSource.GetById(keyValue);
            return CreateObject(dictionary);
        }

        void IDataMapper.Update(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo p in properties)
            {
                if (collections != null && collections.ContainsKey(p.PropertyType) && p.GetValue(obj) != null)
                {
                    foreach (PropertyInfo item in p.PropertyType.GetProperties())
                    {
                        if (item.IsDefined(typeof(FireKey)))
                        {
                            object propertyObj = p.GetValue(obj, null);
                            //verifica se existe a classroom na DB
                            object objct = collections[p.PropertyType].GetById(item.GetValue(propertyObj, null));
                            if (objct != null)
                                dictionary.Add(p.Name, item.GetValue(propertyObj, null));
                        }
                    }
                }
                else
                {
                    dictionary.Add(p.Name, p.GetValue(obj, null));
                }
            }
            dataSource.Update(dictionary);
        }

        void setDataSource()
        {
            foreach (PropertyInfo p in properties)
            {
                if (p.IsDefined(typeof(FireKey)))
                {
                    dataSource = new FireDataSource(ProjectId, Collection, p.Name, CredentialsPath);
                    break;
                }
            }
        }

        void setProperties()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            foreach (PropertyInfo p in objType.GetProperties())
            {
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    if (p.PropertyType.IsDefined(typeof(FireCollection)))
                    {
                        IDataMapper db = new FireDataMapper(
                            p.PropertyType,
                            ProjectId,
                            ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection, // Value of colletion propriety of Record.
                            CredentialsPath);
                        collections[p.PropertyType] = db;

                    }
                    properties.Add(p);
                }
            }
            this.properties = properties;
        }



        object CreateObject(Dictionary<string, object> dictionary)
        {
            if (dictionary is null) return null;
            object[] newObjProperties = new object[properties.Count];
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                if (dictionary.ContainsKey(p.Name))
                {
                    if (p.PropertyType.IsPrimitive || !collections.ContainsKey(p.PropertyType))
                    {
                        object value = Convert.ChangeType(dictionary[p.Name], p.PropertyType);
                        newObjProperties[i] = value;
                    }
                    else
                    {
                        newObjProperties[i] = collections[p.PropertyType].GetById(dictionary[p.Name]);
                    }
                }
                else
                {
                    newObjProperties[i] = p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null;
                }
                i++;
            }
            object newObj = Activator.CreateInstance(objType, newObjProperties);
            return newObj;
        }

    }
}