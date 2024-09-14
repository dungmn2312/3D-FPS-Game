using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAttackState : StateMachineBehaviour
{

    Transform player;
    NavMeshAgent navMesh;

    public float stopAttackingDistance = 4f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMesh = animator.GetComponent<NavMeshAgent>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!SoundManager.Instance.zombieChannel_1.isPlaying)
        {
            if (!SoundManager.Instance.zombieChannel_1.isPlaying)
            {
                SoundManager.Instance.zombieChannel_1.PlayOneShot(SoundManager.Instance.zombieAttacking[
                    UnityEngine.Random.Range(0, SoundManager.Instance.zombieAttacking.Count)]);
                
            }

        }

        LookAtPlayer();

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isChasing", true);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - navMesh.transform.position;
        navMesh.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = navMesh.transform.eulerAngles.y;
        navMesh.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
