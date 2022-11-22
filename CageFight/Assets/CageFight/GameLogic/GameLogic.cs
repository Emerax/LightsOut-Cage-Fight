using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Top level logic for a game of Lights Out: Cage Figth (tm).
/// </summary>
public class GameLogic : MonoBehaviourPunCallbacks, IPunObservable {
    [SerializeField]
    private int allowance;
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
    private List<string> playerNames;
    [SerializeField]
    private int startingMoney;
    [SerializeField]
    private float shopTime = 5f;
    [SerializeField]
    private float combatTime = 5f;
    [SerializeField]
    private int roundsPerGame = 5;
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
    private CameraController cameraController;
    private int nextPlayerColorIndex = 0;
    private List<Player> players;
    private float remainingTime = 5f;

    public enum GameState {
        PRE_PHASE = 0,
        COMBAT_PHASE = 1,
        SHOP_PHASE = 2,
        END_PHASE = 3,
    }

    private GameState state = GameState.PRE_PHASE;
    private int currentRound;

    private void Awake() {
        arena = FindObjectOfType<Arena>();
        arena.ShopRelocated += OnShopRelocated;
        arena.shop.SetVisible(false);
        cameraController = FindObjectOfType<CameraController>();
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

    private void OnDestroy() {
        arena.ShopRelocated -= OnShopRelocated;
    }

    private void Update() {
        UpdateInput();
        switch(state) {
            case GameState.PRE_PHASE:
                break;
            case GameState.COMBAT_PHASE:
                monsterManager.Tick(Time.deltaTime);
                CheckTimer(GameState.SHOP_PHASE);
                break;
            case GameState.SHOP_PHASE:
                CheckTimer(GameState.COMBAT_PHASE);
                break;
            case GameState.END_PHASE:
                break;
            default:
                break;
        }
    }

    private void CheckTimer(GameState timeoutState) {
        remainingTime -= Time.deltaTime;
        ui.SetTimerTextDisplay(remainingTime);
        if(remainingTime < 0f) {
            ChangeState(timeoutState);
        }
    }

    private void UpdateInput() {
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            stream.SendNext(remainingTime);
        }
        else {
            remainingTime = (float)stream.ReceiveNext();
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
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { COLOR_KEY, ColorToVector3(playerColors[nextPlayerColorIndex]) } });
            PhotonNetwork.LocalPlayer.NickName = playerNames[nextPlayerColorIndex];
            nextPlayerColorIndex++;
            ui.SetHostPanelDisplay();
        }
        else {
            //Yer a pleb!
        }

        LocalPlayer = new(PhotonNetwork.LocalPlayer, startingMoney);
        LocalPlayer.MoneyChangeAction += OnPlayerMoneyChanged;
        LocalPlayer.ToggleReady(true);
        OnPlayerMoneyChanged(LocalPlayer.Money);
        arena.shop.SetLocalPlayer(LocalPlayer);
        arena.center.SetLocalPlayer(LocalPlayer);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log($"Player {newPlayer} entered the room");
        if(state != GameState.PRE_PHASE) {
            if(PhotonNetwork.IsMasterClient) {
                PhotonNetwork.CloseConnection(newPlayer);
                return;
            }
        }

        if(state == GameState.PRE_PHASE) {
            //Update number of non-ready players.
            int readyPlayerCount = PhotonNetwork.PlayerList.Count(p => p.CustomProperties.TryGetValue(READY_KEY, out object ready) && (bool)ready);
            int currentPlayerCount = PhotonNetwork.PlayerList.Length;
            ui.SetReadyPlayersDisplay(readyPlayerCount, currentPlayerCount);
        }

        if(PhotonNetwork.IsMasterClient) {
            newPlayer.SetCustomProperties(new Hashtable() { { COLOR_KEY, ColorToVector3(playerColors[nextPlayerColorIndex]) } });
            newPlayer.NickName = playerNames[nextPlayerColorIndex];
            nextPlayerColorIndex++;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        switch(state) {
            case GameState.PRE_PHASE:
                //Read READY here to update UI with number of ready players. Display in UI?
                if(changedProps.TryGetValue(READY_KEY, out object _)) {
                    int readyPlayerCount = PhotonNetwork.PlayerList.Count(p => p.CustomProperties.TryGetValue(READY_KEY, out object ready) && (bool)ready);
                    int currentPlayerCount = PhotonNetwork.PlayerList.Length;
                    ui.SetReadyPlayersDisplay(readyPlayerCount, currentPlayerCount);
                }
                break;
            case GameState.SHOP_PHASE:
                Debug.Log($"State is now {state}");
                if(changedProps.TryGetValue(READY_KEY, out object _)) {
                    if(AllPlayersAreReady()) {
                        ChangeState(GameState.COMBAT_PHASE);
                    }
                    else {
                        int readyPlayerCount = players.Count(p => p.CustomProperties.TryGetValue(READY_KEY, out object ready) && (bool)ready);
                        int currentPlayerCount = players.Count;
                        ui.SetReadyPlayersDisplay(readyPlayerCount, currentPlayerCount);
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
        ui.ResetAction += ResetGame;
#endif
    }

    private void ResetGame() {
        photonView.RPC(nameof(ResetGameRPC), RpcTarget.All);
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
        if(!PhotonNetwork.IsMasterClient) {
            return;
        }

        if(gameState is GameState.SHOP_PHASE && currentRound >= roundsPerGame) {
            photonView.RPC(nameof(ChangeStateRPC), RpcTarget.All, GameState.END_PHASE);
        }
        else {
            photonView.RPC(nameof(ChangeStateRPC), RpcTarget.All, gameState);
        }
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
                arena.shop.DespawnMonsters(monsterManager);
                arena.center.SetActive(false);
                //Cleanup ALL THE MONSTERS!
                if(PhotonNetwork.IsMasterClient) {
                    arena.ResetSegmentOwnerships();
                }
                break;
            case GameState.SHOP_PHASE:
                //Hide away shops, re-spawn monsters.
                cameraController.SetArenaTarget();
                arena.shop.SetVisible(false);
                break;
            case GameState.END_PHASE:
                //Reset everything for a new game?
                break;
            default:
                break;
        }

        state = newState;
        ui.OnEnterState(state);
        Debug.Log($"Start {state}");

        //Handle entering new state.
        switch(state) {
            case GameState.COMBAT_PHASE:
                remainingTime = combatTime;
                arena.shop.SpawnMonsters(monsterManager);
                arena.center.SetActive(true);
                LocalPlayer.ToggleReady(false);
                break;
            case GameState.SHOP_PHASE:
                LocalPlayer.Money += allowance;
                remainingTime = shopTime;
                currentRound++;
                if(PhotonNetwork.IsMasterClient) {
                    arena.AssignSegmentsToPlayers(players);
                }
                arena.shop.SetVisible(true);
                arena.shop.OnNewShopPhase();
                break;
            case GameState.END_PHASE:
                break;
            default:
                break;
        }
    }

    public void OnShopRelocated() {
        if(state == GameState.SHOP_PHASE) {
            cameraController.SetShopTarget(arena.shop);
        }
    }

    [PunRPC]
    private void ResetGameRPC() {
        LocalPlayer.ResetScore();
        LocalPlayer.Money = startingMoney;
        arena.shop.DespawnMonsters(monsterManager);
        arena.shop.DestroyCages();
        currentRound = 0;
        ChangeState(GameState.SHOP_PHASE);
    }
}
