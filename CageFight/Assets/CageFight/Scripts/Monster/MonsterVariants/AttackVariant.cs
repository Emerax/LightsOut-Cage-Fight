using UnityEngine;

[CreateAssetMenu(fileName = nameof(AttackVariant), menuName = "MonsterVariants/" + nameof(AttackVariant), order = 1)]
public class AttackVariant : MonsterVariant {

    public float attackRange = 1f;
    public float attackCooldown = 2f;
    public float attackDamage = 5f;

    public override IMonsterController[] CreateControllers(MonsterSettings monsterSettings, ArenaData arenaData, Vector2 position) {
        MonsterData[] monsterData = CreateMonsterData(monsterSettings, position);
        AttackController[] attackControllers = new AttackController[monsterData.Length];
        for(int i = 0; i < monsterData.Length; ++i) {
            attackControllers[i] = new AttackController(arenaData, monsterData[i], this);
        }
        return attackControllers;
    }
}
