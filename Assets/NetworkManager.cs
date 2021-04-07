using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	// Start is called before the first frame update
	void Start()
	{
		ConnectToServer();
	}

	void ConnectToServer()
	{
		PhotonNetwork.ConnectUsingSettings();
		Debug.Log("Trying to connect to the server...");
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to the server!");
		base.OnConnectedToMaster();
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 2;
		roomOptions.IsVisible = true;
		roomOptions.IsOpen = true;
		PhotonNetwork.JoinOrCreateRoom("Pottery Room", roomOptions, TypedLobby.Default);
	}


	public override void OnJoinedRoom()
	{
		Debug.Log("Joined a room");
		base.OnJoinedRoom();
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log("A new player joined the room");
		base.OnPlayerEnteredRoom(newPlayer);
	}

}