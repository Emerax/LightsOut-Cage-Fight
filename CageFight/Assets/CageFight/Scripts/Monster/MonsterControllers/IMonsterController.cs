using System;

public interface IMonsterController {

    public Action<IMonsterController> OnDeath { get; set; }

    public void Tick(float deltaTime);
    public void ReceiveDamage(float damage);
}
