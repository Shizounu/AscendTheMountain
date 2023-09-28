using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [System.Serializable]
    public class CardDefinition
    {
        [Header("Visuals")]
        public string Name;
    
        [Header("Stats")]
        public int Cost;
    }
}
