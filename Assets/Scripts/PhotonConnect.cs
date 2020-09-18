using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;


// automatically connects to a Photon room when the app starts (no lobby)
// tries to join an existing room, if no room exists, create room

public class PhotonConnect : MonoBehaviourPunCallbacks
{
    // only two players, HL2 application and PC application with AzureKinect
    private byte maxPlayersPerRoom = 2;

    // The gameobjects that this player wants to own, even if not server
    public GameObject punObject1, punObject2, punObject3;

    // this function configures all the Photon server settings and connects to server
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
       
    }

    // once the application is connected to the server, OnConnectedToMaster us automatically called
    // try to join a room
    public override void OnConnectedToMaster()
    {
        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
    }

    // if no room exists, create room
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
    }

    // once room is joined, instantiate a player prefab
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), Vector3.zero, Quaternion.identity);

        // takes over the ownership of the punObjects, so that they can be controlled locally
        if (punObject1 != null)
            punObject1.GetComponent<PhotonView>().RequestOwnership();
        if (punObject2 != null)
            punObject2.GetComponent<PhotonView>().RequestOwnership();
        if (punObject3 != null)
            punObject3.GetComponent<PhotonView>().RequestOwnership();
    }


}
