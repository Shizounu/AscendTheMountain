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
                new SearchTreeEntry(new GUIContent("Single Choice", indentationIcon))
                {
                    level = 2,
                    userData = Map.Events.Enumeration.SlideType.SingleChoice
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                {
                    level = 2,
                    userData = Map.Events.Enumeration.SlideType.MultipleChoice
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
                case Map.Events.Enumeration.SlideType.SingleChoice:
                    {
                        SingleChoiceEventNode node = (SingleChoiceEventNode)graphView.CreateNode(Map.Events.Enumeration.SlideType.SingleChoice, localPos);
                        graphView.AddElement(node);
                        return true;
                    }
                case Map.Events.Enumeration.SlideType.MultipleChoice:
                    {
                        MultipleChoiceEventNode node = (MultipleChoiceEventNode)graphView.CreateNode(Map.Events.Enumeration.SlideType.MultipleChoice, localPos);
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
