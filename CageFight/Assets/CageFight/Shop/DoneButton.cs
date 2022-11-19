using System;
using UnityEngine;

public class DoneButton : MonoBehaviour {
    public Action<bool> ToggleReadyAction;
    private bool ready = false;

    private Vector3 originalPosition;

    private void Awake() {
        originalPosition = transform.position;
    }

    private void OnMouseDown() {
        ready = !ready;
        transform.position = ready ? originalPosition - Vector3.up * 0.5f : originalPosition;
        ToggleReadyAction?.Invoke(ready);
    }

    public void ResetButton() {
        ready = false;
        transform.position = originalPosition;
    }
}
