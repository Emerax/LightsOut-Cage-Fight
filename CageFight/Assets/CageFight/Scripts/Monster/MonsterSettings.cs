using Photon.Pun;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MonsterSettings), menuName = "Settings/" + nameof(MonsterSettings), order = 1)]
public class MonsterSettings: ScriptableObject {

    public MonsterVariant[] monsterVariants;
}

public enum MonsterVariantID {
    Unspecified = -1,
    Melee = 0,
    Ranged = 1,
}

public abstract class MonsterVariant : ScriptableObject {
    public MonsterVariantID identifier;
    public float width = 1f;
    public float height = 1f;
    public float health = 10f;
    public float movementSpeed = 1f;

    public abstract IMonsterController CreateController(ArenaData arenaData, Vector2 startPosition);

    protected MonsterData CreateMonsterData(Vector2 startPosition) {
        int team = PhotonNetwork.LocalPlayer.ActorNumber;
        return new(team, identifier, width, height, health, health, startPosition, isSynced: true);
    }
}
