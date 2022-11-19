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
    [SerializeField]
    private Transform cameraHolder;

    public Transform CameraHolderTransform { get => cameraHolder; }

    public Player Owner { get; private set; }

    public void AssignToPlayer(Player player) {
        Owner = player;

        if(player.CustomProperties.TryGetValue(GameLogic.COLOR_KEY, out object color)) {
            bannerRenderer.material.color = GameLogic.Vector3ToColor((Vector3)color);
        }

        int ownerScore = (int)player.CustomProperties[GameLogic.SCORE_KEY];
        scoreText.text = $"Score:\n{ownerScore}";
    }

    public void DeassignOwnership(Color color) {
        Owner = null;
        bannerRenderer.material.color = color;
        scoreText.text = "";
    }

    public void OnScoreUpdated(Player targetPlayer, int score) {
        if(targetPlayer == Owner) {
            scoreText.text = $"Score:\n{score}";
        }
    }

    internal void OnColorUpdated(Player targetPlayer, Color color) {
        if(targetPlayer == Owner) {
            bannerRenderer.material.color = color;
        }
    }
}
