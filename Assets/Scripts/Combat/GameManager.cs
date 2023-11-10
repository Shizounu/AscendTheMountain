using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Combat;

public delegate void OnCommandExecuteHandler(ICommand command);

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
        OnCommandExecute += currentBoard.SetCommand;
        OnCommandExecute += _ => currentBoard.DoQueuedCommands();
    }


    public Board currentBoard;    
    public event OnCommandExecuteHandler OnCommandExecute;

    public void ExecuteCommand(ICommand command) {
        OnCommandExecute.Invoke(command);        
    }
}
