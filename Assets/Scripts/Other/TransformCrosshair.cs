using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformCrosshair : MonoBehaviour
{
    private RectTransform crossHairTransform;
    [Range(80f, 200f)]
    public float normalSize;

    public float adjustSpeed = 0.0f;

    public float currentSize;
    public float targetSize;

    //public bool allowAdjust;

    private void Start()
    {
        crossHairTransform = GetComponent<RectTransform>();
        normalSize = 80f;
        currentSize = normalSize;
        targetSize = currentSize;
        //allowAdjust = false;
    }

    private void Update()
    {
        currentSize = Mathf.SmoothDamp(currentSize, targetSize, ref adjustSpeed, 0.15f);

        crossHairTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }

}
