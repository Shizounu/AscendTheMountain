using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using Editor.EventEditor.Utilities;
using Unity.Plastic.Newtonsoft.Json;

namespace Editor.EventEditor.Windows
{
    public class EventEditorWindow : EditorWindow
    {
        private const string defaultEventName = "New Event";

        [MenuItem("Shizounu/Event Editor")]
        public static void Open()
        {
            GetWindow<EventEditorWindow>("Event Editor");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();
            AddStyles();
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            toolbar.Add(ElementUtility.CreateTextField(defaultEventName, "FileName:"));
            toolbar.Add(ElementUtility.CreateButton("Save"));

            toolbar.AddStyleSheets("EventEditor/ToolbarStyle.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("EventEditor/Variables.uss");
        }

        private void AddGraphView()
        {
            EventGraphView graphView = new EventGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }
    }

}