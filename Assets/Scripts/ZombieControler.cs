using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieControler : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    Animator animator;
    NavMeshAgent agent;
    GameObject[] humans;
    GameObject currentTarget;
    bool isDead = false;
    public Action<ZombieControler> Died;

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
        isDead = true;
        tag = "Eliminated";
        if (!agent.isOnNavMesh)
            Die();
        agent.isStopped = true;
        animator.SetTrigger("Die");
        
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        Died?.Invoke(this);
        this.enabled = false;


    }
    void GetTargets()
    {
        humans = null;
        humans = GameObject.FindGameObjectsWithTag("Playable");
        
    }
    void SelectTarget()
    {
        if (humans.Length > 0)
        {
            foreach (GameObject target in humans)
            {
                if (currentTarget == null)
                {
                    currentTarget = target;
                }
                else
                {
                    float currentDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
                    float newDistance = Vector3.Distance(transform.position, target.transform.position);
                    if (newDistance < currentDistance)
                    {
                        currentTarget = target;
                    }
                }
            }
        }
        else
        {
            currentTarget = null;
        }
        
    }
    private void Update()
    {
        if (!isDead && agent.isOnNavMesh)
        {
            GetTargets();
            SelectTarget();
            if (currentTarget != null)
            {
                
                if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1f)
                {
                    animator.SetBool("Walking", false);
                    agent.isStopped = true;
                    if (currentTarget.tag == "Playable")
                    {
                        animator.SetTrigger("Attack");
                    }
                    else
                    {
                        GetTargets();
                        SelectTarget();
                    }
                        
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = moveSpeed / 2;
                    agent.SetDestination(currentTarget.transform.position);
                    animator.SetBool("Walking", true);
                }

            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("Walking", false);
            }
        }
        
    }
    public void OnHit()
    {
        Debug.Log("Zombie hit");
        if (currentTarget != null)
        {
            PlayableControler pc = currentTarget.GetComponent<PlayableControler>();
            if (pc != null && Vector3.Distance(transform.position, pc.transform.position)<2f)
            {
                pc.ReciveHit();
            }
        }
        GetTargets();
        SelectTarget();

    }

}


