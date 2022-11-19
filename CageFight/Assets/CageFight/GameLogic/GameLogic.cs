using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Top level logic for a game of Lights Out: Cage Figth (tm).
/// </summary>
public class GameLogic : MonoBehaviourPunCallbacks {
    [SerializeField]
    private GameObject gameplaySceneUI;
    [SerializeField]
    private string androidVotingUI;
    [SerializeField]
    private string iphoneVotingUI;
    [SerializeField]
    private string webVotingUI;
    private List<GladiatorManager> players = new();
    private MonsterManager monsterManager;

    private void Awake() {
#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        //These clients should never be master.
        //Try to join room, else... coroutine for 5 seconds and retry?
#else
        PhotonNetwork.ConnectUsingSettings();
        LoadPlatformSpecificContent();
#endif
    }

    private void Start() {
        monsterManager = new MonsterManager();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            photonView.RPC(nameof(SayHiRPC), RpcTarget.All);
        }
        monsterManager.Tick(Time.deltaTime);
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() {
        Debug.Log($"Joined the room as player {PhotonNetwork.LocalPlayer}");
        if(PhotonNetwork.IsMasterClient) {
            //First time setup goes here! This player should decide when the game starts.
        }
        else {
            //Yer a pleb!
        }
        monsterManager.SpawnMonster();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log($"Player {newPlayer} entered the room");
        //Add to player list.
    }

    private void LoadPlatformSpecificContent() {
#if UNITY_ANDROID
        Instantiate(androidVotingUI);
#elif UNITY_IOS
        Instantiate(iphoneVotingUI);
#elif UNITY_WEBGL
        Instantiate(webVotingUI);
#else
        Instantiate(gameplaySceneUI);
#endif
    }

    [PunRPC]
    private void SayHiRPC(PhotonMessageInfo info) {
        if(info.Sender.IsLocal) {
            Debug.Log($"You: Hi!");
        }
        else {
            Debug.Log($"{info.Sender.NickName}: Hi!");
        }
    }
}
