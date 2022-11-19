using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    private MonsterSettings monsterSettings;
    public GladiatorManager LocalPlayer { get; private set; }

    //Player property keys
    public const string SCORE_KEY = "SCORE";
    public const string COLOR_KEY = "COLOR";
    public const string READY_KEY = "READY";

    private MonsterManager monsterManager;
    private UI ui;
    private Arena arena;
    private int nextPlayerColorIndex = 0;
    private List<Player> players;

    private enum GameState {
        PRE_PHASE = 0,
        COMBAT_PHASE = 1,
        SHOP_PHASE = 2,
        END_PHASE = 3,
        DEBUG_COMBAT_PHASE = 99,
    }

    private GameState state = GameState.PRE_PHASE;

    private void Awake() {
        arena = FindObjectOfType<Arena>();
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
        UpdateInput();
        switch(state) {
            case GameState.PRE_PHASE:
                break;
            case GameState.DEBUG_COMBAT_PHASE:
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
        if(Input.GetKeyDown(KeyCode.P)) {
            if(state == GameState.DEBUG_COMBAT_PHASE) {
                photonView.RPC(nameof(DebugSpawnMonstersRPC), RpcTarget.All);
            }
            else {
                ChangeState(GameState.DEBUG_COMBAT_PHASE);
            }
        }

        switch(state) {
            case GameState.PRE_PHASE:
                if(Input.GetKeyDown(KeyCode.Return)) {
                    if(PhotonNetwork.IsMasterClient && AllPlayersAreReady()) {
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
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { COLOR_KEY, ColorToVector3(playerColors[nextPlayerColorIndex++]) } });
        }
        else {
            //Yer a pleb!
        }

        LocalPlayer = new(PhotonNetwork.LocalPlayer, startingMoney);
        LocalPlayer.MoneyChangeAction += OnPlayerMoneyChanged;
        LocalPlayer.ToggleReady(true);
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
            newPlayer.SetCustomProperties(new Hashtable() { { COLOR_KEY, ColorToVector3(playerColors[nextPlayerColorIndex++]) } });
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        switch(state) {
            case GameState.SHOP_PHASE:
                if(changedProps.TryGetValue(READY_KEY, out object _)) {
                    if(AllPlayersAreReady()) {
                        ChangeState(GameState.COMBAT_PHASE);
                    }
                }
                break;
            default:
                break;
        }
    }

    public static Vector3 ColorToVector3(Color c) {
        return new Vector3(c.r, c.g, c.b);
    }

    public static Color Vector3ToColor(Vector3 v) {
        return new Color(v.x, v.y, v.z);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"Player {otherPlayer} left the room");
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
        ChangeState(GameState.SHOP_PHASE);
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

    private void ChangeState(GameState gameState) {
        photonView.RPC(nameof(ChangeStateRPC), RpcTarget.All, gameState);
    }

    [PunRPC]
    private void ChangeStateRPC(GameState newState) {
        //Handle exiting old state. Guard with PhotonNetwork.IsMasterClient as needed.
        switch(state) {
            case GameState.PRE_PHASE:
                Debug.Log("Starting game!");
                players = new List<Player>(PhotonNetwork.PlayerList); //This should never need another update, hopefully.
                LocalPlayer.ToggleReady(false);
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
        Debug.Log($"Start {state}");

        //Handle entering new state.
        switch(state) {
            case GameState.COMBAT_PHASE:
                break;
            case GameState.SHOP_PHASE:
                if(PhotonNetwork.IsMasterClient) {
                    arena.AssignSegmentsToPlayers(players);
                }
                break;
            case GameState.END_PHASE:
                break;
            case GameState.DEBUG_COMBAT_PHASE:
                DebugSpawnMonsters();
                break;
            default:
                break;
        }
    }

    private void DebugSpawnMonsters() {
        ArenaData arenaData = new(Vector2.zero);
        int team = PhotonNetwork.LocalPlayer.ActorNumber;
        float angle = team * 0.25f * Mathf.PI;
        Vector2 axis = new(Mathf.Cos(angle), Mathf.Sin(angle));

        monsterManager.SpawnMonster(IMonsterController.Create(monsterSettings, MonsterVariantID.Melee, arenaData, 10f * axis));
        monsterManager.SpawnMonster(IMonsterController.Create(monsterSettings, MonsterVariantID.Ranged, arenaData, -10f * axis));
    }

    [PunRPC]
    private void DebugSpawnMonstersRPC() {
        DebugSpawnMonsters();
    }
}
