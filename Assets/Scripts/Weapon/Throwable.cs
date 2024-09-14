using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    // Thời gian chờ nổ
    [SerializeField] float delay = 3f;
    // Bán kính nổ
    [SerializeField] float damageRadius = 20f;
    // Lực nổ
    [SerializeField] float explosionForce = 1200f;

    // Sát thương bom
    float grenadeDamage = 100f;

    // Đếm ngược thời gian chờ nổ
    float countDown;

    // Nổ hay chưa
    bool hasExploded = false;
    // Ném hay chưa
    public bool hasBeenThrown = false;

    

    public enum ThrowableType
    {
        None,
        Grenade,
        FlashBang,
        Smoke
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countDown = delay;
        
    }

    private void Update()
    {
        // Nếu bom đã được ném thì giảm thời gian đếm ngược chờ nổ
        // Hết thời gian chờ và bom chưa nổ thì gọi hàm nổ
        if (hasBeenThrown)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0f && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        GetThrowableEffect();

        Destroy(gameObject);
    }

    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.Smoke:
                SmokeEffect();
                break;
            //case ThrowableType.FlashBang:
            //    FlashBangEffect();
            //    break;
            default:
                break;
        }
    }

    private void GrenadeEffect()
    {
        // Ngẫu nhiên nổ 1 trong 2 hiệu ứng
        int randomExplosionEffect = Random.Range(0, 2);

        GameObject explosionEffect;
        if (randomExplosionEffect == 0)
        {
            // Hiệu ứng nổ
            explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect_1;
        }
        else
        {
            explosionEffect = GlobalReferences.Instance.grenadeExplosionEffect_2;
        }
        
        Instantiate(explosionEffect, transform.position, transform.rotation);

        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        // Lực nổ vật lý

        // Xác định tất cả object trong phạm vi nổ
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }

            if (objectInRange.GetComponent<Zombie>())
            {
                objectInRange.GetComponent<Zombie>().TakeDamage(grenadeDamage, ref objectInRange.GetComponent<Zombie>().zombieHP, 0);
            }
        }
    }

    private void SmokeEffect()
    {
        GameObject smokeEffect = GlobalReferences.Instance.smokeEffect;
        
        Instantiate(smokeEffect, transform.position, transform.rotation);

        //SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.smokeSound);

        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider objectInRange in colliders)
        {
            Rigidbody rb = objectInRange.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Blindess enemy
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        readyToPickup = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        readyToPickup = false;
    //    }
    //}

}
