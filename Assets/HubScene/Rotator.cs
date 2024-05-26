using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed;
    private void Update()
    {
        gameObject.transform.Rotate(0, rotationSpeed, 0);
    }
}
