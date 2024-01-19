using Cards;
using Combat;
using Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlayerActorManager))]
    public class PlayerManagerEditor : UnityEditor.Editor {

        

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
                return;

            //declarations
            PlayerActorManager actorManager = (PlayerActorManager)target;
            GUIStyle style = new GUIStyle();
            style.richText = true;


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("<color=gray><size=15><b>Debug Actions</b></size></color>", style);

            GUILayout.Toggle(actorManager.isEnabled, "Is Active");

            if (GUILayout.Button("Draw Card")) {
                GameManager.Instance.currentBoard.SetCommand(Command_DrawCard.GetAvailable().Init(Actors.Actor1));
            }

            EditorGUILayout.LabelField($"<color=cyan>{actorManager.deckInformation.CurManagems}</color> / <color=blue>{actorManager.deckInformation.MaxManagems}</color>", style);
            
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Add Max Mana", UnityEditor.EditorStyles.miniButtonLeft)) {
                GameManager.Instance.currentBoard.SetCommand(Command_ChangeMaxMana.GetAvailable().Init(Actors.Actor1, 1));
                
            }
            if(GUILayout.Button("Sub Max Mana", UnityEditor.EditorStyles.miniButtonRight)) {
                GameManager.Instance.currentBoard.SetCommand(Command_ChangeMaxMana.GetAvailable().Init(Actors.Actor1, -1));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Cur Mana", UnityEditor.EditorStyles.miniButtonLeft))
            {
                GameManager.Instance.currentBoard.SetCommand(Command_ChangeCurrentMana.GetAvailable().Init(Actors.Actor1, 1));
            }
            if (GUILayout.Button("Sub Cur Mana", UnityEditor.EditorStyles.miniButtonRight))
            {
                GameManager.Instance.currentBoard.SetCommand(Command_ChangeCurrentMana.GetAvailable().Init(Actors.Actor1, -1));
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Move true", UnityEditor.EditorStyles.miniButtonLeft))
            {
                for (int x = 0; x < GameManager.Instance.currentBoard.tiles.GetLength(0); x++) {
                    for (int y = 0; y < GameManager.Instance.currentBoard.tiles.GetLength(1); y++) {
                        if (GameManager.Instance.currentBoard.tiles[x,y].unit != null) {
                            GameManager.Instance.currentBoard.SetCommand(Command_SetCanMove.GetAvailable().Init(GameManager.Instance.currentBoard.tiles[x, y].unit, true));
                        }
                    }
                }
            }
            if (GUILayout.Button("Attack True", UnityEditor.EditorStyles.miniButtonRight))
            {
                for (int x = 0; x < GameManager.Instance.currentBoard.tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < GameManager.Instance.currentBoard.tiles.GetLength(1); y++)
                    {
                        if (GameManager.Instance.currentBoard.tiles[x, y].unit != null)
                        {
                            GameManager.Instance.currentBoard.SetCommand(Command_SetCanAttack.GetAvailable().Init(GameManager.Instance.currentBoard.tiles[x, y].unit, true));
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Kill All")) {
                for (int x = 0; x < GameManager.Instance.currentBoard.tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < GameManager.Instance.currentBoard.tiles.GetLength(1); y++)
                    {
                        if (GameManager.Instance.currentBoard.tiles[x, y].unit != null)
                        {
                            GameManager.Instance.currentBoard.SetCommand(Command_KillUnit.GetAvailable().Init(GameManager.Instance.currentBoard.tiles[x, y].unit));
                        }
                    }
                }
            }

            
            if(GUILayout.Button("Summon Testing Unit") && actorManager.testingDefinition != null) {
                GameManager.Instance.currentBoard.SetCommand(Command_SummonUnit.GetAvailable().Init(
                    actorManager.testingDefinition,
                    new Vector2Int(7, 2), 
                    Actors.Actor2)
                );
            }

            GameManager.Instance.currentBoard.DoQueuedCommands();

        }
    }
}


