using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyScreenInstantiate : MonoBehaviour
{

    public GameObject bloodyScreenPrefab;

    public GameObject InstantiateBloodyScreen()
    {
        GameObject newBloodyScreen = Instantiate(bloodyScreenPrefab, transform.position, Quaternion.identity, transform);
        return newBloodyScreen;
    }
}
