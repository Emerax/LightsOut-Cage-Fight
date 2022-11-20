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

        bool isCooldownLeft = attackCooldownLeft > 0;

        if(closestEnemyDistance < stats.attackRange) {
            if(!isCooldownLeft) {
                closestEnemy.ReceiveDamage(stats.attackDamage);
                attackCooldownLeft = stats.attackCooldown;
                Debug.Log($"{Data.ID} monster on team {monsterData.Team} attacked {closestEnemy.Data.ID} monster on team {closestEnemy.Data.Team} from {closestEnemyDistance} m away.");
            }
        }
        else {
            Vector2 toTarget;
            if(closestEnemyDistance < Data.EnemyDetectionRange) {
                toTarget = closestEnemy.Data.position - monsterData.position;
            }
            else {
                toTarget = arenaData.centerPosition - monsterData.position;
            }

            Vector2 aboidance = Vector2.zero;
            foreach(MonsterBehaviour friend in MonsterList.Instance.GetMonstersOfTeam(monsterData.Team)) {
                float distanceToFriend = Vector2.Distance(Data.position, friend.Data.position);
                aboidance += (Data.position - friend.Data.position) * Mathf.Clamp(Data.AboidanceDistance - distanceToFriend, 0, 1);
            }
            monsterData.position += (deltaTime * stats.movementSpeed * toTarget.normalized) + aboidance;
        }

        if(isCooldownLeft) {
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
