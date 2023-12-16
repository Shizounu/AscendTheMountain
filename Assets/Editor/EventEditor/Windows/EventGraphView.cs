using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using Editor.EventEditor.Elements;
using Map.Events.Enumeration;
using Editor.EventEditor.Utilities;


namespace Editor.EventEditor.Windows
{
    public class EventGraphView : GraphView {
        private GraphSearchWindow searchWindow;
        private EventEditorWindow editorWindow;

        public EventGraphView(EventEditorWindow editorWindow) {
            this.editorWindow = editorWindow;   

            AddManipulators();
            AddGridBackground();
            AddStyles();
            AddSearchWindow();
        }

        private void AddSearchWindow()
        {
            if(searchWindow == null)
                searchWindow = ScriptableObject.CreateInstance<GraphSearchWindow>();
            searchWindow.Initialize(this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger()); //needs to be before selector... for some reason
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(CreateContextualMenu("Add Node (Single Choice)", SlideType.SingleChoice));
            this.AddManipulator(CreateContextualMenu("Add Node (Multiple Choice)", SlideType.MultipleChoice));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new(
                menuEvent => menuEvent.menu.AppendAction(
                    "Add Group", 
                    actionEvent => AddElement(CreateGroup("Dialogue Group", getLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
                ));

            return contextualMenuManipulator;
        }

        public GraphElement CreateGroup(string title, Vector2 localMousePosition)
        {
            Group group = new Group()
            {
                title = title
            };
            group.SetPosition(new Rect(localMousePosition, Vector2.zero));

            foreach (GraphElement selectedElement in selection) {
                if(selectedElement is EventNode) {
                    EventNode node = selectedElement as EventNode;
                    group.AddElement(node);
                }
            }

            return group;
        }

        private IManipulator CreateContextualMenu(string actionTitle, SlideType type)
        {
            ContextualMenuManipulator contextualMenuManipulator = new(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(type, getLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }

        public EventNode CreateNode(SlideType type, Vector2 pos)
        {
            EventNode node = (type == SlideType.SingleChoice) ? new SingleChoiceEventNode() : new MultipleChoiceEventNode();

            node.Initialize(pos, this);
            node.Draw();

            return node;
        }

        private void AddGridBackground()
        {
            GridBackground grid = new GridBackground();
            
            grid.StretchToParentSize(); //sets the size
            Insert(0, grid); //adds it to the graph view
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "EventEditor/ViewStyles.uss",
                "EventEditor/NodeStyle.uss"
            );
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new();

            ports.ForEach(port => {
                if(port.direction != startPort.direction) {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public Vector2 getLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false) {
            Vector2 worldMousePosition = mousePosition;
            if(searchWindow) {
                worldMousePosition -= editorWindow.position.position;
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }
    }
}
