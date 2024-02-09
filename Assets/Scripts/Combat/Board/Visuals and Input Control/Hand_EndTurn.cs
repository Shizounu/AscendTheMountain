using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand_EndTurn : MonoBehaviour
{
    public void onButtonClick() {
        if (PlayerActorManager.Instance.isEnabled) {
            GameManager.Instance.currentBoard.SetCommand(Commands.Command_EndTurn.GetAvailable().Init(Actors.Actor2));
            GameManager.Instance.currentBoard.DoQueuedCommands();
        }
    }
}
