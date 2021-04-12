using System;
using System.Collections;
using System.Collections.Generic;
using FireSource;
using System.Reflection;

namespace FireMapper
{
    public class FireDataMapper : IDataMapper
    {
        Type obj;
        string Collection;
        IDataSource dataSource;
        string CredentialsPath;
        string ProjectId;

        public FireDataMapper(Type obj, string ProjectId, string Collection, string CredentialsPath)
        {
            this.obj = obj;
            this.Collection = Collection;
            this.CredentialsPath = CredentialsPath;
            this.ProjectId = ProjectId;
            setDataSource();
        }

        void IDataMapper.Add(object obj)
        {
            throw new NotImplementedException();
        }

        void IDataMapper.Delete(object keyValue)
        {
            throw new NotImplementedException();
        }

        IEnumerable IDataMapper.GetAll()
        {
            return dataSource.GetAll();
        }

        object IDataMapper.GetById(object keyValue)
        {
            Dictionary<string, object> dictionary;
            dictionary = dataSource.GetById(keyValue);
            object[] y = new object[obj.GetProperties().Length];
            int i = 0;
            foreach (PropertyInfo p in obj.GetProperties())
            {
                if (!p.IsDefined(typeof(FireIgnore)) && dictionary.ContainsKey(p.Name))
                {
                    y[i] = dictionary[p.Name];
                }
                else
                {
                    y[i] = p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null;
                }

                i++;
            }

            object x = Activator.CreateInstance(obj, y);
            return dictionary;
        }

        void IDataMapper.Update(object obj)
        {
            throw new NotImplementedException();
        }

        void setDataSource()
        {

            foreach (PropertyInfo p in obj.GetProperties())
            {
                if (p.IsDefined(typeof(FireKey)))
                {
                    dataSource = new FireDataSource(ProjectId, Collection, p.Name, CredentialsPath);
                    break;
                }

            }
        }

    }
}