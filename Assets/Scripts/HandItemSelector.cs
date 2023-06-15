using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlefield
{
    public class HandItemSelector : MonoBehaviour
    {
        public int handIndex = 0;
        public Battlefield.Controller.Controller controller;

        public void selectCard(){
            controller.selectHandCard(handIndex);
        }
    }
}
