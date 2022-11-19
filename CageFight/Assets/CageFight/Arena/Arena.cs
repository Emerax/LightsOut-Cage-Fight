using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviourPunCallbacks {
    [SerializeField]
    private List<ArenaSegment> segments;
    [SerializeField]
    private Color defaultBannerColor = Color.black;

    private void Awake() {
        segments.ForEach(s => s.DeassignOwnership(defaultBannerColor));
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        if(changedProps.TryGetValue(GameLogic.SCORE_KEY, out object score)) {
            segments.ForEach(s => s.OnScoreUpdated(targetPlayer, (int)score));

        }
        if(changedProps.TryGetValue(GameLogic.COLOR_KEY, out object color)) {
            segments.ForEach(s => s.OnColorUpdated(targetPlayer, GameLogic.Vector3ToColor((Vector3)color)));
        }
    }

    public void ResetSegmentOwnerships() {
        photonView.RPC(nameof(DeassignSegmentRPC), RpcTarget.All);
    }

    public void AssignSegmentsToPlayers(List<Player> players) {
        List<ArenaSegment> tempSegments = new(segments);
        int r;
        foreach(Player player in players) {
            r = Random.Range(0, tempSegments.Count);
            ArenaSegment segment = tempSegments[r];
            Debug.Log($"Assigning segment {segments.IndexOf(segment)} for {player}");
            photonView.RPC(nameof(AssignSegmentRPC), RpcTarget.All, segments.IndexOf(segment), player);
            tempSegments.Remove(segment);
        }
    }

    [PunRPC]
    private void AssignSegmentRPC(int segmentIndex, Player newOwner) {
        segments[segmentIndex].AssignToPlayer(newOwner);
    }

    [PunRPC]
    private void DeassignSegmentRPC() {
        Debug.Log("Deassign");
        segments.ForEach(s => s.DeassignOwnership(defaultBannerColor));
    }
}
