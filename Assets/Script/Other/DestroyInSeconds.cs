using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    
    [SerializeField] private float time = 1f;

    void Start()
    {
        Destroy(gameObject, time);
    }

   
}
