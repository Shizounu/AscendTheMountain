using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardDefinition_Unit", menuName = "Gameplay/CardDefinition_Unit", order = 0)]
public class CardDefinition_Unit : CardDefinition{
    public int Attack;
    public int Health;
    public int MoveDistance;
}
