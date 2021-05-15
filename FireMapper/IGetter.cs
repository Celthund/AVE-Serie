using System;
using System.Collections;
using System.Collections.Generic;

public interface IGetter{

    string GetName();
    object GetValue(object obj);

    bool IsDefined();

    Type PropertyType();

    Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj);
    
}