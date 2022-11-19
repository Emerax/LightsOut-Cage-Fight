using System;

public interface IMonsterController {

    public MonsterData Data { get; }

    public Action<IMonsterController> OnDeath { get; set; }

    public void Tick(float deltaTime);
    public void ReceiveDamage(float damage);
}
