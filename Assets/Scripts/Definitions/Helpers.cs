using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static T GetInterface<T>(System.Object obj) {
        return (obj is T) ? (T)obj : default(T);
    }
}
