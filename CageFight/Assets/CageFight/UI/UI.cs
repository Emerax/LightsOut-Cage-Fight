using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameLogic;

public class UI : MonoBehaviourPunCallbacks {
    [SerializeField]
    private TMP_Text moneyText;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    private TMP_Text readyText;
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private GameObject hostPanel;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private TMP_Text winnerText;
    [SerializeField]
    private Button resetButton;

    public Action ResetAction;

    private void Awake() {
        OnEnterState(GameState.PRE_PHASE);
        resetButton.onClick.AddListener(() => { ResetAction?.Invoke(); });
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if(targetPlayer == PhotonNetwork.LocalPlayer) {
            if(changedProps.TryGetValue(SCORE_KEY, out object score)) {
                scoreText.text = $"Score:\n{(int)score}";
            }
        }
    }

    public void SetMoneyDisplay(int money) {
        moneyText.text = $"Money:\n{money}";
    }

    public void SetReadyPlayersDisplay(int ready, int current) {
        if(ready == current) {
            readyText.color = Color.green;
        }
        else {
            readyText.color = Color.red;
        }

        readyText.text = $"Readied players:\n{ready}/{current}";
    }

    public void SetTimerTextDisplay(float remainingTime) {
        TimeSpan time = TimeSpan.FromSeconds(remainingTime);
        timerText.text = time.ToString("mm':'ss");
    }

    public void SetHostPanelDisplay() {
        hostPanel.SetActive(PhotonNetwork.IsMasterClient);
    }

    private void SetWinnerDisplay() {
        Color winnerColor = Color.white;
        string winnerName = "";
        int bestScore = 0;
        foreach(Player player in PhotonNetwork.PlayerList) {
            if(player.CustomProperties.TryGetValue(SCORE_KEY, out object score)) {
                if((int)score > bestScore) {
                    bestScore = (int)score;
                    winnerColor = Vector3ToColor((Vector3)player.CustomProperties[COLOR_KEY]);
                    winnerName = player.NickName;
                }
            }
        }

        winnerText.color = winnerColor;
        winnerText.text = $"{winnerName} won with {bestScore} points!";
    }

    public void OnEnterState(GameState state) {
        switch(state) {
            case GameState.PRE_PHASE:
                moneyText.enabled = false;
                scoreText.enabled = false;
                readyText.enabled = true;
                timerText.enabled = false;
                hostPanel.SetActive(PhotonNetwork.IsMasterClient);
                gameOverPanel.SetActive(false);
                break;
            case GameState.COMBAT_PHASE:
                moneyText.enabled = true;
                scoreText.enabled = true;
                readyText.enabled = false;
                timerText.enabled = true;
                hostPanel.SetActive(false);
                gameOverPanel.SetActive(false);
                break;
            case GameState.SHOP_PHASE:
                moneyText.enabled = true;
                scoreText.enabled = true;
                readyText.enabled = true;
                hostPanel.SetActive(false);
                timerText.enabled = true;
                gameOverPanel.SetActive(false);
                break;
            case GameState.END_PHASE:
                moneyText.enabled = false;
                scoreText.enabled = false;
                readyText.enabled = false;
                timerText.enabled = false;
                hostPanel.SetActive(false);
                gameOverPanel.SetActive(true);
                SetWinnerDisplay();
                break;
            case GameState.DEBUG_COMBAT_PHASE:
                break;
            default:
                break;
        }

    }
}
