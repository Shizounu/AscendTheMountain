using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Card
{
    [CreateAssetMenu(fileName = "New Unit Info", menuName = "Cards/Unit Info")]
    public class CardInfo_Unit : CardInfo
    {

        [Header("Unit Stats")]
        public int Attack = 1;
        public int Health = 1;

        
        [TextArea] public string EffectDescription;

        private void OnValidate() {
            if(Attack < 0)
                Attack = 0;
            if(Health < 1)
                Health = 1;
        }

        //Keywords 
        // Reference "Ideas for Implementation" in Design Doc
    }
    
}
