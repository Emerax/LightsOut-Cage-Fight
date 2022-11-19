using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager {

    private List<IMonsterController> monsterControllers;

    public MonsterManager() {
        monsterControllers = new List<IMonsterController>();
    }

    public GameObject SpawnMonster() {
        float moveSpeed = 1f;
        Vector2 startPosition = 10f * Vector2.left;
        ArenaData arenaData = new(Vector2.zero);
        MonsterData monsterData = new(moveSpeed, startPosition);
        monsterControllers.Add(new MeleeController(arenaData, monsterData));

        GameObject go = PhotonNetwork.Instantiate("Monster", new Vector3(startPosition.x, 0, startPosition.y), Quaternion.identity, 0, monsterData.ToObjectArray());
        go.GetComponent<MonsterBehaviour>().SetMonsterData(monsterData);
        return go;
    }

    public void Tick(float deltaTime) {
        foreach(IMonsterController controller in monsterControllers) {
            controller.Tick(deltaTime);
        }
    }
}
