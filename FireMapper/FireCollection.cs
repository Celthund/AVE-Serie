using System;

public class FireCollection : Attribute
{
    public FireCollection(string collection)
    {
        this.collection = collection;
    }

    public string collection { get; }
}