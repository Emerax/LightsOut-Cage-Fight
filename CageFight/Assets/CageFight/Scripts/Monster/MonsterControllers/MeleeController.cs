using UnityEngine;

public struct MeleeController : IMonsterController {

    private readonly ArenaData arenaData;
    private readonly MonsterData monsterData;

    public MeleeController(ArenaData arenaData, MonsterData monsterData) {
        this.arenaData = arenaData;
        this.monsterData = monsterData;
    }

    public void Tick(float deltaTime) {
        MonsterData closestEnemy = null;
        float closestEnemyDistance = float.MaxValue;
        foreach(MonsterData enemy in MonsterList.Instance.GetMonstersOfOtherTeams(monsterData.Team)) {
            var distanceToEnemy = Vector2.Distance(monsterData.position, enemy.position);
            if(distanceToEnemy < closestEnemyDistance) {
                closestEnemy = enemy;
                closestEnemyDistance = distanceToEnemy;
            }
        }

        Vector2 toTarget;
        if(closestEnemyDistance < 5f) {
            toTarget = closestEnemy.position - monsterData.position;
        }
        else {
            toTarget = arenaData.centerPosition - monsterData.position;
        }
        monsterData.position += deltaTime * monsterData.MovementSpeed * toTarget.normalized;
    }
}
