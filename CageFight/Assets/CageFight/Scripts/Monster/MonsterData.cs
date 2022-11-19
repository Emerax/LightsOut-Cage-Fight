using System;
using UnityEngine;

public class MonsterData {

    public int Team { get; private set; }
    public MonsterVariantID ID { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public float MaxHealth { get; private set; }

    public float health;
    public Vector2 position;

    public bool isSynced;

    public Action<float> OnDamageReceived;

    public MonsterData(int team, MonsterVariantID id, float width, float height, float maxHealth, float health, Vector2 startPosition, bool isSynced) {
        Team = team;
        ID = id;
        Width = width;
        Height = height;
        MaxHealth = maxHealth;
        this.health = health;
        position = startPosition;
        this.isSynced = isSynced;
    }

    public object[] ToObjectArray() {
        return new object[] {
            Team,
            ID,
            Width,
            Height,
            MaxHealth,
            health,
            position
        };
    }

    public static MonsterData FromObjectArray(object[] objects) {
        int team = (int)objects[0];
        MonsterVariantID id = (MonsterVariantID)objects[1];
        float width = (float)objects[2];
        float height = (float)objects[3];
        float maxHealth = (float)objects[4];
        float health = (float)objects[5];
        Vector2 position = (Vector2)objects[6];
        return new MonsterData(team, id, width, height, maxHealth, health, position, isSynced: false);
    }
}
