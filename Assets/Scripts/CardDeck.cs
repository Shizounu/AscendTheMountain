using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[CreateAssetMenu(fileName = "CardDeck", menuName = "Gameplay/CardDeck", order = 0)]
public class CardDeck : ScriptableObject {
    public List<CardDefinition> deckDefinition;
    public List<CardDefinition> currentDeck;

    public void initializeDeck(){
        
        currentDeck = new List<CardDefinition>(deckDefinition);
        ShuffleDeck(currentDeck);
    }
    public void deinitializeDeck(){
        currentDeck = new();
    }

    public void ShuffleDeck(List<CardDefinition> deck){
        System.Random rng = new();
        int n = deck.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            CardDefinition value = deck[k];  
            deck[k] = deck[n];  
            deck[n] = value;  
        }  
    }

    public CardDefinition DrawCard(){
        currentDeck.RemoveAt(0);
        return currentDeck[0];
    } 

    
}
