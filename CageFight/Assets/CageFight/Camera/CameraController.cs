using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private Transform arenaTransform;

    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float speedDecay;
    [SerializeField]
    private float maxSpeed;

    private Camera cameraObject;

    private float speed = 0f;

    private void Awake() {
        cameraObject = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    private void Update() {
        UpdateInput();
        UpdateTransform();
    }

    private void UpdateInput() {
        float frameAcceleration = 0;
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            //Go left
            frameAcceleration = -acceleration * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            //Go right
            frameAcceleration = acceleration * Time.deltaTime;
        }

        if(frameAcceleration != 0f) {
            speed = Mathf.Clamp(speed + frameAcceleration, -maxSpeed, maxSpeed);
        }
        else {
            speed *= speedDecay;
            if(Mathf.Approximately(speed, 0f)) {
                speed = 0f;
            }
        }
    }

    private void UpdateTransform() {
        transform.position = Quaternion.Euler(new Vector3(0f, speed * Time.deltaTime, 0f)) * (transform.position - arenaTransform.position) + arenaTransform.position;
        cameraObject.transform.LookAt(arenaTransform);
    }
}
