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
        static int objPropertiesNumber = 0;

        public FireDataMapper(Type objType, string ProjectId, string Collection, string CredentialsPath)
        {
            this.objType = objType;
            this.Collection = Collection;
            this.CredentialsPath = CredentialsPath;
            this.ProjectId = ProjectId;
            this.properties = setProperties();
            setDataSource();
        }

        void IDataMapper.Add(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (PropertyInfo p in properties)
            {
                dictionary.Add(p.Name, p.GetValue(obj, null));
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
            foreach (var item in dataSource.GetAll())
            {
                objs.Add(CreateObject(item));
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
                dictionary.Add(p.Name, p.GetValue(obj, null));
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

        List<PropertyInfo> setProperties()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            foreach (PropertyInfo p in objType.GetProperties())
            {
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    properties.Add(p);
                }
                objPropertiesNumber++;

            }
            return properties;
        }

        object CreateObject(Dictionary<string, object> dictionary)
        {

            object[] newObjProperties = new object[objPropertiesNumber];
            int i = 0;
            foreach (PropertyInfo p in properties)
            {
                if (dictionary.ContainsKey(p.Name))
                {
                    object value = Convert.ChangeType(dictionary[p.Name], p.PropertyType);
                    newObjProperties[i] = value;
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