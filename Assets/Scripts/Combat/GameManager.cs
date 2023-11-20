using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Commands;
using UnityEditor.PackageManager;

namespace Combat {
    public delegate void OnCommandHandler(ICommand command);

    public class GameManager : Shizounu.Library.SingletonBehaviour<GameManager> {

        public Board currentBoard;
        public event OnCommandHandler onCommand;   

        protected override void Awake() {
            base.Awake();

            currentBoard = new Board(onCommand);
        }
    }
}
