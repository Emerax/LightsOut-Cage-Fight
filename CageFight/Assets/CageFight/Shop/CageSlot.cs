using System;
using UnityEngine;

public class CageSlot : MonoBehaviour {
    [SerializeField]
    private Transform attachPoint;

    public Action<CageSlot> MouseOverAction;

    public Transform AttachPoint { get => attachPoint; }

    private void OnMouseEnter() {
        MouseOverAction?.Invoke(this);
    }
}
