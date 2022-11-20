using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
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

    private void Awake() {
        OnEnterState(GameState.PRE_PHASE);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if(targetPlayer == PhotonNetwork.LocalPlayer) {
            if(changedProps.TryGetValue(GameLogic.SCORE_KEY, out object score)) {
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
        timerText.text = time.ToString("hh':'mm':'ss");
    }

    public void OnEnterState(GameState state) {
        switch(state) {
            case GameState.PRE_PHASE:
                moneyText.enabled = false;
                scoreText.enabled = false;
                readyText.enabled = true;
                timerText.enabled = false;
                break;
            case GameState.COMBAT_PHASE:
                moneyText.enabled = true;
                scoreText.enabled = true;
                readyText.enabled = false;
                timerText.enabled = true;
                break;
            case GameState.SHOP_PHASE:
                moneyText.enabled = true;
                scoreText.enabled = true;
                readyText.enabled = true;
                timerText.enabled = true;
                break;
            case GameState.END_PHASE:
                moneyText.enabled = false;
                scoreText.enabled = false;
                readyText.enabled = false;
                timerText.enabled = false;
                break;
            case GameState.DEBUG_COMBAT_PHASE:
                break;
            default:
                break;
        }

    }
}
