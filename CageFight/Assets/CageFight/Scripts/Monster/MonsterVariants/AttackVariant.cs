using UnityEngine;

[CreateAssetMenu(fileName = nameof(AttackVariant), menuName = "MonsterVariants/" + nameof(AttackVariant), order = 1)]
public class AttackVariant : MonsterVariant {

    public float attackRange = 1f;
    public float attackCooldown = 2f;
    public float attackDamage = 5f;

    public override IMonsterController CreateController(MonsterSettings monsterSettings, ArenaData arenaData, Vector2 position) {
        return new AttackController(arenaData, CreateMonsterData(monsterSettings, position), this);
    }
}
