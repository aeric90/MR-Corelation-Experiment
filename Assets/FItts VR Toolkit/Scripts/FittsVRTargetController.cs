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
        Debug.Log(other.tag);
        if(other.tag == "poke") FittsVRController.instance.TargetTriggerIn(this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "poke") FittsVRController.instance.TargetTriggerOut(this.gameObject);
    }
}
