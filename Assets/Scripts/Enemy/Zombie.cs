using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    public float zombieHP = 100f;

    public ZombieHand zombieHand;

    public float zombieBaseDamage = 25f;

    private void Start()
    {
        zombieHand.damage = zombieBaseDamage;
    }

    public override void TriggerDieAnimator(Animator animator, string dieAnimator)
    {
        animator.SetTrigger(dieAnimator);
    }

    public override void MakeDieSound(int indexDieRandom)
    {
        if (indexDieRandom == 0)
        {
            SoundManager.Instance.zombieChannel_2.PlayOneShot(SoundManager.Instance.zombieDeath_1);
        }
        else
        {
            SoundManager.Instance.zombieChannel_2.PlayOneShot(SoundManager.Instance.zombieDeath_2);
        }
    }

    public override void MakeHurtSound()
    {
        SoundManager.Instance.zombieChannel_1.PlayOneShot(SoundManager.Instance.zombieHurt);
    }
}
