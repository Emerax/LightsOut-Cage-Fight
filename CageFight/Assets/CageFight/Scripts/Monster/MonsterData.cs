using System;
using System.Linq;
using UnityEngine;

public class MonsterData {

    public int Team { get; private set; }
    public MonsterVariantID ID { get; private set; }
    public Sprite Sprite { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public float MaxHealth { get; private set; }
    public float EnemyDetectionRange { get; private set; }
    public float AboidanceDistance { get; private set; }

    public float health;
    public Vector2 position;

    public bool isSynced;

    public Action<float> OnDamageReceived;

    public MonsterData(MonsterSettings monsterSettings, int team, MonsterVariantID id, float health, Vector2 position, bool isSynced) {
        Team = team;
        ID = id;

        MonsterVariant variant = monsterSettings.monsterVariants.First(v => v.identifier == ID);
        Sprite = variant.sprite;
        Width = variant.width;
        Height = variant.height;
        MaxHealth = variant.health;
        EnemyDetectionRange = variant.enemyDetectionRange;
        AboidanceDistance = variant.aboidanceDistance;

        this.health = health;
        this.position = position;
        this.isSynced = isSynced;
    }

    public object[] ToObjectArray() {
        return new object[] {
            Team,
            ID,
            health,
            position
        };
    }

    public static MonsterData FromObjectArray(MonsterSettings monsterSettings, object[] objects) {
        int team = (int)objects[0];
        MonsterVariantID id = (MonsterVariantID)objects[1];
        float health = (float)objects[2];
        Vector2 position = (Vector2)objects[3];
        return new MonsterData(monsterSettings, team, id, health, position, isSynced: false);
    }
}
