using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance { get; set; }
    //[Header("Pool Properties")]
    //public int bulletPoolSize = 20;

    private Transform bulletSpawnTemp;

    [Header("Bullet")]
    public int bulletPoolSize = 20;
    private Queue<GameObject> bulletPool = new Queue<GameObject>();
    public GameObject bulletPrefab;
    

    [Header("Bloody Screen")]
    public int bloodyPoolSize = 5;
    private Queue<GameObject> bloodyScreenPool = new Queue<GameObject>();
    public GameObject bloodyScreenPrefab;
    public GameObject canvasBloodyScreen;

    [Header("Blood Spray")]
    public int bloodSprayPoolSize = 10;
    private Queue<GameObject> bloodSprayPool = new Queue<GameObject>();
    public GameObject bloodSprayPrefab;

    [Header("Zombie")]
    public int zombiePoolSize = 20;
    private Queue<GameObject> zombiePool = new Queue<GameObject>();
    public List<GameObject> zombiePrefabList;
    public GameObject zombieSpawnPosition;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        // Bullet
        InstantiateBullet();

        // BloodyScreen
        InstantiateBloodyScreen();

        // Zombie
        InstantiateZombie();

        // BloodSpray
        InstantiateBloodSpray();

    }

    public void InstantiateBullet()
    {
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, 0));
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public void InstantiateBloodyScreen()
    {
        for (int i = 0; i < bloodyPoolSize; i++)
        {

            GameObject bloodyScreen = Instantiate(bloodyScreenPrefab, canvasBloodyScreen.transform.position, Quaternion.identity);
            bloodyScreen.transform.SetParent(canvasBloodyScreen.transform, false);

            bloodyScreen.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
            bloodyScreen.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            bloodyScreen.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            bloodyScreen.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            bloodyScreen.SetActive(false);
            bloodyScreenPool.Enqueue(bloodyScreen);
        }
    }

    public void InstantiateZombie()
    {
        for (int i = 0; i < zombiePoolSize; i++)
        {
            Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            Vector3 spawnPosition = zombieSpawnPosition.transform.position + spawnOffset;

            GameObject zombiePrefab = zombiePrefabList[Random.Range(0, zombiePrefabList.Count)];
            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            GameObject zombieParent = GameObject.Find("Zombie");
            zombie.transform.SetParent(zombieParent.transform, true);
            zombie.SetActive(false);
            zombiePool.Enqueue(zombie);
        }
    }

    public void InstantiateBloodSpray()
    {
        for (int i = 0; i < bloodSprayPoolSize; i++)
        {
            GameObject bloodSpray = Instantiate(bloodSprayPrefab, transform.position, Quaternion.identity);
            
            bloodSpray.SetActive(false);
            bloodSprayPool.Enqueue(bloodSpray);
        }
    }

    public void ChangeBulletSpawnPosition(Transform bulletSpawn)
    {
        bulletSpawnTemp = bulletSpawn;
        foreach(GameObject bullet in bulletPool)
        {
            bullet.transform.SetParent(bulletSpawn.transform, false);
            bullet.transform.localPosition = Vector3.zero;
            
        }
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            print("Instantiate new bullet");
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, 0));
            bullet.transform.SetParent(bulletSpawnTemp.transform, false);
            bullet.transform.localPosition = bulletSpawnTemp.localPosition;
            bullet.SetActive(true);
            
            return bullet;
        }
    }

    public GameObject GetBloodyScreen()
    {
        if (bloodyScreenPool.Count > 0)
        {
            GameObject bloodyScreen = bloodyScreenPool.Dequeue();
            bloodyScreen.SetActive(true);
            return bloodyScreen;
        }
        else
        {
            GameObject bloodyScreen = Instantiate(bloodyScreenPrefab, canvasBloodyScreen.transform.position, Quaternion.identity);
            bloodyScreen.SetActive(true);
            return bloodyScreen;
        }
    }

    public GameObject GetZombie()
    {
        if (zombiePool.Count > 0)
        {
            GameObject zombie = zombiePool.Dequeue();
            zombie.SetActive(true);
            return zombie;
        }
        else
        {
            Vector3 spawnOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));
            Vector3 spawnPosition = zombieSpawnPosition.transform.position + spawnOffset;

            GameObject zombiePrefab = zombiePrefabList[Random.Range(0, zombiePrefabList.Count)];
            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            zombie.SetActive(true);
            return zombie;
        }
    }

    public GameObject GetBloodSpray()
    {
        if (bloodSprayPool.Count > 0)
        {
            GameObject bloodSpray = bloodSprayPool.Dequeue();
            bloodSpray.SetActive(true);
            return bloodSpray;
        }
        else
        {
            GameObject bloodSpray = Instantiate(bloodSprayPrefab, transform.position, Quaternion.identity);
            bloodSpray.SetActive(true);
            return bloodSpray;
        }
    }

    public void ReturnBloodyScreen(GameObject bloodyScreen)
    {
        bloodyScreen.SetActive(false);
        bloodyScreenPool.Enqueue(bloodyScreen);
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.localRotation = Quaternion.Euler(0, 0, 0);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
        
    }

    public void ReturnZombie(GameObject zombie)
    {
        zombie.SetActive(false);
        zombiePool.Enqueue(zombie);
        zombie.GetComponent<Collider>().enabled = true;
    }

    public void ReturBloodSpray(GameObject bloodSpray)
    {
        StartCoroutine(ReturnBloodSprayToPool(bloodSpray, 0.2f));
        
    }

    private IEnumerator ReturnBloodSprayToPool(GameObject bloodSpray, float delay)
    {
        
        yield return new WaitForSeconds(delay);
        bloodSpray.SetActive(false);
        bloodSprayPool.Enqueue(bloodSpray);
        bloodSpray.transform.localRotation = Quaternion.identity;
        
    }
}
