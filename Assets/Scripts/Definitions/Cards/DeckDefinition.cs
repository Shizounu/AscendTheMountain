using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "new Deck", menuName = "Cards/Deck")]
    public class DeckDefinition : ScriptableObject {
        public List<CardDefinition> Cards;
    }
}
