using System;
using System.Linq;
using UnityEngine;

public interface IMonsterController {

    public MonsterData Data { get; }

    public Action<IMonsterController> OnDeath { get; set; }

    public void Tick(float deltaTime);
    public void ReceiveDamage(float damage);

    public static IMonsterController[] Create(MonsterSettings monsterSettings, MonsterVariantID id, ArenaData arenaData, Vector2 startPosition) {
        return monsterSettings.monsterVariants.First(v => v.identifier == id).CreateControllers(monsterSettings, arenaData, startPosition);
    }
}
