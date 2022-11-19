using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviourPunCallbacks {
    [SerializeField]
    private TMP_Text moneyText;
    [SerializeField]
    private TMP_Text scoreText;

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
}
