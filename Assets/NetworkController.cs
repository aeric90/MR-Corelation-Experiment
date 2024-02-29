using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRoom("GSR_Network");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        PhotonNetwork.CreateRoom("GSR_Network", new RoomOptions());
    }
}
