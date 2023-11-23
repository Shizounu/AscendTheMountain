using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand_EndTurn : MonoBehaviour
{
    public void onButtonClick() {
        if (PlayerActorManager.Instance.isEnabled) {
            GameManager.Instance.currentBoard.SetCommand(new Commands.Command_SwitchSide(Actors.Actor1));
            GameManager.Instance.currentBoard.DoQueuedCommands();
        }
    }
}
