using Editor.EventEditor.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

using UnityEngine.UIElements;
using Map.Events;
using System;

namespace Editor.EventEditor.Elements
{

    public class NodeAction_AddHealth : BaseAction
    {
        private IntegerField valueField;
        public override string getTitle()
        {
            return "Add Health";
        }
        public override MapEventActionLogic getAction()
        {
            return new MapEventActionLogic(Actions.AddHealth, valueField.value.ToString());
        }

        protected override void MakeMain()
        {
            base.MakeMain();
            valueField = new IntegerField("Value:");

            valueField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
                );

            mainContainer.Add(valueField);
        }

    }
}