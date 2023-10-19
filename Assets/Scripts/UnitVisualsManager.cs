using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitVisualsManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Init(RuntimeAnimatorController animatorController) {
        animator.runtimeAnimatorController = animatorController;
    }
}
