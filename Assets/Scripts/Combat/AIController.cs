using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{

    [RequireComponent(typeof(SideManager))]
    public class AIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SideManager sideManager;
    }
}

