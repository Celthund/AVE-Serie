using System;
using System.Collections.Generic;
using FireMapper;
public abstract class AbstractGetter : IGetter
{
    public string name;
    public IDataMapper db;

    /*
    Constructor for Simple getter
    */
    public AbstractGetter(string name)
    {
        this.name = name;
    }
    /*
    Constructor for Complex getter
    */
    public AbstractGetter(string name, IDataMapper db)
    {
        this.db = db;
        this.name = name;
    }
    /*
    Get default value
    */
    public abstract object GetDefaultValue();
    /*
    Get key value
    */
    public abstract object GetKeyValue(object obj);
    /*
    Get name
    */
    public string GetName()
    {
        return name;
    }
    /*
    Get value
    */
    public abstract object GetValue(object target);

}