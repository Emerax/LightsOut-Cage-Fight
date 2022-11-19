using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {
    [SerializeField]
    private List<ArenaSegment> segments;
    [SerializeField]
    private Color defaultBannerColor = Color.black;

    private void Awake() {
        ResetSegmentOwnerships();
    }

    public void ResetSegmentOwnerships() {
        foreach(ArenaSegment segment in segments) {
            segment.DeassignOwnership(defaultBannerColor);
        }
    }

    public void AssignSegmentsToPlayers(List<Player> players) {
        List<ArenaSegment> tempSegments = new List<ArenaSegment>(segments);
        int r;
        foreach(Player player in players) {
            r = Random.Range(0, tempSegments.Count);
            tempSegments[r].AssignToPlayer(player);
            tempSegments.Remove(tempSegments[r]);
        }
    }
}
