using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

/// <summary>
/// 1/8 of the arena. Owned by a player who will use the shop.
/// </summary>
public class ArenaSegment : MonoBehaviourPunCallbacks {
    [SerializeField]
    private Renderer bannerRenderer;
    [SerializeField]
    private TMP_Text scoreText;
    private Player owner;

    public void AssignToPlayer(Player player) {
        owner = player;

        if(player.CustomProperties.TryGetValue(GameLogic.COLOR_KEY, out object color)) {
            bannerRenderer.material.color = (Color)color;
        }

        int ownerScore = (int)player.CustomProperties[GameLogic.SCORE_KEY];
        scoreText.text = $"Score:\n{ownerScore}";
    }

    public void DeassignOwnership(Color color) {
        owner = null;
        bannerRenderer.material.color = color;
        scoreText.text = "";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if(owner != null && targetPlayer == owner) {
            if(changedProps.TryGetValue(GameLogic.SCORE_KEY, out object score)) {
                scoreText.text = $"Score:\n{(int)score}";
            }
            if(changedProps.TryGetValue(GameLogic.COLOR_KEY, out object color)) {
                bannerRenderer.material.color = (Color)color;
            }
        }
    }
}
