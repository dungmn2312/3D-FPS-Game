using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombiePatrolState : StateMachineBehaviour
{

    float timer;
    public float patrolingTime = 10f;

    Transform player;
    NavMeshAgent navMesh;

    public float detectionAreaRadius = 18f;
    public float patrolSpeed;

    List<Transform> wayPointsList = new List<Transform>();

    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMesh = animator.GetComponent<NavMeshAgent>();

        patrolSpeed = animator.GetComponent<Zombie>().walkSpeed;
        navMesh.speed = patrolSpeed;
        timer = 0;

        // Lấy tất cả waypoint và di chuyển tới điểm đầu tiên

        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoint");
        foreach(Transform t in waypointCluster.transform)
        {
            wayPointsList.Add(t);
        }

        Vector3 nextPosition = wayPointsList[UnityEngine.Random.Range(0, wayPointsList.Count)].position;
        navMesh.SetDestination(nextPosition);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!SoundManager.Instance.zombieChannel_1.isPlaying)
        {
            if (!SoundManager.Instance.zombieChannel_1.isPlaying)
            {
                SoundManager.Instance.zombieChannel_1.clip = SoundManager.Instance.zombieWalking[
                    UnityEngine.Random.Range(0, SoundManager.Instance.zombieWalking.Count)];
                SoundManager.Instance.zombieChannel_1.PlayDelayed(1f);
            }
            
        }

        // Nếu tới waypoint, sẽ di chuyển tới điểm tiếp theo
        if (navMesh.remainingDistance <= navMesh.stoppingDistance)
        {
            navMesh.SetDestination(wayPointsList[UnityEngine.Random.Range(0, wayPointsList.Count)].position);
        }

        // Transition Idle State
        timer += Time.deltaTime;
        if (timer > patrolingTime/* || animator.GetCurrentAnimatorStateInfo(0).IsName("DAMAGE")*/)
        {
            animator.SetBool("isPatroling", false);
            animator.SetBool("isIdling", true);
        }

        // Transition Chase State
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
            animator.SetBool("isPatroling", false);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navMesh.SetDestination(navMesh.transform.position);
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
