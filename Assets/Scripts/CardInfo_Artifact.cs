using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    [CreateAssetMenu(fileName = "New Artifact Info", menuName = "Cards/Artifact Info")]
    public class CardInfo_Artifact : CardInfo
    {

        [Header("Spell Info")]
        [TextArea] public string Description;
    }
}

