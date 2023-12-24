using Editor.EventEditor.Utilities;
using Map.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.EventEditor.Elements
{
    public class EntryNode : BaseNode

    {
        protected override void MakeInput()
        {
            //No Entry as this is the entry into the graph
        }
        protected override void MakeOutput()
        {
            Port choicePort = this.CreatePort("Start");
            outputContainer.Add(choicePort);
        }
        protected override void MakeTitle()
        {
            TextField titleLabel = ElementUtility.CreateTextField("Entry");
            titleLabel.isReadOnly = true;

            titleLabel.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );
            titleContainer.style.color = new Color(0, 153, 0); //TODO fix color to actually apply
            

            titleContainer.Add(titleLabel);

            
        }
        protected override void MakeExtension()
        {
            //Removing the text body
        }

        public override MapEventActionLogic getAction()
        {
            throw new System.NotImplementedException();
        }
    }


}
