using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtension
{
    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> mergeTo, Dictionary<TKey, TValue> mergeFrom) {
        mergeFrom.ToList().ForEach(x => mergeTo.AddElementIfUnique(x));
    }
    public static void AddElementIfUnique<TKey, TValue>(this Dictionary<TKey, TValue> mergeTo, KeyValuePair<TKey, TValue> elem) {
        if(!mergeTo.ContainsKey(elem.Key)) 
            mergeTo.Add(elem.Key, elem.Value);
    }
}