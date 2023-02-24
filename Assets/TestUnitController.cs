using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnitController : MonoBehaviour
{
    public Animator animator;

    private void Start() {
        
    }
    [ContextMenu("Attack")]
    private void playAttackAnimation(){
        animator.SetTrigger("OnAttack");
    }
    [ContextMenu("Start Walk")]
    private void startWalking(){
        animator.SetBool("IsWalking", true);
    }
    [ContextMenu("End Walk")]
    private void endWalking(){
        animator.SetBool("IsWalking", false);
    }
    [ContextMenu("Take Damage")]
    private void playDamageAnimation(){
        animator.SetTrigger("OnTakeDamage");
    }
    [ContextMenu("Damage to Death")]
    private void takeDamageToDeath(){
        animator.SetTrigger("OnTakeDamage");
        animator.SetTrigger("OnDeath");
    }
}
