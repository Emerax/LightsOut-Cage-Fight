using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MonsterSettings), menuName = "Settings/" + nameof(MonsterSettings), order = 1)]
public class MonsterSettings : ScriptableObject {

    public MonsterVariant[] monsterVariants;
}

public enum MonsterVariantID {
    Unspecified = -1,
    Melee = 0,
    Ranged = 1,
    Swarm = 2,
}

public abstract class MonsterVariant : ScriptableObject {
    public MonsterVariantID identifier;
    public Sprite sprite;
    public int count = 1;
    public float width = 1f;
    public float height = 1f;
    public float health = 10f;
    public float movementSpeed = 1f;
    public int cost = 60;

    public abstract IMonsterController[] CreateControllers(MonsterSettings monsterSettings, ArenaData arenaData, Vector2 position);

    protected MonsterData[] CreateMonsterData(MonsterSettings monsterSettings, Vector2 position) {
        int team = PhotonNetwork.LocalPlayer.ActorNumber;
        MonsterData[] monstersData = new MonsterData[count];
        for(int i = 0; i < count; ++i) {
            monstersData[i] = new(monsterSettings, team, identifier, health, position, isSynced: true);
        }
        return monstersData;
    }
}
