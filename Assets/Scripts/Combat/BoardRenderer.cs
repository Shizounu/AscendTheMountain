using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Combat;

public class BoardRenderer : MonoBehaviour
{
    public Queue<IVisualCommand> visualCommands;

    private void Awake()
    {
        GameManager.Instance.OnCommandExecute += ProcessCommand;
    }
    public void ProcessCommand(ICommand command) {
        IVisualCommand visualCommand = GetVisualCommand(command);
        if (visualCommand == null)
            return;
        visualCommands.Enqueue(visualCommand); 
    }
    public IVisualCommand GetVisualCommand(ICommand command) {
        return (command is IVisualCommand) ? (IVisualCommand)command : null;
    }
}
