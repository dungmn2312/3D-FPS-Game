using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{
    public List<Rigidbody> allParts = new List<Rigidbody>();

    public void Shatter()
    {
        foreach (Rigidbody part in allParts)
        {
            part.isKinematic = false;
        }
        StartCoroutine(DestroyBeerBottleCollider());
        //Destroy(gameObject);
    }

    IEnumerator DestroyBeerBottleCollider()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(GetComponent<Collider>());
    }
}
