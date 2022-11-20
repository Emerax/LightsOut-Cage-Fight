using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {
    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private List<CageSlot> slots;

    [SerializeField]
    private Cage cagePrefab;

    [SerializeField]
    private DoneButton doneButton;

    [SerializeField]
    private Transform cameraHolder;

    public Transform CameraHolderTransform { get => cameraHolder; }

    [SerializeField]
    private ArenaData arenaData;

    [SerializeField]
    private MonsterSettings monsterSettings;

    [SerializeField]
    private GameObject shopParent;
    [SerializeField]
    private GameObject slotsParent;

    private readonly Dictionary<CageSlot, Cage> cageSlots = new();
    private readonly List<Cage> boughtCages = new();
    private Cage currentCage;
    private GladiatorManager localPlayer;

    public enum CageEventType {
        BOUGHT,
        SOLD,
        BEGIN_CLICK,
        END_CLICK,
        DESTROYED,
    }

    private void Awake() {
        for(int i = 0; i < spawnPoints.Count; i++) {
            SpawnCage(i);
        }

        foreach(CageSlot slot in slots) {
            slot.MouseOverAction += OnSlotMouseOver;
        }

        doneButton.ToggleReadyAction += OnReadyToggle;
        SetVisible(false);
    }

    public void SpawnCage(int slot) {
        Transform spawnPointTransform = spawnPoints[slot];
        Cage cage = Instantiate(cagePrefab, spawnPointTransform);
        cage.transform.localPosition = Vector3.zero;
        cage.transform.localRotation = Quaternion.identity;
        cage.Init(monsterSettings, arenaData);
        cage.CageEventAction += OnCageEvent;
    }

    public void SetVisible(bool isVisible) {
        gameObject.SetActive(isVisible);
    }

    public void OnNewShopPhase() {
        doneButton.ResetButton();
    }

    private void Update() {
        if(Input.GetMouseButtonUp(0)) {
            currentCage = null;
        }
    }

    public void SetLocalPlayer(GladiatorManager gladiatorManager) {
        localPlayer = gladiatorManager;
    }

    public void ToggleVisibility(bool visible) {
        shopParent.SetActive(visible);
        slotsParent.SetActive(visible);
    }

    private void OnCageEvent(CageEventType type, Cage cage) {
        switch(type) {
            case CageEventType.BOUGHT:
                if(localPlayer.Money >= cage.Cost) {
                    localPlayer.RemoveMoney(cage.Cost);
                    if(TryFindVacantSlot(out CageSlot slot)) {
                        AttachCageToSlot(cage, slot);
                        cage.OnBought();
                        boughtCages.Add(cage);
                    }
                }
                break;
            case CageEventType.SOLD:
                break;
            case CageEventType.BEGIN_CLICK:
                currentCage = cage;
                break;
            case CageEventType.END_CLICK:
                currentCage = null;
                break;
            case CageEventType.DESTROYED:
                boughtCages.Remove(cage);
                break;
            default:
                break;
        }
    }

    private void OnSlotMouseOver(CageSlot slot) {
        if(currentCage != null) {
            if(!cageSlots.TryGetValue(slot, out Cage cage) || cage == null) {
                ClearCageFromSlot(currentCage);
                AttachCageToSlot(currentCage, slot);
            }
        }
    }

    private void OnReadyToggle(bool ready) {
        localPlayer.ToggleReady(ready);
    }

    private bool TryFindVacantSlot(out CageSlot cageSlot) {
        foreach(CageSlot slot in slots) {
            if(!cageSlots.TryGetValue(slot, out Cage _)) {
                cageSlot = slot;
                return true;
            }
        }

        cageSlot = null;
        return false;
    }

    private void AttachCageToSlot(Cage cage, CageSlot slot) {
        cage.transform.position = slot.AttachPoint.position;
        cageSlots[slot] = cage;
    }

    private void ClearCageFromSlot(Cage cageToClear) {
        foreach(CageSlot slot in slots) {
            if(cageSlots.TryGetValue(slot, out Cage cage)) {
                if(cage == cageToClear) {
                    cageSlots[slot] = null;
                }
            }
        }
    }

    public void SpawnMonsters(MonsterManager monsterManager) {
        foreach(Cage cage in boughtCages) {
            int monsterCount = cage.Monsters.Count;
            for(int i = 0; i < monsterCount; ++i) {
                IMonsterController monster = cage.Monsters[i];
                Vector2 centerPos = new(cage.transform.position.x, cage.transform.position.z);
                if (monsterCount == 1) {
                    monster.Data.position = centerPos;
                }
                else {
                    float angle = i / (float)monsterCount * 2 * Mathf.PI;
                    monster.Data.position = centerPos + 0.4f * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }
                GameObject go = monsterManager.SpawnMonster(monster);
                go.GetComponent<MonsterBehaviour>().Died += (MonsterBehaviour _) => DestroyCage(cage);
            }
        }
    }

    public void DespawnMonsters() {
        foreach(MonsterBehaviour monster in MonsterList.Instance.GetMonstersOfTeam(localPlayer.Team)) {
            PhotonNetwork.Destroy(monster.gameObject);
        }
    }

    public void DebugSpawnMonsters(MonsterManager monsterManager, int team) {
        float angle = team * 0.25f * Mathf.PI;
        Vector2 axis = new(Mathf.Cos(angle), Mathf.Sin(angle));

        monsterManager.SpawnMonster(IMonsterController.Create(monsterSettings, MonsterVariantID.Melee, arenaData, 10f * axis)[0]);
        monsterManager.SpawnMonster(IMonsterController.Create(monsterSettings, MonsterVariantID.Ranged, arenaData, -10f * axis)[0]);
    }

    private void DestroyCage(Cage cage) {
        ClearCageFromSlot(cage);
        boughtCages.Remove(cage);
        Destroy(cage.gameObject);
    }
}
