using Editor.EventEditor.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

using UnityEngine.UIElements;
namespace Editor.EventEditor.Elements
{

    public class NodeAction_AddGold : BaseAction
    {
        private int value;
        public override string getTitle()
        {
            return "Add Gold";
        }
        public override string getAction()
        {
            throw new System.NotImplementedException();
        }

        protected override void MakeMain()
        {
            base.MakeMain();
            IntegerField valueField = new IntegerField();
            valueField.label = "Value:";

            valueField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
                );


            value = valueField.value;

            mainContainer.Add(valueField);
        }

    }
}