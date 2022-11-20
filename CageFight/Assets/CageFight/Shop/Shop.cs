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

    private readonly Dictionary<CageSlot, Cage> cageSlots = new();
    private Cage currentCage;
    private GladiatorManager localPlayer;

    public enum CageEventType {
        BOUGHT,
        SOLD,
        BEGIN_CLICK,
        END_CLICK
    }

    private void Awake() {
        for(int i = 0; i < spawnPoints.Count; i++) {
            Transform spawnPointTransform = spawnPoints[i];
            Cage cage = Instantiate(cagePrefab, spawnPointTransform);
            cage.transform.localPosition = Vector3.zero;
            cage.transform.localRotation = Quaternion.identity;
            cage.Init(i);
            cage.CageEventAction += OnCageEvent;
        }

        foreach(CageSlot slot in slots) {
            slot.MouseOverAction += OnSlotMouseOver;
        }

        doneButton.ToggleReadyAction += OnReadyToggle;
        SetVisible(false);
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

    private void OnCageEvent(CageEventType type, Cage cage) {
        switch(type) {
            case CageEventType.BOUGHT:
                if(localPlayer.Money >= cage.Cost) {
                    localPlayer.RemoveMoney(cage.Cost);
                    if(TryFindVacantSlot(out CageSlot slot)) {
                        AttachCageToSlot(cage, slot);
                        cage.OnBought();
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
}
