using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager {

    private readonly List<IMonsterController> monsterControllers;

    public MonsterManager() {
        monsterControllers = new List<IMonsterController>();
    }

    public GameObject SpawnMonster() {
        int team = PhotonNetwork.LocalPlayer.ActorNumber;
        float startHealth = 10f;
        Vector2 startPosition = 10f * Vector2.left;
        ArenaData arenaData = new(Vector2.zero);
        MonsterData monsterData = new(team, startHealth, startPosition, isSynced: true);
        float moveSpeed = 1f;
        float attackRange = 1f;
        float attackCooldown = 2f;
        float attackDamage = 5f;
        IMonsterController monsterController = new MeleeController(arenaData, monsterData, moveSpeed, attackRange, attackCooldown, attackDamage);
        monsterControllers.Add(monsterController);
        monsterController.OnDeath += OnMonsterDied;

        GameObject go = PhotonNetwork.Instantiate("Monster", new Vector3(startPosition.x, 0, startPosition.y), Quaternion.identity, 0, monsterData.ToObjectArray());
        go.GetComponent<MonsterBehaviour>().SetController(monsterController, monsterData);
        return go;
    }

    private void OnMonsterDied(IMonsterController monsterController) {
        monsterControllers.Remove(monsterController);
    }

    public void Tick(float deltaTime) {
        foreach(IMonsterController controller in monsterControllers) {
            controller.Tick(deltaTime);
        }
    }
}
