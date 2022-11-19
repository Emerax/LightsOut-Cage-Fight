using Photon.Pun;
using UnityEngine;

public class MonsterData {

    public int Team { get; private set; }
    public float MovementSpeed { get; private set; }

    public Vector2 position;

    public MonsterData(int team, float movementSpeed, Vector2 startPosition) {
        Team = team;
        MovementSpeed = movementSpeed;
        position = startPosition;
    }

    public object[] ToObjectArray() {
        return new object[] {
            Team,
            MovementSpeed,
            position
        };
    }

    public static MonsterData FromObjectArray(object[] objects) {
        int team = (int)objects[0];
        float movementSpeed = (float)objects[1];
        Vector2 position = (Vector2)objects[2];
        return new MonsterData(team, movementSpeed, position);

    }
}
