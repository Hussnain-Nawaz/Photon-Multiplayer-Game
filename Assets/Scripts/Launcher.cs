using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    bool isConnecting;
    private string gameVersion = "1";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }
    public void Connect()
    {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            isConnecting=PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        if(isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
        
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        base.OnDisconnected(cause);
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("Room for 1");
        }

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers=maxPlayersPerRoom});
    }
}
