using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor;
using Editor.EventEditor.Elements;
using Editor.EventEditor.Utilities;
using System.Runtime.CompilerServices;

namespace Editor.EventEditor.Windows
{
    public class EventGraphView : GraphView {
        
        private GraphSearchWindow searchWindow;
        private EventEditorWindow editorWindow;

        public EntryNode entryNode;
        public EventGraphView(EventEditorWindow editorWindow) {
            this.editorWindow = editorWindow;   

            AddManipulators();
            AddGridBackground();
            AddStyles();
            AddSearchWindow();

            AddEntryExitNodes();
        }

        private void AddEntryExitNodes()
        {
            entryNode = (EntryNode)CreateNode(NodeType.EntryNode, new Vector2(100, 300));
            AddElement(entryNode);
            AddElement(CreateNode(NodeType.ExitNode, new Vector2(500, 300)));
        }

        #region Context Menu
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
            /*
            this.AddManipulator(CreateContextualMenu("Add Slide", NodeType.Slide));
            this.AddManipulator(CreateContextualMenu(""))
            

            this.AddManipulator(CreateGroupContextualMenu());*/
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


        private IManipulator CreateContextualMenu(string actionTitle, NodeType type)
        {
            ContextualMenuManipulator contextualMenuManipulator = new(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(type, getLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }


        #endregion

        #region Create Element Functions 
        public GraphElement CreateGroup(string title, Vector2 localMousePosition)
        {
            Group group = new Group()
            {
                title = title
            };
            group.SetPosition(new Rect(localMousePosition, Vector2.zero));

            foreach (GraphElement selectedElement in selection)
            {
                if (selectedElement is BaseNode)
                {
                    BaseNode node = selectedElement as BaseNode;
                    group.AddElement(node);
                }
            }

            return group;
        }

        public BaseNode CreateNode(NodeType type, Vector2 pos)
        {
            BaseNode node = GetNode(type);

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
        #endregion

        #region Helpers
        private BaseNode GetNode(NodeType type)
        {
            switch (type)
            {
                case NodeType.Slide     : return new SlideNode();
                case NodeType.EntryNode : return new EntryNode();
                case NodeType.ExitNode  : return new ExitNode();
                case NodeType.AddGoldAction: return new NodeAction_AddGold();
                default: return null;
            }
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
                if (port.direction != startPort.direction)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        public Vector2 getLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;
            if (searchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
            return localMousePosition;
        }

        #endregion
    }
}
