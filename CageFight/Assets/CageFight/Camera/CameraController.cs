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
    private float defaultZoom;
    [SerializeField]
    private float maxZoom;
    [SerializeField]
    private float minZoom;
    [SerializeField]
    private float maxZoomDelta;

    [SerializeField]
    private float shopLerpTime = 2f;

    private Camera cameraObject;

    private float speed = 0f;
    private float zoomSpeed = 0f;
    private float zoom = 0f;
    private bool viewShop = false;
    private ArenaSegment shopSegment;
    private Vector3 postShopPosition;
    private Quaternion postShopRotation;

    private void Awake() {
        cameraObject = GetComponentInChildren<Camera>();
        zoom = defaultZoom;
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

        zoomSpeed = Input.mouseScrollDelta.y;
        zoomSpeed = Mathf.Clamp(zoomSpeed, -maxZoomDelta, maxZoomDelta);
    }

    private void UpdateTransform() {
        if(viewShop) {
            return;
        }
        zoom -= zoomSpeed;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        Vector3 zoomedOffset = zoom * (transform.position - arenaTransform.position).normalized;

        transform.position = Quaternion.Euler(new Vector3(0f, speed * Time.deltaTime, 0f)) * zoomedOffset + arenaTransform.position;
        cameraObject.transform.LookAt(arenaTransform);
    }

    public void SetShopTarget(Shop shop) {
        viewShop = true;
        Transform target = shop.CameraHolderTransform;
        zoom = defaultZoom;
        postShopPosition = zoom * (transform.position - arenaTransform.position).normalized;
        postShopRotation = cameraObject.transform.rotation;
        StartCoroutine(DoLerpToTarget(transform.position, target.position, cameraObject.transform.rotation, target.rotation, shopLerpTime));
    }

    public void SetArenaTarget() {
        StartCoroutine(DoLerpAndThenSetViewShop(transform.position, postShopPosition, cameraObject.transform.rotation, postShopRotation, shopLerpTime));
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
