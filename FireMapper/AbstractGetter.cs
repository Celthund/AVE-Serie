using System;
using System.Collections.Generic;
using FireMapper;
public abstract class AbstractGetter : IGetter
{
    public string name;
    public IDataMapper db;

    public bool isKey;

    public AbstractGetter(string name)
    {
        this.name = name;
    }
    public AbstractGetter(string name, IDataMapper db)
    {
        this.db = db;
        this.name = name;
    }
    public AbstractGetter(string name, bool isKey)
    {
        this.name = name;
        this.isKey = isKey;
    }


    public abstract object GetDefaultValue();

    public abstract object GetKeyValue(object obj);

    public string GetName()
    {
        return name;
    }

    public abstract object GetValue(object target);

    public  bool IsDefined()
    {
        return isKey;
    }




}