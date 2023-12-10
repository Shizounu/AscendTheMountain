using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///TODO: Possibly put under combat
namespace Commands
{
    public interface ICommand
    {
        void Execute(Combat.Board board);
        void Unexecute(Combat.Board board);
    }
    public interface IVisualCommand
    {
        void Visuals(BoardRenderer boardRenderer);
    }
}


