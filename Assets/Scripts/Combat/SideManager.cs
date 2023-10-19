using Cards;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// Control independant and equal between both sides
    /// Would then be affected by a corresponding "Controller" which interacts with this and can be changed
    /// </summary>
    public class SideManager : MonoBehaviour
    {
        public List<CardDefinition> Deck;
        public CardDefinition[] Hand = new CardDefinition[5];

        public Combat.GameManager GameManager;

        public int curMana;
        public int maxMana;

        private void Start()
        {

        }

        public void PlayCard(CardDefinition definition, Vector2Int position) {
            if(definition.GetType() == typeof(UnitDefinition)) {
                PlayUnit((UnitDefinition)definition, position);
                return;
            }


            Debug.LogError("Undefined Card Type");
        }

        private void PlayUnit(UnitDefinition definition, Vector2Int position) {
            GameManager.SummonUnit(definition, position, this);
        }

        private void PlaySpell()
        {

        }

        private void PlayArtifact()
        {

        }

        
    }

}
