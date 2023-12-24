using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

using Map.Events;
using Editor.EventEditor.Windows;
using Editor.EventEditor.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using Codice.CM.SEIDInfo;
using UnityEditor;
using System.IO;
using System.Text;
using System;

namespace Editor.EventEditor.Utilities
{
	public static class SerializationUtility {
		static string baseFileEnding = ".event";
		static string editorFileEnding = $"{baseFileEnding}.graph";


		public static void Save(string eventName,EventGraphView graphView, EntryNode entry) {
			SaveAsEvent(eventName, graphView, entry);




			AssetDatabase.Refresh();

        }
		
		private static void SaveAsEvent(string eventName, EventGraphView graphView, EntryNode entry) {
			List<MapEventSlide> slides = new();
			foreach (VisualElement elem in graphView.graphElements) {
                if(elem.GetType() == typeof(SlideNode))
					slides.Add(getSlideFromNode((SlideNode)elem));
            }

			string initialSlideID = ((SlideNode)((Port)entry.outputContainer.Children().ToList()[0]).connections.ToList()[0].input.node).ID;
			MapEvent mapEvent = new(eventName, slides, initialSlideID);

			Directory.CreateDirectory($"{Application.dataPath}/Events/{eventName}");
			using (FileStream stream = File.Create($"{Application.dataPath}/Events/{eventName}/{eventName}{baseFileEnding}")) {
				char[] text = mapEvent.GetJson().ToCharArray();
				stream.Write(Encoding.UTF8.GetBytes(text));
			}
        }

		private static MapEventSlide getSlideFromNode(SlideNode node) {
			List<MapEventAction> choices = new List<MapEventAction>();

            foreach (VisualElement visualElement in node.outputContainer.Children()) {
				Port port = visualElement as Port;

				string choiceText = "";  
				foreach (VisualElement element in port.Children())
					if (element.GetType() == typeof(TextField))
						choiceText = ((TextField)element).value;

				List<MapEventActionLogic> actions = new();

				BaseNode curNode = (BaseNode)port.connections.ToList()[0].input.node;
                while (curNode.GetType() != typeof(SlideNode) && curNode.GetType() != typeof(ExitNode))
                {
					actions.Add(curNode.getAction());
					//Under the definition that only slide nodes can have a branch
					curNode = ((Port)curNode.outputContainer.Children().ToList()[0]).connections.ToList()[0].input.node as BaseNode;
                    
                }
				actions.Add(curNode.getAction());
                choices.Add(new MapEventAction(choiceText, actions));
            }
			
            return new MapEventSlide(node.Text, choices, node.ID);
		}


	}

	
}