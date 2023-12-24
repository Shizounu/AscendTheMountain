using Editor.EventEditor.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

using Map.Events;
namespace Editor.EventEditor.Elements
{
    public abstract class BaseAction : BaseNode {
        public abstract string getTitle();

        protected override void MakeTitle()
        {
            TextField titleLabel = ElementUtility.CreateTextField(getTitle());
            titleLabel.isReadOnly = true;

            titleLabel.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            titleContainer.Add(titleLabel);
        }
        protected override void MakeInput()
        {
            base.MakeInput(); //stays default
        }
        protected override void MakeOutput()
        {
            Port choicePort = this.CreatePort();
            outputContainer.Add(choicePort);
        }
        protected override void MakeExtension()
        {
            
        }


    }
}
