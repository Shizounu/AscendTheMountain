using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Commands;
using UnityEditor.PackageManager;

namespace Combat {
    public delegate void OnCommandHandler(ICommand command);
    public class GameManager : Shizounu.Library.SingletonBehaviour<GameManager> {
        [Header("Actor References")]
        public PlayerActorManager player;
        public AIActorManager enemy;

        public BoardRenderer boardRenderer;
        public Board currentBoard;
        private Board rootBoard;

        protected override void Awake() {
            base.Awake();

            currentBoard = new Board();
            currentBoard.onCommand += SideEnableCommand;
        }

        private void Start()
        {
            player.Init();
            enemy.Init();


            rootBoard = currentBoard.GetCopy();
            //currentBoard.onCommand += boardRenderer.ProcessCommand;
        }

        private void OnDestroy()
        {
            currentBoard = null;
            rootBoard = null;
        }

        public void SideEnableCommand(ICommand command) {
            if(command.GetType() == typeof(Command_SetEnable)) {
                if (((Command_SetEnable)command).side == Actors.Actor1) {
                    if(((Command_SetEnable)command).val) 
                        player.Enable();
                    else 
                        player.Disable();
                } else {
                    if (((Command_SetEnable)command).val)
                        enemy.Enable();
                    else
                        enemy.Disable();
                }


            }
        }

        

        public Board GetRootBoardCopy() {
            return rootBoard.GetCopy();
        }
    }
}
