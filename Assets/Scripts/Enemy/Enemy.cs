using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    
    private Animator animator;

    private NavMeshAgent navMesh;

    public bool isDead;

    public float walkSpeed;
    public float chaseSpeed;

    public enum EnemyType
    {
        Zombie,
    }

    public EnemyType enemyType;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        isDead = false;
    }

    public virtual void TakeDamage(float damageAmout, ref float enemyHP, int criticalRate)
    {
        if (criticalRate == 3)
        {
            enemyHP -= (damageAmout * 1.5f);
        }
        else
        {
            enemyHP -= damageAmout;
        }
        if (enemyHP <= 0)
        {
            int indexDieRandom = Random.Range(0, 2);
            if (indexDieRandom == 0)
            {
                TriggerDieAnimator(animator, "DIE1");
                
            }
            else
            {
                TriggerDieAnimator(animator, "DIE2");
                
            }
            MakeDieSound(indexDieRandom);

            isDead = true;
            gameObject.GetComponent<Collider>().enabled = false;
            Invoke("ReturnZombieToPool", 5f);
        }
        else
        {
            if (criticalRate == 3)
            {
                animator.SetTrigger("DAMAGE");
                MakeHurtSound();
            }
        }
    }

    private void ReturnZombieToPool()
    {
        ObjectPool.instance.ReturnZombie(gameObject);
    }

    public abstract void TriggerDieAnimator(Animator animator, string dieAnimator);

    public abstract void MakeDieSound(int indexDieRandom);

    public abstract void MakeHurtSound();
    

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, 4f); // Attacking, Stop attacking

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawSphere(transform.position, 13f); // Start chasing

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(transform.position, 15f); // Stop chasing
    //}
}
