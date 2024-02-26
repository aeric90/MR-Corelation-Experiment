using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceController : MonoBehaviour
{
    public GameObject shadowWall;
    public GameObject ghostWall;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleGhostWall(bool status)
    {
        ghostWall.SetActive(status);
    }

    public void toggleShadowWall(bool status)
    {
        shadowWall.SetActive(status);
    }
}
