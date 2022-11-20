using System;
using System.Collections.Generic;
using UnityEngine;
using static Shop;

public class Cage : MonoBehaviour {
    public Action<CageEventType, Cage> CageEventAction;
    public int Cost { get => cost; private set => cost = value; }
    public List<IMonsterController> Monsters { get; private set; } = new();

    private MonsterSettings monsterSettings;
    private ArenaData arenaData;
    private bool bought = false;
    private int cost = 60;

    private void OnMouseDown() {
        if(bought) {
            CageEventAction?.Invoke(CageEventType.BEGIN_CLICK, this);
        }
        else {
            CageEventAction?.Invoke(CageEventType.BOUGHT, this);
        }
    }

    private void OnMouseUp() {
        if(bought) {
            CageEventAction?.Invoke(CageEventType.END_CLICK, this);
        }
    }

    public void Init(MonsterSettings monsterSettings, ArenaData arenaData) {
        this.monsterSettings = monsterSettings;
        this.arenaData = arenaData;
    }

    public void OnBought() {
        bought = true;
        Monsters.Add(IMonsterController.Create(monsterSettings, MonsterVariantID.Melee, arenaData, new(transform.position.x, transform.position.z)));
    }

    private void OnDestroy() {
        CageEventAction?.Invoke(CageEventType.DESTROYED, this);
    }
}
