using Photon.Pun;
using UnityEngine;

public class MonsterFactory {

    public static IMonsterController CreateMonster() {
        int team = PhotonNetwork.LocalPlayer.ActorNumber;
        float startHealth = 10f;
        Vector2 startPosition = 10f * Vector2.left;
        ArenaData arenaData = new(Vector2.zero);
        MonsterData monsterData = new(team, startHealth, startHealth, startPosition, isSynced: true);

        float moveSpeed = 1f;
        float attackRange = 1f;
        float attackCooldown = 2f;
        float attackDamage = 5f;
        IMonsterController monsterController = new MeleeController(arenaData, monsterData, moveSpeed, attackRange, attackCooldown, attackDamage);

        return monsterController;
    }
}
