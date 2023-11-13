using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Commands;

namespace Combat
{
    public interface IEffect : ICommand { }

    /// <summary>
    /// Interface for Unit Effect for phase start trigger
    /// </summary>
    public interface ITrigger_OnPhaseStart : IEffect {
        
    }

    /// <summary>
    /// Interface for Unit Effect for phase end trigger
    /// </summary>
    public interface ITrigger_OnPhaseEnd : IEffect {

    }
}
