using Photon.Pun;
using System;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviourPun, IPunInstantiateMagicCallback, IPunObservable {

    public Action<MonsterBehaviour> Died;
    public MonsterData Data { get; private set; }

    [SerializeField]
    private GameObject visuals;
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private MonsterSettings monsterSettings;

    private IMonsterController monsterController;

    private bool isFirstNetworkRead = true;
    private float networkPositionDelta = 0f;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        Data = MonsterData.FromObjectArray(monsterSettings, info.photonView.InstantiationData);
        Debug.Log($"Instantiated Monster on team {Data.Team}");
        MonsterList.Instance.AddMonster(this);

        visuals.transform.localScale = new Vector3(Data.Width, Data.Height, Data.Width);
        Vector3 position = visuals.transform.localPosition;
        position.y = 0f;
        visuals.transform.localPosition = position;

        sprite.sprite = Data.Sprite;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            stream.SendNext(Data.position);
        }
        else if(stream.IsReading) {
            Vector2 prevPosittion = Data.position;

            Data.position = (Vector2)stream.ReceiveNext();

            if(isFirstNetworkRead) {
                Data.isSynced = true;
                transform.position = new Vector3(Data.position.x, transform.position.y, Data.position.y);
                networkPositionDelta = 0;
                isFirstNetworkRead = false;
            }
            else {
                networkPositionDelta = Vector2.Distance(prevPosittion, Data.position);
            }
        }
    }

    private void OnDestroy() {
        Debug.Log($"Destroyed Monster on team {Data.Team}");
        MonsterList.Instance.RemoveMonster(this);
        if(monsterController != null) {
            monsterController.OnDeath -= OnDeath;
        }
    }

    public void SetController(IMonsterController monsterController) {
        Data = monsterController.Data;
        this.monsterController = monsterController;
        monsterController.OnDeath += OnDeath;
    }

    public void ReceiveDamage(float damage) {
        if(photonView.IsMine) {
            monsterController.ReceiveDamage(damage);
        }
        else {
            photonView.RPC(nameof(ReceiveDamageRPC), photonView.Owner, damage);
        }
    }

    [PunRPC]
    public void ReceiveDamageRPC(float damage) {
        ReceiveDamage(damage);
    }

    public void OnDeath(IMonsterController _) {
        Died?.Invoke(this);
        PhotonNetwork.Destroy(gameObject);
    }

    private void LateUpdate() {
        Vector3 targetPosition = new(Data.position.x, transform.position.y, Data.position.y);
        if(photonView.IsMine) {
            transform.position = targetPosition;
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, networkPositionDelta * Time.deltaTime * PhotonNetwork.SerializationRate);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(!photonView.IsMine) {
            return;
        }

        other.GetComponent<ArenaCenter>().Register(this);
    }

    private void OnTriggerExit(Collider other) {
        other.GetComponent<ArenaCenter>().Deregister(this);
    }
}
