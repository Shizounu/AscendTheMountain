using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shizounu.Library.Editor;

namespace Battlefield{
    public class Deck : MonoBehaviour
    {
        public CardDeck deck;

        public CardDefinition[] hand = new CardDefinition[6];

        private void Start() {
            deck.initializeDeck();
            for (int i = 0; i < 3; i++){
                drawCard();
            }
        }

        public void drawCard(){
            int indx = freeIndex();
            if(indx == -1)
            {
                Debug.Log("Hands full");
                return;
            }

            hand[indx] = deck.DrawCard();
        }

        public int freeIndex(){
            for (int i = 0; i < hand.Length; i++){
                if(hand[i] == null)
                    return i;
            }
            return -1;
        }
    }

}
