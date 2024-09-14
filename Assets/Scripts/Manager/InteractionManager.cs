using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Weapon;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null;
    public Throwable hoveredThrowable = null;

    private float pickupRange = 5f;

    private Transform playerPos;


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
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        playerPos = GameObject.FindWithTag("Player").transform;

        // Kiểm tra tia ray có chạm được vào vật gì hay không

        // Weapon
        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;

            if (objectHitByRaycast.GetComponent<Weapon>() && objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon == false)
            {
                hoveredWeapon = objectHitByRaycast.gameObject.GetComponent<Weapon>();
                float distance = Vector3.Distance(hoveredWeapon.transform.position, playerPos.position);
                if (distance <= pickupRange)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = true;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        hoveredWeapon.GetComponent<Outline>().enabled = false;
                        WeaponManager.Instance.PickUpWeapon(objectHitByRaycast.gameObject);
                    }
                }
            }
            else
            {
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }
            }

            // Ammo Box
            if (objectHitByRaycast.GetComponent<AmmoBox>())
            {
                hoveredAmmoBox = objectHitByRaycast.gameObject.GetComponent<AmmoBox>();

                float distance = Vector3.Distance(hoveredAmmoBox.transform.position, playerPos.position);
                if (distance <= pickupRange)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = true;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        WeaponManager.Instance.PickUpAmmo(hoveredAmmoBox);
                        Destroy(objectHitByRaycast.gameObject);
                    }
                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }
            }

            // Throwable
            if (objectHitByRaycast.GetComponent<Throwable>())
            {
                hoveredThrowable = objectHitByRaycast.gameObject.GetComponent<Throwable>();

                float distance = Vector3.Distance(hoveredThrowable.transform.position, playerPos.position);
                if (distance <= pickupRange)
                {
                    if (!hoveredThrowable.hasBeenThrown)
                    {
                        hoveredThrowable.GetComponent<Outline>().enabled = true;

                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            WeaponManager.Instance.PickUpThrowable(hoveredThrowable);
                            //Destroy(objectHitByRaycast.gameObject);
                        }
                    }

                }
            }
            else
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }
            }
        }
        //}
    }
}
