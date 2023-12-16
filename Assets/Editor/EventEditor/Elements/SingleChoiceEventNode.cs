using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Map.Events.Enumeration;
using UnityEditor.Experimental.GraphView;
using Editor.EventEditor.Utilities;
using Editor.EventEditor.Windows;


namespace Editor.EventEditor.Elements
{
	public class SingleChoiceEventNode : EventNode {

        public override void Initialize(Vector2 position, EventGraphView graphView)
        {
            base.Initialize(position, graphView);
            SlideType = SlideType.SingleChoice;

            Choices.Add("Next");
        }
        public override void Draw()
        {
            base.Draw();

            foreach (var choice in Choices) {
                Port choicePort = this.CreatePort(choice);
                choicePort.portName = choice;
                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
