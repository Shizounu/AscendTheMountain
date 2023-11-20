using Combat;
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

            if (GUILayout.Button("Draw Card")) {
                GameManager.Instance.currentBoard.SetCommand(new Commands.Command_DrawCard(Actors.Actor1));
            }

            EditorGUILayout.LabelField($"<color=cyan>{actorManager.deckInformation.CurManagems}</color> / <color=blue>{actorManager.deckInformation.MaxManagems}</color>", style);
            
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Add Max Mana", UnityEditor.EditorStyles.miniButtonLeft)) {
                GameManager.Instance.currentBoard.SetCommand(new Commands.Command_AddMaxMana(Actors.Actor1, 1));
                
            }
            if(GUILayout.Button("Sub Max Mana", UnityEditor.EditorStyles.miniButtonRight)) {
                GameManager.Instance.currentBoard.SetCommand(new Commands.Command_SubMaxMana(Actors.Actor1, 1));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Cur Mana", UnityEditor.EditorStyles.miniButtonLeft))
            {
                GameManager.Instance.currentBoard.SetCommand(new Commands.Command_AddCurrentMana(Actors.Actor1, 1));
            }
            if (GUILayout.Button("Sub Cur Mana", UnityEditor.EditorStyles.miniButtonRight))
            {
                GameManager.Instance.currentBoard.SetCommand(new Commands.Command_SubCurrentMana(Actors.Actor1, 1));
            }
            GUILayout.EndHorizontal();

            GameManager.Instance.currentBoard.DoQueuedCommands();

        }
    }
}


