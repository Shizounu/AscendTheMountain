using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Commands;

namespace Combat {
    public class GameManager {
        private static GameManager _instance;
        public static GameManager Instance { 
            get {
                if (_instance == null)
                    _instance = new GameManager();
                return _instance;
            }
        }

        private GameManager() {
            currentBoard = new Board();
            
        }

        public Board currentBoard;
    }
}
