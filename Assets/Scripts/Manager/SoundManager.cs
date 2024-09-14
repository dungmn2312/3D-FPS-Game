using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("Weapon Channel")]
    public AudioSource shootingSoundChanel;

    public AudioClip simplePistolShooting;
    //public AudioSource reloadSoundSimplePistol;
    public AudioClip reloadSoundSimplePistol;
    public AudioSource emptyMagazineSoundSimplePistol;

    public AudioClip M416Shooting;
    //public AudioSource reloadSoundM416;
    public AudioClip reloadSoundM416;

    public AudioClip AK47Shooting;
    //public AudioSource reloadSoundAK47;
    public AudioClip reloadSoundAK47;

    [Header("Throwable Channel")]
    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;

    [Header("Zombie Channel")]
    public AudioSource zombieChannel_1;
    public AudioSource zombieChannel_2;

    public AudioClip zombieWalking_1;
    public AudioClip zombieWalking_2;
    public AudioClip zombieWalking_3;
    public AudioClip zombieWalking_4;
    public AudioClip zombieChase;
    public AudioClip zombieAttack_1;
    public AudioClip zombieAttack_2;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath_1;
    public AudioClip zombieDeath_2;

    public List<AudioClip> zombieWalking;
    public List<AudioClip> zombieAttacking;
    public List<AudioClip> zombieDeath;

    [Header("Player Channel")]
    public AudioSource playerChannel;
    public AudioClip playerHurt;
    public AudioClip playerDeath;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        InstantiateZombieSoundList();
    }

    private void InstantiateZombieSoundList()
    {
        zombieWalking = new List<AudioClip>();
        zombieWalking.Add(zombieWalking_1);
        zombieWalking.Add(zombieWalking_2);
        zombieWalking.Add(zombieWalking_3);
        zombieWalking.Add(zombieWalking_4);

        zombieAttacking = new List<AudioClip>();
        zombieAttacking.Add(zombieAttack_1);
        zombieAttacking.Add(zombieAttack_2);

        zombieDeath = new List<AudioClip>();
        zombieDeath.Add(zombieDeath_1);
        zombieDeath.Add(zombieDeath_2);

    }

    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.SimplePistol:
                shootingSoundChanel.PlayOneShot(simplePistolShooting);
                break;
            case WeaponModel.M416:
                shootingSoundChanel.PlayOneShot(M416Shooting);
                break;
            case WeaponModel.AK47:
                shootingSoundChanel.PlayOneShot(AK47Shooting);
                break;
        }
    }
    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.SimplePistol:
                //reloadSoundSimplePistol.Play();
                shootingSoundChanel.PlayOneShot(reloadSoundSimplePistol);
                break;
            case WeaponModel.M416:
                //reloadSoundM416.Play();
                shootingSoundChanel.PlayOneShot(reloadSoundM416);
                break;
            case WeaponModel.AK47:
                //reloadSoundAK47.Play();
                shootingSoundChanel.PlayOneShot(reloadSoundAK47);
                break;
        }
    }

}
