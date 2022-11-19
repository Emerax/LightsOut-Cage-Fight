using System;
using UnityEngine;

public class AttackController : IMonsterController {

    public MonsterData Data => monsterData;

    private readonly ArenaData arenaData;
    private readonly MonsterData monsterData;
    private readonly AttackVariant stats;

    private float attackCooldownLeft;
    private bool isDead;

    public Action<IMonsterController> OnDeath { get; set; }

    public AttackController(ArenaData arenaData, MonsterData monsterData, AttackVariant stats) {

        this.arenaData = arenaData;
        this.monsterData = monsterData;
        this.stats = stats;

        attackCooldownLeft = 0f;
        isDead = false;
    }

    public void Tick(float deltaTime) {
        if(isDead) {
            return;
        }

        MonsterBehaviour closestEnemy = null;
        float closestEnemyDistance = float.MaxValue;
        foreach(MonsterBehaviour enemy in MonsterList.Instance.GetMonstersOfOtherTeams(monsterData.Team)) {
            if(enemy.Data.isSynced) {
                var distanceToEnemy = Vector2.Distance(monsterData.position, enemy.Data.position);
                if(distanceToEnemy < closestEnemyDistance) {
                    closestEnemy = enemy;
                    closestEnemyDistance = distanceToEnemy;
                }
            }
        }

        Vector2 toTarget;
        if(closestEnemyDistance < 5f) {
            toTarget = closestEnemy.Data.position - monsterData.position;
        }
        else {
            toTarget = arenaData.centerPosition - monsterData.position;
        }
        monsterData.position += deltaTime * stats.movementSpeed * toTarget.normalized;

        if(attackCooldownLeft <= 0) {
            if(closestEnemyDistance < stats.attackRange) {
                closestEnemy.ReceiveDamage(stats.attackDamage);
                attackCooldownLeft = stats.attackCooldown;
                Debug.Log($"{Data.ID} monster on team {monsterData.Team} attacked {closestEnemy.Data.ID} monster on team {closestEnemy.Data.Team} from {closestEnemyDistance} m away.");
            }
        }
        else {
            attackCooldownLeft -= deltaTime;
        }
    }

    public void ReceiveDamage(float damage) {
        monsterData.health -= damage;
        if(monsterData.health <= 0) {
            isDead = true;
            OnDeath?.Invoke(this);
        }
    }
}
