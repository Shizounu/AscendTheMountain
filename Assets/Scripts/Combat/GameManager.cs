using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Commands;
using UnityEditor.PackageManager;

namespace Combat {
    public delegate void OnCommandHandler(ICommand command);

    public class GameManager : Shizounu.Library.SingletonBehaviour<GameManager> {

        public Board currentBoard;
        private Board rootBoard;

        protected override void Awake() {
            base.Awake();

            currentBoard = new Board();
        }

        private void OnDestroy()
        {
            currentBoard = null;
        }

        /// <summary>
        /// Will only trigger on second call
        /// Done to solve a problem with race conditions and having to figure out what is actually the root board
        /// </summary>
        bool isFirst = true;
        public void InitRootBoard() {
            if (isFirst) {
                isFirst = false;
                return;
            }
            rootBoard = new Board(currentBoard);
        }

        public Board GetRootBoardCopy() {
            return new Board(rootBoard);
        }
    }
}
