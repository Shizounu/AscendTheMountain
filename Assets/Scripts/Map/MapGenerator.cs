using System.Collections;
using System.Collections.Generic;
using Shizounu.Library.Editor.StateMachineEditor;
using Shizounu.Library.ScriptableArchitecture;
using UnityEngine;

namespace Map
{
    [System.Serializable]
    public struct MapLayerDefinition
    {
        public int minNodeCount;
        public int maxNodeCount;
        public float layerWidth;

        public List<WeightedNode> weightedNodes;
    }

    [System.Serializable]
    public struct WeightedNode
    {
        public float weight;
        public Node node; 
    }

    public class MapGenerator : MonoBehaviour
    {
        [Header("Generation Settings")]
        [SerializeField] private FloatReference mapHeight;
        [SerializeField] private List<MapLayerDefinition> mapDefinition;

        [Header("References")]
        private List<Node>[] generatedMap;
        public void GenerateMap()
        {
            GenerateNodes();
            GenerateConnections();
        }
        private void GenerateNodes() {
            Node getWeightedRandomNode(List<WeightedNode> weightedNodes){
                List<Node> nodes = new();
                foreach (WeightedNode node in weightedNodes)
                    for (int i = 0; i < node.weight; i++)
                        nodes.Add(node.node);

                return nodes[Random.Range(0, nodes.Count)];
            }


            generatedMap = new List<Node>[mapDefinition.Count];
            for (int currentLayer = 0; currentLayer < mapDefinition.Count; currentLayer++)
            {
                int nodesToGenerateForLayer = Random.Range(mapDefinition[currentLayer].minNodeCount, mapDefinition[currentLayer].maxNodeCount + 1);

                GameObject layerGroup = new GameObject($"Layer {currentLayer}");
                layerGroup.transform.parent = transform;

                generatedMap[currentLayer] = new List<Node>();

                for (int i = 0; i < nodesToGenerateForLayer; i++)
                {
                    Node newNode = Instantiate(getWeightedRandomNode(mapDefinition[currentLayer].weightedNodes), layerGroup.transform);
                    newNode.gameObject.name = $"Node : {i}";

                    if (currentLayer == 0)
                        newNode.isInitial = true;

                    float xPosition = -(mapDefinition[currentLayer].layerWidth / 2) + (mapDefinition[currentLayer].layerWidth * (i / Mathf.Max((float)(nodesToGenerateForLayer - 1), 1)));
                    float yPosition = -(mapHeight / 2) + (mapHeight * (currentLayer / Mathf.Max((float)(mapDefinition.Count - 1), 1)));
                    newNode.transform.position = new Vector3(xPosition, yPosition, 0);

                    generatedMap[currentLayer].Add(newNode);
                }
            }
        }
        private void GenerateConnections()
        {
            for (int i = 0; i < generatedMap.Length - 1; i++)
            {

                int remainingNodes = generatedMap[i + 1].Count;
                int startingPoint = 0;
                if (generatedMap[i].Count != 1){ 
                    //check if theres more than one node in the current layer
                    if (remainingNodes != 1)
                    {
                        //Handles connecting all nodes under normal rules
                        foreach (Node node in generatedMap[i])
                        {
                            int nodesToconnect = Random.Range(1, remainingNodes - startingPoint); //will never collect last node due to being exclusive

                            for (int j = 0; j < nodesToconnect; j++) //Connects all the nodes based on random chance
                                if (startingPoint + j < generatedMap[i + 1].Count)
                                    node.connectedNodes.Add(generatedMap[i + 1][startingPoint + j]);

                            if (startingPoint > 0 && startingPoint < generatedMap[i + 1].Count)
                            {
                                float flip = Random.Range(0f, 1f);
                                if (flip > 0.5f)
                                    node.connectedNodes.Add(generatedMap[i + 1][startingPoint - 1]);
                            }

                            if (startingPoint + nodesToconnect < generatedMap[i + 1].Count - 1)
                            {
                                float flip = Random.Range(0f, 1f);
                                if (flip > 0.5f)
                                    node.connectedNodes.Add(generatedMap[i + 1][startingPoint + nodesToconnect]);
                            }

                            if (node.connectedNodes.Count == 0)
                                node.connectedNodes.Add(generatedMap[i + 1][generatedMap[i + 1].Count - 1]);

                            startingPoint += nodesToconnect;
                        }

                        //Handles connecting last node of the current layer with the last node of the next layer
                        generatedMap[i][generatedMap[i].Count - 1].connectedNodes.Add(generatedMap[i + 1][generatedMap[i + 1].Count - 1]);
                    }
                    else
                        foreach (Node node in generatedMap[i]) // Special case for if next layer has only one node
                            node.connectedNodes.Add(generatedMap[i + 1][0]);
                    
                }
                else
                    foreach (Node node in generatedMap[i + 1])
                        generatedMap[i][0].connectedNodes.Add(node);
            }

            //removes unconnected nodes
            for (int i = 0; i < generatedMap.Length - 1; i++){
                List<Node> nextLayer = new(generatedMap[i+1]);
                foreach (Node node in generatedMap[i]) //removes nodes in next layer list if they are found to be connected
                {
                    foreach (Node connectedNode in node.connectedNodes)
                    {
                        nextLayer.Remove(connectedNode);    
                    }        
                }

                for (int j = 0; j < nextLayer.Count; j++){
                    generatedMap[i+1].Remove(nextLayer[j]);
                    Destroy(nextLayer[j].gameObject);
                    nextLayer.RemoveAt(j);
                }
            }

            for (int i = 0; i < generatedMap.Length; i++)
                foreach (Node node in generatedMap[i])
                    node.Init(node.connectedNodes);
            
        }

    }
}
