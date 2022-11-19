using System;
using System.Collections.Generic;
using UnityEngine;
using static Shop;

public class Cage : MonoBehaviour {
    private int slotNumber;
    public Action<CageEventType, Cage> CageEventAction;

    private List<IMonsterController> monsters;
    private bool bought = false;

    private void OnMouseDown() {
        if(bought) {
            CageEventAction?.Invoke(CageEventType.BEGIN_CLICK, this);
        }
        else {
            CageEventAction?.Invoke(CageEventType.BOUGHT, this);
        }
    }

    private void OnMouseUp() {
        if(bought) {
            CageEventAction?.Invoke(CageEventType.END_CLICK, this);
        }
    }

    public void Init(int slot) {
        slotNumber = slot;
    }

    public void OnBought() {
        bought = true;
    }
}
