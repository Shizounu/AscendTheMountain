using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "new Deck", menuName = "Cards/Deck")]
    public class DeckDefinition : ScriptableObject {
        public UnitDefinition SideGeneral; //TODO: Make more sophisticated

        public List<CardDefinition> Cards;
    }
}
