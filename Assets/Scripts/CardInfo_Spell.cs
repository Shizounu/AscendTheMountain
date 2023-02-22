using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    [CreateAssetMenu(fileName = "New Spell Info", menuName = "Cards/Spell Info")]
    public class CardInfo_Spell : CardInfo
    {
        [Header("Spell Info")]
        [TextArea] public string Description;
    }
    
}
