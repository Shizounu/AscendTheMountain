using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using Map.Events.Enumeration;
using UnityEngine.UIElements;
using Editor.EventEditor.Utilities;
using Editor.EventEditor.Windows;

namespace Editor.EventEditor.Elements
{
    //TODO: Expand to fit wanted capabilities better, add extras for underlying actions
    public class EventNode : Node
    {
        public string SlideName { get; set; }
        public List<string> Choices { get; set; } 
        public string Text { get; set; }
        public SlideType SlideType { get; set; }
        protected EventGraphView graphView { get; set; }


        public virtual void Initialize(Vector2 position, EventGraphView graphView)
        {
            SlideName = "SlideName";
            Choices = new List<string>();
            Text = "Slide Text";

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");

            this.graphView = graphView;
        }

        public virtual void Draw() {
            //Title Container
            TextField slideNameTextField = Utilities.ElementUtility.CreateTextField(SlideName);
            titleContainer.Insert(0, slideNameTextField);

            slideNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            //input
            Port inputPort = this.CreatePort("Incoming", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(inputPort);

            //Extension container
            VisualElement customDataContainer = new();
            customDataContainer.AddToClassList("ds-node__custom-data-container");
            Foldout textFoldout = ElementUtility.CreateFoldout("Slide Text");
            TextField textTextField = ElementUtility.CreateTextArea(Text);

            textTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__quote-text-field"
            );

            textFoldout.Add(textTextField);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);

            //Redraws visuals
            RefreshExpandedState();
        }
    }
}
