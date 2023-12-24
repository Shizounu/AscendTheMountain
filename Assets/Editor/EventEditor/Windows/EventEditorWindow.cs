using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using Editor.EventEditor.Utilities;

namespace Editor.EventEditor.Windows
{
    public class EventEditorWindow : EditorWindow
    {
        private const string defaultEventName = "New Event";
        
        private string curEventName = "New Event";
        private EventGraphView graphView;
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

            toolbar.Add(ElementUtility.CreateTextField(defaultEventName, "FileName:", change => curEventName = change.newValue));
            toolbar.Add(ElementUtility.CreateButton("Save", () => DoSave()));

            toolbar.AddStyleSheets("EventEditor/ToolbarStyle.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("EventEditor/Variables.uss");
        }

        private void AddGraphView()
        {
            graphView = new EventGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void DoSave()
        {
            Utilities.SerializationUtility.Save(curEventName, graphView, graphView.entryNode);
        }
    }

}