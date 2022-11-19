using UnityEngine;

public struct MeleeController : IMonsterController {

    private readonly ArenaData arenaData;
    private readonly MonsterData monsterData;

    public MeleeController(ArenaData arenaData, MonsterData monsterData) {
        this.arenaData = arenaData;
        this.monsterData = monsterData;
    }

    public void Tick(float deltaTime) {
        Vector2 toTarget = arenaData.centerPosition - monsterData.position;
        Vector2 startPosition = monsterData.position;
        monsterData.position += deltaTime * monsterData.MovementSpeed * toTarget.normalized;
    }
}
