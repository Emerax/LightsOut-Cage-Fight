using System;
using UnityEngine;

public class MonsterData {

    public int Team { get; private set; }
    public float MaxHealth { get; private set; }

    public float health;
    public Vector2 position;

    public bool isSynced;

    public Action<float> OnDamageReceived;

    public MonsterData(int team, float maxHealth, float health, Vector2 startPosition, bool isSynced) {
        Team = team;
        MaxHealth = maxHealth;
        this.health = health;
        position = startPosition;
        this.isSynced = isSynced;
    }

    public object[] ToObjectArray() {
        return new object[] {
            Team,
            MaxHealth,
            health,
            position
        };
    }

    public static MonsterData FromObjectArray(object[] objects) {
        int team = (int)objects[0];
        float maxHealth = (float)objects[1];
        float health = (float)objects[2];
        Vector2 position = (Vector2)objects[3];
        return new MonsterData(team, maxHealth, health, position, isSynced: false);
    }
}
