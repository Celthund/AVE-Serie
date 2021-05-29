using System.Collections;
using System.Collections.Generic;
using FireSource;
using System.Reflection;


using System;
namespace FireMapper
{
    public class DynamicFireMapper : AbstractDataMapper
    {
        DynamicGetterBuilder getterBuilder;
 
        /*
        Constructor
        */
        public DynamicFireMapper(Type domain, string projectId, string collection, string credentialsPath, Type dataSourceType) : 
        base( domain, projectId, collection, credentialsPath, dataSourceType)
        {
        }
         protected override void setProperties()
        {

            getterBuilder = new DynamicGetterBuilder(domain);
            
            List<IGetter> properties = new List<IGetter>();

            //Iterates over the object properties
            foreach (PropertyInfo p in domain.GetProperties())
            {
                bool isKey = false;   
                //Checks that the property is not ignored
                if (!p.IsDefined(typeof(FireIgnore)))
                {
                    if (p.IsDefined(typeof(FireKey)))
                    {
                        setDataSource(p);
                        isKey = true;
                        
                    }
                    IGetter getter;

                    //Checks if the property is a collection
                    if (p.PropertyType.IsDefined(typeof(FireCollection)))
                    {
                        IDataMapper db = new DynamicFireMapper(
                            p.PropertyType,
                            ProjectId,
                            ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection, // Value of collection propriety of Record.
                            CredentialsPath,
                            dataSourceType);
                        //New FireDataMapper with the property collection                       
                        //string OtherCollection = ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection;
                        Type getterType = getterBuilder.GenerateComplexGetter(p);
                        getter = (IGetter)Activator.CreateInstance(getterType, new object[]{p.Name, db});
                    }
                    else
                    {
                        Type getterType = getterBuilder.GenerateSimpleGetter(p);
                        getter = (IGetter)Activator.CreateInstance(getterType);
                    
                    }
                    if(isKey)
                        FireKey = getter;  
                    //Adds property to properties list
                    properties.Add(getter);
                }
            }
            //Defines properties list
            getterBuilder.SaveModule();
            this.properties = properties;
        }
    }
}