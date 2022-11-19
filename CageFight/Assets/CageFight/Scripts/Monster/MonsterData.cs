using System;
using UnityEngine;

public class MonsterData {

    public int Team { get; private set; }

    public float health;
    public Vector2 position;

    public bool isSynced;

    public Action<float> OnDamageReceived;

    public MonsterData(int team, float startHealth, Vector2 startPosition, bool isSynced) {
        Team = team;
        health = startHealth;
        position = startPosition;
        this.isSynced = isSynced;
    }

    public object[] ToObjectArray() {
        return new object[] {
            Team,
            health,
            position
        };
    }

    public static MonsterData FromObjectArray(object[] objects) {
        int team = (int)objects[0];
        float health = (float)objects[1];
        Vector2 position = (Vector2)objects[2];
        return new MonsterData(team, health, position, isSynced: false);
    }
}
