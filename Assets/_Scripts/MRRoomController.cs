using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRRoomController : MonoBehaviour
{
    public static MRRoomController instance;

    public GameObject roomPrefab;


    private void Awake()
    {
        instance = this;
    }


    public void AlignRoom()
    {
        StartCoroutine(RoomAlignment());
    }

    IEnumerator RoomAlignment()
    {
        Debug.Log("Aligning Room");
        yield return new WaitForSeconds(2.0f);

        GameObject floor = null;

        Debug.Log("Waiting for floor");
        while (floor == null)
        {
            OVRSemanticClassification[] anchorLists = FindObjectsOfType(typeof(OVRSemanticClassification)) as OVRSemanticClassification[];

            foreach (OVRSemanticClassification anchor in anchorLists)
            {
                if (anchor.Contains("FLOOR"))
                {
                    floor = anchor.gameObject;
                    break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Floor found");


        foreach(GameObject room in GameObject.FindGameObjectsWithTag("room"))
        {
            Destroy(room);
        }
        
        GameObject newRoom = Instantiate(roomPrefab);

        if (floor != null)
        {
            Debug.Log("Align To Floor");
            newRoom.transform.position = floor.transform.position;
            float floorZRoatation = floor.transform.eulerAngles.z + floor.transform.eulerAngles.y + 90.0f;
            newRoom.transform.eulerAngles = new Vector3(0.0f, floorZRoatation, 0.0f);
        }

        ExperimentController.instance.SetFittsSpawnPoint();

        ExperimentController.instance.SetUIAnchorPoint();

        while (RoomController.instance == null) yield return new WaitForSeconds(0.1f);
        ExperimentController.instance.ResetCurrentState();
    }
}
