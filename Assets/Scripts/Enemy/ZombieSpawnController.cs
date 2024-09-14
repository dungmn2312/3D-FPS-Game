using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZombieSpawnController : MonoBehaviour
{
    // Lượng zombie được tạo trong 1 wave
    public int initialZombiesPerWave = 1;
    // Lượng zombie của wave hiện tại
    public int currentZombiesPerWave;
    
    public float spawnDelay = 1f;
    public bool isSpawning;
   
    public int currentWave = 0;
    // Thời gian đếm ngược đổi wave
    public float waveCoolDown = 10f;

    public bool inCoolDown;
    public float coolDownCounter = 0f;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI countDownUI;
    public TextMeshProUGUI waveNumberUI;

    public List<Zombie> currentZombiesAlive;

    public Zombie zombiePrefab;


    private void Start()
    {
        currentZombiesPerWave = initialZombiesPerWave;
        isSpawning = false;
        StartNextWave();
    }

    private void StartNextWave()
    {
        
        currentZombiesAlive.Clear();

        currentWave++;
        waveNumberUI.text = $"Wave: {currentWave}";

        GlobalReferences.Instance.bestWaveToSave = currentWave;

        StartCoroutine(SpawnWave());

    }

    private IEnumerator SpawnWave()
    {
        isSpawning = true;
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            Zombie zombie = ObjectPool.instance.GetZombie().GetComponent<Zombie>();
            currentZombiesAlive.Add(zombie);

            yield return new WaitForSeconds(spawnDelay);
        }
        isSpawning = false;
    }

    private void Update()
    {
        List<Zombie> zombiesToRemove = new List<Zombie>();
        
        foreach (Zombie zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        foreach (Zombie zombie in zombiesToRemove) {
            currentZombiesAlive.Remove(zombie);
        }

        zombiesToRemove.Clear();

        if (currentZombiesAlive.Count == 0 && !inCoolDown && !isSpawning)
        {
            StartCoroutine(WaveCoolDown());
        }

        if (inCoolDown)
        {
            coolDownCounter -= Time.deltaTime;
            countDownUI.text = coolDownCounter.ToString("F0");
        }
        else
        {
            coolDownCounter = waveCoolDown;
        }
    }

    private IEnumerator WaveCoolDown()
    {
        inCoolDown = true;
        waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCoolDown);

        waveOverUI.gameObject.SetActive(false);
        inCoolDown = false;

        currentZombiesPerWave += Convert.ToInt32(currentZombiesPerWave * 1.5);

        StartNextWave();
    }

}
