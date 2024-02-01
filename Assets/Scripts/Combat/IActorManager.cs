using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public interface IActorManager
    {
        public bool isEnabled { get; }
        public abstract DeckInformation deckInformation { get; }

        void Init();
        void Enable();
        void Disable();
    }
}
