using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [System.Serializable]
    public abstract class CardDefinition : ScriptableObject
    {
        [Header("Visuals")]
        public string Name;
        public RuntimeAnimatorController animatorController;
        public Sprite Icon;
        [Header("Stats")]
        public int Cost;

        public CardDefinition Clone()
        {
            return MemberwiseClone() as CardDefinition;
        }
    }
}
