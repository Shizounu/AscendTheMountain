using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    [CreateAssetMenu(fileName = "New Spell Info", menuName = "Cards/Spell Info")]
    public class CardInfo_Spell : CardInfo
    {
        [TextArea] public string Description;
    }
    
}
