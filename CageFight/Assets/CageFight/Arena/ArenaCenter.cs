using System.Collections.Generic;
using UnityEngine;

public class ArenaCenter : MonoBehaviour {
    [SerializeField]
    private float scoringDelay;
    [SerializeField]
    private int moneyPerAward;

    private float coolDown = 0f;
    private GladiatorManager localPlayer;
    private bool active = false;
    readonly List<MonsterBehaviour> enteredMonsters = new();

    private void Awake() {
        coolDown = scoringDelay;
    }

    private void Update() {
        if(!active) {
            return;
        }

        coolDown -= Time.deltaTime;
        if(coolDown <= 0f) {
            AwardMoney();
            coolDown = scoringDelay;
        }
    }


    public void Register(MonsterBehaviour monsterBehaviour) {
        enteredMonsters.Add(monsterBehaviour);
        monsterBehaviour.Died += OnMonsterDeath;
    }

    public void Deregister(MonsterBehaviour monsterBehaviour) {
        enteredMonsters.Remove(monsterBehaviour);
        monsterBehaviour.Died -= OnMonsterDeath;
    }

    public void SetActive(bool active) {
        enteredMonsters.Clear();
        this.active = active;
    }

    private void AwardMoney() {
        localPlayer.AddMoney(moneyPerAward * enteredMonsters.Count);
    }

    private void OnMonsterDeath(MonsterBehaviour exMonster) {
        enteredMonsters.Remove(exMonster);
    }

    internal void SetLocalPlayer(GladiatorManager localPlayer) {
        this.localPlayer = localPlayer;
    }
}
