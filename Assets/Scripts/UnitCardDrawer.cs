using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Card
{
    public class UnitCardDrawer : MonoBehaviour
    {
        public CardInfo_Unit UnitInfo;

        [Header("References")]
        [SerializeField] private SpriteRenderer UnitSprite;
        [SerializeField] private TMPro.TextMeshPro NameText;
        [SerializeField] private TMPro.TextMeshPro TribeText;
        [Space()]
        [SerializeField] private TMPro.TextMeshPro AttackText;
        [SerializeField] private TMPro.TextMeshPro HealthText;
        [SerializeField] private TMPro.TextMeshPro DescriptionText;
        
        private void OnValidate() {
            LoadInfo();
        }

        [ContextMenu("Load info")]
        private void LoadInfo(){
            if(UnitInfo == null){
                //Debug.LogError("No card info selected");
                return;
            }

            NameText.text = UnitInfo.Name;
            TribeText.text = UnitInfo.Tribe;
            UnitSprite.sprite = UnitInfo.Icon;
            AttackText.text = $"{UnitInfo.Attack}";
            HealthText.text = $"{UnitInfo.Health}";
            DescriptionText.text = UnitInfo.EffectDescription;
        }
    }
}
