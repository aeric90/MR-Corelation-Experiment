using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fittsSpawnController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentLocal = transform.localPosition;
        transform.localPosition = new Vector3(0.0f, currentLocal.y, currentLocal.z);
    }
}
