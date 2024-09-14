using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.AI;

public class ZombieChaseState : StateMachineBehaviour
{

    NavMeshAgent navMesh;
    Transform player;

    public float chaseSpeed;

    public float stopChasingDistance = 21f;
    public float attackingDistance = 4f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMesh = animator.GetComponent<NavMeshAgent>();

        chaseSpeed = animator.GetComponent<Zombie>().chaseSpeed;
        navMesh.speed = chaseSpeed;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!SoundManager.Instance.zombieChannel_1.isPlaying)
        {
            if (!SoundManager.Instance.zombieChannel_1.isPlaying)
            {
                SoundManager.Instance.zombieChannel_1.PlayOneShot(SoundManager.Instance.zombieChase);
                
            }

        }

        navMesh.SetDestination(player.position);
        animator.transform.LookAt(player);

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
            animator.SetBool("isPatroling", true);
        }
        
        if (distance <= attackingDistance)
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("isChasing", false);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navMesh.SetDestination(animator.transform.position);

        SoundManager.Instance.zombieChannel_1.Stop();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
