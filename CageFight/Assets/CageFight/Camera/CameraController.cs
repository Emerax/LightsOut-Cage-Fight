using System.Collections;
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

    [SerializeField]
    private float shopLerpTime = 2f;

    private Camera cameraObject;

    private float speed = 0f;
    private bool viewShop = false;
    private ArenaSegment shopSegment;
    private Vector3 preShopPosition;
    private Quaternion preShopRotation;

    private void Awake() {
        cameraObject = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    private void Update() {
        UpdateInput();
        UpdateTransform();
    }

    private void UpdateInput() {
        if(viewShop) {
            return;
        }

        float frameAcceleration = 0;
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            //Go left
            frameAcceleration = acceleration * Time.deltaTime;
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            //Go right
            frameAcceleration = -acceleration * Time.deltaTime;
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
        if(viewShop) {
            return;
        }
        transform.position = Quaternion.Euler(new Vector3(0f, speed * Time.deltaTime, 0f)) * (transform.position - arenaTransform.position) + arenaTransform.position;
        cameraObject.transform.LookAt(arenaTransform);
    }

    public void SetShopTarget(ArenaSegment segment) {
        viewShop = true;
        shopSegment = segment;
        Transform target = shopSegment.CameraHolderTransform;
        preShopPosition = transform.position;
        preShopRotation = cameraObject.transform.rotation;
        StartCoroutine(DoLerpToTarget(transform.position, target.position, cameraObject.transform.rotation, target.rotation, shopLerpTime));
    }

    public void SetArenaTarget() {
        StartCoroutine(DoLerpAndThenSetViewShop(transform.position, preShopPosition, cameraObject.transform.rotation, preShopRotation, shopLerpTime));
    }

    private IEnumerator DoLerpAndThenSetViewShop(Vector3 originPos, Vector3 targetPos, Quaternion originRot, Quaternion targetRot, float lerpTime) {
        yield return StartCoroutine(DoLerpToTarget(originPos, targetPos, originRot, targetRot, lerpTime));
        viewShop = false;
    }

    private IEnumerator DoLerpToTarget(Vector3 originPos, Vector3 targetPos, Quaternion originRot, Quaternion targetRot, float lerpTime) {
        float timeSinceStart = 0f;
        while(timeSinceStart < lerpTime) {
            timeSinceStart += Time.deltaTime;
            float t = timeSinceStart / lerpTime;
            transform.position = Vector3.Lerp(originPos, targetPos, t);
            cameraObject.transform.rotation = Quaternion.Lerp(originRot, targetRot, t);
            yield return null;
        }
    }
}
