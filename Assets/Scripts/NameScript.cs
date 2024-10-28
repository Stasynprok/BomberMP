using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameScript : MonoBehaviour
{
    private void Awake()
    {
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }
}
