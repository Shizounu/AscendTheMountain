using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Map.Events.Enumeration;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Editor.EventEditor.Utilities;
using Editor.EventEditor.Windows;
namespace Editor.EventEditor.Elements
{
    public class MultipleChoiceEventNode : EventNode
    {

        public override void Initialize(Vector2 position, EventGraphView graphView)
        {
            base.Initialize(position, graphView);
            SlideType = SlideType.MultipleChoice;

            Choices.Add("New Choice");
        }
        public override void Draw()
        {
            base.Draw();

            //Add connection button
            Button addChoiceButton = ElementUtility.CreateButton("Add Choice", () => { CreateChoicePort("New Choice"); Choices.Add("New Choice"); });
            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);


            foreach (var choice in Choices)
                CreateChoicePort(choice);

            RefreshExpandedState();
        }

        private void CreateChoicePort(string choice)
        {
            Port choicePort = this.CreatePort();

            Button deleteChoiceButton = ElementUtility.CreateButton("X", () => {
                if(choicePort.connected) {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choice);
                graphView.RemoveElement(choicePort);
            });
            deleteChoiceButton.AddToClassList("ds-node__button");
            TextField choiceTextField = ElementUtility.CreateTextField(choice);

            choiceTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            outputContainer.Add(choicePort);
        }
    }
}
