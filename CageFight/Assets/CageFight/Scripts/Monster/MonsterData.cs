using Photon.Pun;
using UnityEngine;

public class MonsterData {

    public float MovementSpeed { get; private set; }

    public Vector2 position;

    public MonsterData(float movementSpeed, Vector2 startPosition) {
        MovementSpeed = movementSpeed;
        position = startPosition;
    }

    public object[] ToObjectArray() {
        return new object[] {
            MovementSpeed,
            position
        };
    }

    public static MonsterData FromObjectArray(object[] objects) {
        float movementSpeed = (float)objects[0];
        Vector2 position = (Vector2)objects[1];
        return new MonsterData(movementSpeed, position);

    }
}
