using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Editor.EventEditor.Utilities;
using Editor.EventEditor.Windows;
using Map.Events;
using UnityEditor;
using System;

namespace Editor.EventEditor.Elements
{
    public class SlideNode : BaseNode
    {

        public string ID;


        public override MapEventActionLogic getAction()
        {
            return new MapEventActionLogic(Actions.GoToSlide, ID);
        }

        public override void Initialize(Vector2 position, EventGraphView graphView)
        {
            base.Initialize(position, graphView);

            NodeType = NodeType.Slide;

            Choices.Add("New Choice");
            ID = Guid.NewGuid().ToString();
        }

        

        protected override void MakeMain()
        {
            Button addChoiceButton = ElementUtility.CreateButton("Add Choice", () => { CreateChoicePort("New Choice"); Choices.Add("New Choice"); });
            addChoiceButton.AddToClassList("ds-node__button");
            mainContainer.Insert(1, addChoiceButton);
        }

        protected override void MakeOutput()
        {
            foreach (var choice in Choices)
                CreateChoicePort(choice);
        }

     
    }
}
