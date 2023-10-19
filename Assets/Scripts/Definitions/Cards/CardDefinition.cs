using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [System.Serializable]
    public class CardDefinition : ScriptableObject
    {
        [Header("Visuals")]
        public string Name;
        public RuntimeAnimatorController animatorController;

        [Header("Stats")]
        public int Cost;
    }
}
