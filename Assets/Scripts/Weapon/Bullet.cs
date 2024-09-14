using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int bulletDamage;
    public float bulletLifeTime = 0.5f;

    

    private void OnEnable()
    {
        Invoke("ReturnBulletToPool", bulletLifeTime);
    }

    private void OnCollisionEnter(Collision objectHit)
    {

        switch (objectHit.gameObject.tag)
        {
            case "Target":
                //print("hit " + objectHit.gameObject.name + " !");
                CreateBulletImpactEffect(objectHit);
                ObjectPool.instance.ReturnBullet(gameObject);
                break;

            case "Wall":
                //print("hit a wall !");
                CreateBulletImpactEffect(objectHit);
                ObjectPool.instance.ReturnBullet(gameObject);
                break;

            case "BeerBottle":
                //print("hit a beer bottle !");
                objectHit.gameObject.GetComponent<BeerBottle>().Shatter();
                ObjectPool.instance.ReturnBullet(gameObject);
                break;

            case "Zombie":
                
                Zombie zombieHit = objectHit.gameObject.GetComponent<Zombie>();
                if (!zombieHit.isDead)
                {
                    int criticalRate = UnityEngine.Random.Range(0, 5);
                    zombieHit.TakeDamage(bulletDamage, ref zombieHit.zombieHP, criticalRate);
                    CreateBloodSprayEffect(objectHit);
                }
                ObjectPool.instance.ReturnBullet(gameObject);
                break;
            case "ZombieHead":
                GameObject zombieHeadHit = objectHit.gameObject;
                

                Zombie actuallyZombie = zombieHeadHit.gameObject.GetComponent<ZombieHead>().zombieParent.GetComponent<Zombie>();
                if (!actuallyZombie.isDead)
                {
                    int criticalRate = UnityEngine.Random.Range(0, 5);
                    actuallyZombie.TakeDamage(bulletDamage * 3, ref actuallyZombie.zombieHP, criticalRate);
                    CreateBloodSprayEffect(objectHit);
                }
                

                ObjectPool.instance.ReturnBullet(gameObject);
                break;

                //default:
                //    Invoke("ReturnBulletToPool", 2f);
                //    break;
        }
        
    }

    //private GameObject FindParentByName(GameObject childObject, string name)
    //{
    //    Transform currentParent = childObject.transform.parent;

    //    while (currentParent != null)
    //    {
    //        if (currentParent.name == name)
    //        {
    //            return currentParent.gameObject;
    //        }
    //        currentParent = currentParent.parent;
    //    }

    //    return null;
    //}

    private void ReturnBulletToPool()
    {
        if (gameObject.activeSelf)
        {
            ObjectPool.instance.ReturnBullet(gameObject);
        }
    }

    private void CreateBloodSprayEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject bloodSpray = ObjectPool.instance.GetBloodSpray();
        bloodSpray.transform.localPosition = contact.point;
        bloodSpray.transform.localRotation = Quaternion.LookRotation(contact.normal);

        //bloodSpray.transform.SetParent(objectHit.gameObject.transform);

        ObjectPool.instance.ReturBloodSpray(bloodSpray);
    }

    

    void CreateBulletImpactEffect(Collision objectHit)
    {
        // Lấy điểm tiếp xúc đầu tiên của va chạm
        ContactPoint contact = objectHit.contacts[0];

        //Hướng: Sử dụng Quaternion.LookRotation(contact.normal) để định hướng hiệu ứng theo hướng pháp tuyến của điểm tiếp xúc
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        // Đặt hiệu ứng va chạm là con của đối tượng bị bắn trúng
        hole.transform.SetParent(objectHit.gameObject.transform);
    }
}
