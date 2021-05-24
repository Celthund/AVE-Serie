using System;
using System.Collections;
using System.Collections.Generic;
using FireSource;
using System.Reflection;

namespace FireMapper
{
    public class FireDataMapper : AbstractDataMapper
    {
        public FireDataMapper(Type domain, string projectId, string collection, string credentialsPath, Type dataSourceType) :
        base (domain, projectId, collection, credentialsPath, dataSourceType)
        {
            setProperties();
        }

        /*
        Defines and populates properties list
        */
        protected override void setProperties()
        {
            List<IGetter> properties = new List<IGetter>();
            //Iterates over the object properties
            foreach (PropertyInfo p in domain.GetProperties())
            {

                //Checks that the property is not ignored
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    bool isKey = false;
                    if (p.IsDefined(typeof(FireKey)))
                    {
                        isKey = true;
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
                            FireKey = getter;
                        
                    }
                    //Adds property to properties list
                    properties.Add(getter);
                }
            }
            //Defines properties list
            this.properties = properties;
        }
   }
}