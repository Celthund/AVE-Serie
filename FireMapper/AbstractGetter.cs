using System;
using System.Collections.Generic;
using FireMapper;
public abstract class AbstractGetter : IGetter
{
    public readonly string name;
    public IDataMapper db;

    public AbstractGetter(string name)
    {
        this.name = name;
    }
    public AbstractGetter(string name, IDataMapper db){
        this.db= db;
        this.name = name;
    }


    public abstract Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj);

    

    public string GetName()
    {
        return name;
    }

    public abstract object GetValue(object target);

    public abstract bool IsDefined();

    public abstract Type PropertyType();

    
}