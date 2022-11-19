using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Top level logic for a game of Lights Out: Cage Figth (tm).
/// </summary>
public class GameLogic : MonoBehaviourPunCallbacks {
    [SerializeField]
    private UI gameplaySceneUI;
    [SerializeField]
    private string androidVotingUI;
    [SerializeField]
    private string iphoneVotingUI;
    [SerializeField]
    private string webVotingUI;
    [SerializeField]
    private List<Color> playerColors;
    [SerializeField]
    private int startingMoney;
    public GladiatorManager LocalPlayer { get; private set; }

    //Player property keys
    public const string SCORE_KEY = "SCORE";
    public const string COLOR_KEY = "COLOR";
    public const string READY_KEY = "READY";

    private MonsterManager monsterManager;
    private UI ui;
    private int nextPlayerColorIndex = 0;

    private enum GameState {
        PRE_PHASE,
        COMBAT_PHASE,
        SHOP_PHASE,
        END_PHASE
    }

    private GameState state = GameState.PRE_PHASE;

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
        Hashtable props = new() {
            { READY_KEY, true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void Update() {
        UpdateInput();
        switch(state) {
            case GameState.PRE_PHASE:
                break;
            case GameState.COMBAT_PHASE:
                monsterManager.Tick(Time.deltaTime);
                break;
            case GameState.SHOP_PHASE:
                break;
            case GameState.END_PHASE:
                break;
            default:
                break;
        }
    }

    private void UpdateInput() {
        switch(state) {
            case GameState.PRE_PHASE:
                if(Input.GetKeyDown(KeyCode.Return)) {
                    if(AllPlayersAreReady()) {
                        StartGame();
                    }
                }
                break;
            default:
                break;
        }
    }

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom() {
        Debug.Log($"Joined the room as player {PhotonNetwork.LocalPlayer}");
        if(PhotonNetwork.IsMasterClient) {
            //First time setup goes here! This player should decide when the game starts.
            PhotonNetwork.EnableCloseConnection = true;
        }
        else {
            //Yer a pleb!
        }

        LocalPlayer = new(PhotonNetwork.LocalPlayer, startingMoney);
        LocalPlayer.MoneyChangeAction += OnPlayerMoneyChanged;
        OnPlayerMoneyChanged(LocalPlayer.Money);
        foreach(Shop shop in FindObjectsOfType<Shop>()) {
            shop.SetLocalPlayer(LocalPlayer);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log($"Player {newPlayer} entered the room");
        if(state != GameState.PRE_PHASE) {
            if(PhotonNetwork.IsMasterClient) {
                PhotonNetwork.CloseConnection(newPlayer);
                return;
            }
        }

        if(PhotonNetwork.IsMasterClient) {
            newPlayer.SetCustomProperties(new Hashtable() { { COLOR_KEY, playerColors[nextPlayerColorIndex++] } });
        }
    }

    private void LoadPlatformSpecificContent() {
#if UNITY_ANDROID
        Instantiate(androidVotingUI);
#elif UNITY_IOS
        Instantiate(iphoneVotingUI);
#elif UNITY_WEBGL
        Instantiate(webVotingUI);
#else
        ui = Instantiate(gameplaySceneUI);
#endif
    }

    private void StartGame() {
        photonView.RPC(nameof(ChangeState), RpcTarget.All, GameState.COMBAT_PHASE);
    }

    private bool AllPlayersAreReady() {
        foreach(Player player in PhotonNetwork.PlayerList) {
            if(player.CustomProperties.TryGetValue(READY_KEY, out object ready)) {
                if(!(bool)ready) {
                    return false;
                }
            }
        }

        return true;
    }

    private void OnPlayerMoneyChanged(int money) {
        ui.SetMoneyDisplay(money);
    }

    [PunRPC]
    private void ChangeState(GameState newState) {
        //Handle exiting old state. Guard with PhotonNetwork.IsMasterClient as needed.
        switch(state) {
            case GameState.PRE_PHASE:
                //Close room so no more people enter perhaps?
                Debug.Log("Starting game!");
                break;
            case GameState.COMBAT_PHASE:
                //Cleanup ALL THE MONSTERS!
                break;
            case GameState.SHOP_PHASE:
                //Hide away shops, re-spawn monsters.
                break;
            case GameState.END_PHASE:
                //Reset everything for a new game?
                break;
            default:
                break;
        }

        state = newState;

        //Handle entering new state.
        switch(state) {
            case GameState.PRE_PHASE:
                break;
            case GameState.COMBAT_PHASE:
                break;
            case GameState.SHOP_PHASE:
                break;
            case GameState.END_PHASE:
                break;
            default:
                break;
        }
    }
}
