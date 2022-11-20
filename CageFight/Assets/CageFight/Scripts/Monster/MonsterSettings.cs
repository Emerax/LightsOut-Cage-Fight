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

    public abstract IMonsterController CreateController(MonsterSettings monsterSettings, ArenaData arenaData, Vector2 position);

    protected MonsterData CreateMonsterData(MonsterSettings monsterSettings, Vector2 position) {
        int team = PhotonNetwork.LocalPlayer.ActorNumber;
        return new(monsterSettings, team, identifier, health, position, isSynced: true);
    }
}
