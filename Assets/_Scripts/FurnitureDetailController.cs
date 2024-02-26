using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureDetailController : MonoBehaviour
{
    private MeshRenderer objectMesh;
    private MeshFilter objectMeshFilter;
    public Material[] materialsHigh;
    public Material[] materialsLow;

    public Mesh meshHigh;
    public Mesh meshLow;

    private void Awake()
    {
        objectMesh = GetComponent<MeshRenderer>();
        objectMeshFilter = GetComponent<MeshFilter>();
    }

    public void setMaterialHigh()
    {
        objectMesh.materials = materialsHigh;
    }

    public void setMaterialLow()
    {
        objectMesh.materials = materialsLow;
    }

    public void setMeshHigh()
    {
        objectMeshFilter.mesh = meshHigh;
    }

    public void setMeshLow()
    {
        objectMeshFilter.mesh = meshLow;
    }
}
