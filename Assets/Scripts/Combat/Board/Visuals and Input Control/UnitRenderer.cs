using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class UnitRenderer : MonoBehaviour
{
    [Header("Anim values")]
    public float moveTime = 1f;

    [Header("references")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Initialize(RuntimeAnimatorController runtimeAnimatorController)
    {
        animator.runtimeAnimatorController = runtimeAnimatorController;
    }

    public Coroutine animRef;
    public IEnumerator Move(Vector2Int moveToPosition) {
        Vector3 startPos = transform.position;
        animator.SetBool("IsWalking", true);
        for (int i = 0; i < 60; i++) {
            transform.position = Vector3.Lerp(startPos, BoardRenderer.Instance.tiles[moveToPosition.x, moveToPosition.y].transform.position, i / 60f);
            yield return new WaitForSeconds(moveTime / 60f);
        }
        animator.SetBool("IsWalking", false);
        animRef = null;
    }

    public void OnDamage()
    {
        animator.SetTrigger("OnHit");
    }

    public bool isPlayingDeathAnim {
        get
        {
            string animName = animator.runtimeAnimatorController.name.Replace("_Animator", "_death");
            //Debug.Log($"{animator.GetCurrentAnimatorStateInfo(0).shortNameHash}, {Animator.StringToHash(animName)}, {animName}");
            return !animator.GetCurrentAnimatorStateInfo(0).IsName(animName) && animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
        }
    }
    public void OnDeath()
    {
        animator.SetTrigger("OnDeath");
    }


}
