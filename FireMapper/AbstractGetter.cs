using System;
using System.Collections.Generic;
using FireMapper;
public abstract class AbstractGetter : IGetter
{
    public string name;
    public IDataMapper db;

    public AbstractGetter(string name)
    {
        this.name = name;
    }
    public AbstractGetter(string name, IDataMapper db)
    {
        this.db = db;
        this.name = name;
    }


    public abstract object GetDefaultValue();

    public abstract object GetKeyValue(object obj);

    public string GetName()
    {
        return name;
    }

    public abstract object GetValue(object target);

}