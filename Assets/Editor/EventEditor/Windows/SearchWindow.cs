using Editor.EventEditor.Elements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor.EventEditor.Windows
{
    public class GraphSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        public void Initialize(EventGraphView graphView)
        {
            this.graphView = graphView;
            indentationIcon = new Texture2D(1,1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }
        private EventGraphView graphView;
        private Texture2D indentationIcon;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
                new SearchTreeEntry(new GUIContent("Slide Node", indentationIcon))
                {
                    level = 2,
                    userData = NodeType.Slide
                },
                
                new SearchTreeGroupEntry(new GUIContent("Actions"), 2),
                new SearchTreeEntry(new GUIContent("Add Gold", indentationIcon))
                {
                    level= 3,
                    userData = NodeType.AddGoldAction
                },
                new SearchTreeEntry(new GUIContent("Remove Gold", indentationIcon))
                {
                    level= 3,
                    userData = NodeType.RemoveGoldAction
                },
                new SearchTreeEntry(new GUIContent("Add Health", indentationIcon))
                {
                    level= 3,
                    userData = NodeType.AddHealthAction
                },
                new SearchTreeEntry(new GUIContent("Remove Health", indentationIcon))
                {
                    level= 3,
                    userData = NodeType.RemoveHealthAction
                },


                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };
            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localPos = graphView.getLocalMousePosition(context.screenMousePosition, true);
            switch (SearchTreeEntry.userData) 
            {
                case NodeType.Slide:
                    {
                        SlideNode node = (SlideNode)graphView.CreateNode(NodeType.Slide, localPos);
                        graphView.AddElement(node);
                        return true;
                    }
                case NodeType.AddHealthAction:
                case NodeType.RemoveHealthAction:
                case NodeType.RemoveGoldAction:
                case NodeType.AddGoldAction:
                    {
                        BaseNode node = graphView.CreateNode((NodeType)SearchTreeEntry.userData, localPos);
                        graphView.AddElement(node);
                        return true;
                    }
                case Group _:
                    {
                        Group group = (Group)graphView.CreateGroup("DialogueGroup", localPos);
                        graphView.AddElement(group);
                        return true;
                    }
                default : return false;
            }
        }
    }
}
