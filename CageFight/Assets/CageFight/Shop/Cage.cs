using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Shop;

public class Cage : MonoBehaviour {
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private GameObject priceTag;

    public Action<CageEventType, Cage> CageEventAction;
    public int Cost { get => cost; private set => cost = value; }
    public List<IMonsterController> Monsters { get; private set; } = new();

    private MonsterSettings monsterSettings;
    private ArenaData arenaData;
    private MonsterVariantID monsterID;
    private bool bought = false;
    private int cost;

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

    public void Init(MonsterSettings monsterSettings, ArenaData arenaData, int maxMonsterCost) {
        this.monsterSettings = monsterSettings;
        this.arenaData = arenaData;

        // Filter affordable monster variants
        MonsterVariant cheapestVariant = null;
        int cheapestCost = int.MaxValue;
        List<MonsterVariant> affordableVariants = new();
        MonsterVariant[] monsterVariants = monsterSettings.monsterVariants;
        foreach(MonsterVariant variant in monsterVariants) {
            if(variant.cost < maxMonsterCost) {
                affordableVariants.Add(variant);
            }
            if(variant.cost < cheapestCost) {
                cheapestVariant = variant;
                cheapestCost = variant.cost;
            }
        }

        MonsterVariant monsterVariant;
        if(affordableVariants.Count > 0) {
            // Randomize monster variant
            monsterVariant = affordableVariants[UnityEngine.Random.Range(0, affordableVariants.Count)];
        }
        else {
            // Use cheapest monster variant
            monsterVariant = cheapestVariant;
        }
        monsterID = monsterVariant.identifier;
        Cost = monsterVariant.cost;
        priceTag.GetComponentInChildren<TMP_Text>().text = Cost.ToString();

        sprite.sprite = monsterVariant.sprite;
    }

    public void OnBought() {
        bought = true;
        priceTag.SetActive(false);
        Monsters.AddRange(IMonsterController.Create(monsterSettings, monsterID, arenaData, new(transform.position.x, transform.position.z)));
    }

    private void OnDestroy() {
        CageEventAction?.Invoke(CageEventType.DESTROYED, this);
    }
}
