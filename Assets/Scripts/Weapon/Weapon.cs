using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Status")]
    // Check if weapon on hand
    public bool isActiveWeapon;
    public int weaponDamage;
    

    [Header("Shooting")]
    // Shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    [Header("Burst")]
    // Burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    [Header("Spread")]
    // Spread
    private float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntensity;

    [Header("Bullet")]
    // Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 100;
    public float bulletPrefabLifeTime = 1f;

    [Header("Reloading")]
    // Reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    [Header("Save Weapon Status")]
    // Save Position and Rotation of Weapon on the rack
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    public Vector3 spawnScale;

    public ShootingMode currentShootingMode;

    public GameObject muzzleEffect;

    // internal: Allow every script can access
    internal Animator animator;

    private bool isADS;

    public enum WeaponModel
    {
        SimplePistol,
        Uzi,
        M416,
        AK47,
        AutoShotgun,
        Barret
    }

    public WeaponModel currentWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }


    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        spreadIntensity = hipSpreadIntensity;
        

    }
    // Update is called once per frame
    void Update()
    {
        // Click chuột trái để bắn
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    FireWeapon();
        //}

        if (isActiveWeapon)
        {

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                EnterADS();
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                ExitADS();
            }

            if (isShooting && bulletsLeft == 0)
            {
                SoundManager.Instance.emptyMagazineSoundSimplePistol.Play();
            }

            if (currentShootingMode == ShootingMode.Auto)
            {
                // Giữ chuột trái để bắn tự động
                isShooting = Input.GetKey(KeyCode.Mouse0);
                //shootingDelay = 0.1f;
            }
            else if (currentShootingMode == ShootingMode.Single)
            {
                // Click chuột trái 1 lần để bắn
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
                //shootingDelay = 0.5f;
            }
            else
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
                //shootingDelay = 0.1f;
            }

            if (readyToShoot && isShooting && bulletsLeft > 0 && !isReloading)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && WeaponManager.Instance.CheckTotalAmmoLeft(currentWeaponModel) > 0 && !isADS)
            {
                Reload();
            }

            //if (readyToShoot && !isShooting && !isReloading && bulletsLeft <= 0)
            //{
            //    Reload();
            //}

            
        }
    }

    private void EnterADS()
    {
        animator.SetTrigger("enterADS");
        isADS = true;
        HUDManager.Instance.crossHair.gameObject.SetActive(false);
        spreadIntensity = adsSpreadIntensity;
    }

    private void ExitADS()
    {
        animator.SetTrigger("exitADS");
        isADS = false;
        HUDManager.Instance.crossHair.gameObject.SetActive(true);
        spreadIntensity = hipSpreadIntensity;
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (!isADS)
        {
            animator.SetTrigger("RECOIL");
            Recoil();
        }
        else
        {
            animator.SetTrigger("ADS_RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(currentWeaponModel);

        readyToShoot = false;
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Khởi tạo đạn
        // Quaternion.identity: viên đạn sinh ra không bị quay độ nào
        //GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        // lệnh được chuyển sang ObjectPooling

        GameObject bullet = ObjectPool.instance.GetBullet();

        bullet.GetComponent<Bullet>().bulletDamage = weaponDamage;
        // 
        bullet.transform.forward = shootingDirection;

        // Bắn
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        
        // Đạn biến mất sau 1 khoảng thời gian
        //StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
        // ObjectPooling ko cần destroy bullet

        // Kiểm tra khi đã bắn xong
        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Burst Mode
        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }

        //print(spreadIntensity);
    }

    private void Recoil()
    {
        
        HUDManager.Instance.crossHair.targetSize += 30f;
    }

    private void Reload()
    {
        isReloading = true;
        SoundManager.Instance.PlayReloadSound(currentWeaponModel);
        if (currentWeaponModel == WeaponModel.M416 || currentWeaponModel == WeaponModel.AK47 || currentWeaponModel == WeaponModel.SimplePistol)
        {
            animator.SetTrigger("RELOAD");
        }
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        if (WeaponManager.Instance.CheckTotalAmmoLeft(currentWeaponModel) + bulletsLeft >= magazineSize)
        {
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft, magazineSize, currentWeaponModel);
            bulletsLeft = magazineSize;
        }
        else
        {
            int temp = bulletsLeft;
            bulletsLeft += WeaponManager.Instance.CheckTotalAmmoLeft(currentWeaponModel);
            WeaponManager.Instance.DecreaseTotalAmmo(temp, magazineSize, currentWeaponModel);
        }

        isReloading = false;
    } 

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        // Bắn từ giữa màn hình để kiểm tra xem mình đang chỉ vào đâu
        // 0.5 là ở giữa trục X và trục Y tức là giữa màn hình
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        // Nếu tia bắn chạm vào một vật thể, đặt targetPoint là điểm chạm.
        // Nếu không, đặt targetPoint là một điểm xa trong không trung (100 đơn vị).
        if (Physics.Raycast(ray, out hit))
        {
            // Bắn vào cái gì đó
            targetPoint = hit.point;
        }
        else
        {
            // Bắn vào không trung
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
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
