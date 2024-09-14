using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponSlots;
    // Súng hiện tại đang cầm
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables General")]

    public float throwForce = 10f;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0;
    private float forceMultiplierLimit = 2;

    [Header("Lethals")]
    public GameObject grenadePrefab;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;
    private int maxLethalsAmount = 2;

    [Header("Tacticals")]
    public GameObject smokePrefab;
    public int tacticalsCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    private int maxTacticalsAmount = 3;



    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        // Chưa có bom thì loại lethal hiện tại sẽ None
        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;
    }

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
    }

    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            if (weaponSlots[i] == activeWeaponSlot)
            {
                weaponSlots[i].SetActive(true);
            }
            else
            {
                weaponSlots[i].SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1); 
        }

        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.J))
        {
            if (forceMultiplier < forceMultiplierLimit)
            {
                forceMultiplier += Time.deltaTime;
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (lethalsCount > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0;
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
            if (tacticalsCount > 0)
            {
                ThrowTactical();
            }

            forceMultiplier = 0;
        }

    }

    public void PickUpWeapon (GameObject pickedUpWeapon)
    {
        //Destroy(pickedUpWeapon);
        AddWeaponIntoActiveSlot(pickedUpWeapon);
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedUpWeapon)
    {
        ChangeWeaponLayer(pickedUpWeapon, LayerMask.NameToLayer("WeaponRenderCamera"));

        Vector3 oldScale = pickedUpWeapon.transform.localScale;
        DropCurrentWeapon(pickedUpWeapon, oldScale);

        // Tham số false đảm bảo weapon giữ nguyên localtranform của nó
        // Tham số true giữ nguyen tranform so với world
        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedUpWeapon.GetComponent<Weapon>();

        pickedUpWeapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);
        pickedUpWeapon.transform.localScale = new Vector3(weapon.spawnScale.x, weapon.spawnScale.y, weapon.spawnScale.z);
        pickedUpWeapon.GetComponent<Collider>().enabled = false;

        weapon.isActiveWeapon = true;

        weapon.animator.enabled = true;

        ObjectPool.instance.ChangeBulletSpawnPosition(weapon.bulletSpawn);
        
    }

    private void DropCurrentWeapon(GameObject pickedUpWeapon, Vector3 oldScale)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {

            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;
            ChangeWeaponLayer(weaponToDrop, LayerMask.NameToLayer("Default"));

            weaponToDrop.GetComponent<Weapon>().isActiveWeapon = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;
            weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedUpWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedUpWeapon.transform.localRotation;
            weaponToDrop.transform.localScale = oldScale;
            weaponToDrop.GetComponent<Collider>().enabled = true;
        }
    }

    private void ChangeWeaponLayer(GameObject weaponToChange, int newLayer)
    {
        weaponToChange.layer = newLayer;
        foreach (Transform child in weaponToChange.transform)
        {
            ChangeWeaponLayer(child.gameObject, newLayer);
        }
    }

    // Đổi vũ khí thì đổi active của activeSlot
    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActiveWeapon = false;
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            newWeapon.isActiveWeapon = true;

            ObjectPool.instance.ChangeBulletSpawnPosition(newWeapon.bulletSpawn);
        }

    }

    #region || ---- Ammo ---- ||
    internal void PickUpAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoPistolAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoRifleAmount;
                break;
        }
    }

    public int CheckTotalAmmoLeft(WeaponModel model)
    {
        switch (model)
        {
            case WeaponModel.M416:
                return totalRifleAmmo;
            case WeaponModel.AK47:
                return totalRifleAmmo;
            case WeaponModel.SimplePistol:
                return totalPistolAmmo;
            default:
                return 0;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsLeft, int magazineSize, WeaponModel currentWeaponModel)
    {
        switch (currentWeaponModel)
        {
            case Weapon.WeaponModel.M416:
                totalRifleAmmo -= (magazineSize - bulletsLeft);
                break;
            case Weapon.WeaponModel.AK47:
                totalRifleAmmo -= (magazineSize - bulletsLeft);
                break;
            case Weapon.WeaponModel.SimplePistol:
                totalPistolAmmo -= (magazineSize - bulletsLeft);
                break;
        }
        if (totalRifleAmmo < 0)
        {
            totalRifleAmmo = 0;
        }
        if (totalPistolAmmo < 0)
        {
            totalPistolAmmo = 0;
        }
    }
    #endregion

    #region || ---- Throwables ---- ||
    internal void PickUpThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickUpThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke:
                PickUpThrowableAsTactical(Throwable.ThrowableType.Smoke);
                break;
        }
    }

    private void PickUpThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if (lethalsCount < maxLethalsAmount)
            {
                lethalsCount++;
                // Destroy ở đây chứ k phải ở InteractionManager vì khi còn có thể nhặt bom mới Destroy
                // tránh trường hợp đạt giới hạn vẫn Destroy
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
        }
        else
        {
            print("Lethals limit reached");
        }
    }

    private void PickUpThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if (tacticalsCount < maxTacticalsAmount)
            {
                tacticalsCount++;
                // Destroy ở đây chứ k phải ở InteractionManager vì khi còn có thể nhặt bom mới Destroy
                // tránh trường hợp đạt giới hạn vẫn Destroy
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
        }
        else
        {
            print("Tacticals limit reached");
        }
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType equippedType)
    {
        switch (equippedType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke:
                return smokePrefab;
        }

        return new();
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        lethalsCount--;

        if (lethalsCount <= 0)
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);

        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        tacticalsCount--;

        if (tacticalsCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }
    #endregion
}
