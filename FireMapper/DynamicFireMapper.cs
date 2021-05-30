using System.Collections.Generic;
using System.Reflection;
using System;

namespace FireMapper
{
    public class DynamicFireMapper : AbstractDataMapper
    {
        //Dynamic Getter Builder
        DynamicGetterBuilder getterBuilder;

        /*
        Constructor
        */
        public DynamicFireMapper(Type domain, string projectId, string collection, string credentialsPath, Type dataSourceType) : 
        base( domain, projectId, collection, credentialsPath, dataSourceType)
        {
        }
        /*
        Defines and populates properties list
        */
         protected override void setProperties()
        {
            //Instantiate a new Dymanic Getter builder
            getterBuilder = new DynamicGetterBuilder(domain);
            //List of IGetters created dynamically
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
                        //New DynamicDataMapper with the property collection  
                        IDataMapper db = new DynamicFireMapper(
                            p.PropertyType,
                            ProjectId,
                            ((FireCollection)p.PropertyType.GetCustomAttributes(typeof(FireCollection), false).GetValue(0)).collection, 
                            CredentialsPath,
                            dataSourceType);
                        //Create the class that extends AbstractGetter for that property p in domain type.            
                        Type getterType = getterBuilder.GenerateComplexGetter(p);
                        //Instanciate the new class
                        getter = (IGetter)Activator.CreateInstance(getterType, new object[]{p.Name, db});
                    }
                    else
                    {
                        //Create the class that extends AbstractGetter for that property p in domain type.
                        Type getterType = getterBuilder.GenerateSimpleGetter(p);
                        //Instanciate the new class
                        getter = (IGetter)Activator.CreateInstance(getterType);
                    
                    }
                    //Checks and defines the FireKey
                    if(isKey)
                        FireKey = getter;  
                    //Adds property to properties list
                    properties.Add(getter);
                }
            }
            //Save the module to a file 
            getterBuilder.SaveModule();
            //Defines properties list
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("SAVE_MODULES")))
                getterBuilder.SaveModule();
            this.propertyGetters = properties;
        }
    }
}