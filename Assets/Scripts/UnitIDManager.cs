using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitIDManager {
    private const int UIDCount = 512; 

    public UnitIDManager() {
        _Instance = this;

        UniqueUnitIDs = new();
        for (int i = 0; i < UIDCount; i++)
            UniqueUnitIDs.Add(Guid.NewGuid().ToString());
    }

    private static UnitIDManager _Instance; 
    public static UnitIDManager Instance
    {
        get {
            if(_Instance == null)
                _Instance = new UnitIDManager();
            return _Instance;
        }
    }

    private readonly List<string> UniqueUnitIDs;

    public string GetUID(int curIndex) {
        return UniqueUnitIDs[curIndex];
    }
}
