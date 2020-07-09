using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //Cache
    Animator animator;

    private void Start() 
    {
        animator = GetComponent<Animator>();
    }

    public void Die()
    {
        animator.SetTrigger("die");
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
