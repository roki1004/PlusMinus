using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //> argumentNullException

public class MyCloneable
{
    public static T DeepClone<T>(T obj)
    {
        return (T)obj;
    }
}
