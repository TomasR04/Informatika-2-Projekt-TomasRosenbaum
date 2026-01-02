using UnityEngine;
using UnityEngine.AI;

public class ZombieControler : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    Animator animator;
    NavMeshAgent agent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

    }
    public void ReciveHit()
    {
        if (moveSpeed - 1 >= 0)
        {
            moveSpeed -= 1f;
        }
        else
        {
            Die();
        }
    }
    public void Die()
    {
        tag = "Eliminated";
        animator.SetTrigger("Die");
        agent.isStopped = true;

    }

}
