using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager {

    private readonly List<IMonsterController> monsterControllers;

    public MonsterManager() {
        monsterControllers = new List<IMonsterController>();
    }

    public GameObject SpawnMonster(IMonsterController monsterController) {
        monsterControllers.Add(monsterController);
        monsterController.OnDeath += OnMonsterDied;
        MonsterData monsterData = monsterController.Data;
        GameObject go = PhotonNetwork.Instantiate("Monster", new Vector3(monsterData.position.x, 0, monsterData.position.y), Quaternion.identity, 0, monsterData.ToObjectArray());
        go.GetComponent<MonsterBehaviour>().SetController(monsterController);
        return go;
    }

    private void OnMonsterDied(IMonsterController monsterController) {
        monsterControllers.Remove(monsterController);
    }

    public void ClearMonsters() {
        monsterControllers.Clear();
    }

    public void Tick(float deltaTime) {
        foreach(IMonsterController controller in monsterControllers) {
            controller.Tick(deltaTime);
        }
    }
}
