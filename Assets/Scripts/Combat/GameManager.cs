using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Commands;
using UnityEditor.PackageManager;

namespace Combat {
    public delegate void OnCommandHandler(ICommand command);

    public class GameManager : Shizounu.Library.SingletonBehaviour<GameManager> {
        [Header("Actor References")]
        public IActorManager player;
        public IActorManager enemy;


        public Board currentBoard;
        private Board rootBoard;

        protected override void Awake() {
            base.Awake();

            currentBoard = new Board();
        }

        private void Start()
        {
            player.Init();
            enemy.Init();

            rootBoard = currentBoard.GetCopy();
        }

        private void OnDestroy()
        {
            currentBoard = null;
        }

        

        public Board GetRootBoardCopy() {
            return rootBoard.GetCopy();
        }
    }
}
