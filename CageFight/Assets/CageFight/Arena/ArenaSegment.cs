using Photon.Realtime;
using TMPro;
using UnityEngine;

/// <summary>
/// 1/8 of the arena. Owned by a player who will use the shop.
/// </summary>
public class ArenaSegment : MonoBehaviour {
    [SerializeField]
    private Renderer bannerRenderer;
    [SerializeField]
    private TMP_Text scoreText;
    private Player owner;

    public void AssignToPlayer(Player player) {
        owner = player;

        if(player.CustomProperties.TryGetValue(GameLogic.COLOR_KEY, out object color)) {
            bannerRenderer.material.color = GameLogic.Vector3ToColor((Vector3)color);
        }

        int ownerScore = (int)player.CustomProperties[GameLogic.SCORE_KEY];
        scoreText.text = $"Score:\n{ownerScore}";
    }

    public void DeassignOwnership(Color color) {
        owner = null;
        bannerRenderer.material.color = color;
        scoreText.text = "";
    }

    public void OnScoreUpdated(Player targetPlayer, int score) {
        if(targetPlayer == owner) {
            scoreText.text = $"Score:\n{score}";
        }
    }

    internal void OnColorUpdated(Player targetPlayer, Color color) {
        if(targetPlayer == owner) {
            bannerRenderer.material.color = color;
        }
    }
}
