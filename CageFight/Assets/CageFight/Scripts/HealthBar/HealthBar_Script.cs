using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_Script : MonoBehaviour {

    public MonsterBehaviour Monster;
    private Image healthBar;

    private void Start() {
        healthBar = GetComponent<Image>();
        Player player = PhotonNetwork.PlayerList.First(p => p.ActorNumber == Monster.Data.Team);
        if(player.CustomProperties.TryGetValue(GameLogic.COLOR_KEY, out object color)) {
            healthBar.color = GameLogic.Vector3ToColor((Vector3)color);
        }
    }
    private void Update() {
        healthBar.fillAmount = Monster.Data.health/Monster.Data.MaxHealth;

    }
}
 
