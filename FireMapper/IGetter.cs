using System;
using System.Collections;
using System.Collections.Generic;

public interface IGetter{

    string GetName();
    object GetValue(object obj);

    bool IsDefined();

    object GetKeyValue(object obj);

    object GetDefaultValue();

    //Dictionary<string, object> FillDictionary(Dictionary<string, object> dictionary, object obj);
    
}