using System;
using UnityEngine;

public class MeleeController : IMonsterController {

    public MonsterData Data => monsterData;

    private readonly ArenaData arenaData;
    private readonly MonsterData monsterData;

    private readonly float movementSpeed;
    private readonly float attackRange;
    private readonly float attackCooldown;
    private readonly float attackDamage;

    private float attackCooldownLeft;
    private bool isDead;

    public Action<IMonsterController> OnDeath { get; set; }

    public MeleeController(ArenaData arenaData, MonsterData monsterData,
        float movementSpeed, float attackRange, float attackCooldown, float attackDamage) {

        this.arenaData = arenaData;
        this.monsterData = monsterData;

        this.movementSpeed = movementSpeed;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;
        this.attackDamage = attackDamage;

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
        monsterData.position += deltaTime * movementSpeed * toTarget.normalized;

        if(attackCooldownLeft <= 0) {
            if(closestEnemyDistance < attackRange) {
                closestEnemy.ReceiveDamage(attackDamage);
                attackCooldownLeft = attackCooldown;
                Debug.Log($"{nameof(MeleeController)} on team {monsterData.Team} attacked monster on team {closestEnemy.Data.Team} from {closestEnemyDistance} m away.");
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
