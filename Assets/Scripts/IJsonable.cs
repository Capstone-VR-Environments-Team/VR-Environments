using System.Collections.Generic;
using UnityEngine;

public interface IJsonable
{
    virtual public void From2dList(List<List<string>> data) {
        Debug.LogError($"Cannot convert {GetType()} from Json");
    }
}
