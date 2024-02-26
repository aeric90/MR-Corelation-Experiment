using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FittsVRTargetController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "poke") FittsVRController.instance.TargetTriggerIn(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "poke") FittsVRController.instance.TargetTriggerOut(other.gameObject);
    }
}
