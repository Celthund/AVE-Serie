using System;
using System.Collections;
using System.Collections.Generic;

public interface IGetter{

    string GetName();
    object GetValue(object obj);

    object GetKeyValue(object obj);

    object GetDefaultValue();

    
}