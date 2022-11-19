using Photon.Pun;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviourPun, IPunInstantiateMagicCallback, IPunObservable {

    private MonsterData monsterData;

    private bool isFirstNetworkRead = true;
    private float networkPositionDelta = 0f;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        monsterData = MonsterData.FromObjectArray(info.photonView.InstantiationData);
        Debug.Log($"Instantiated Monster on team {monsterData.Team}");
        if(!photonView.IsMine) {
            MonsterList.Instance.AddMonster(monsterData);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            stream.SendNext(monsterData.position);
        }
        else if(stream.IsReading) {
            Vector2 prevPosittion = monsterData.position;

            monsterData.position = (Vector2)stream.ReceiveNext();

            if(isFirstNetworkRead) {
                transform.position = new Vector3(monsterData.position.x, transform.position.y, monsterData.position.y);
                networkPositionDelta = 0;
                isFirstNetworkRead = false;
            }
            else {
                networkPositionDelta = Vector2.Distance(prevPosittion, monsterData.position);
            }
        }
    }

    private void OnDestroy() {
        Debug.Log($"Destroyed Monster on team {monsterData.Team}");
        MonsterList.Instance.RemoveMonster(monsterData);
    }

    // Used on owner side
    public void SetMonsterData(MonsterData monsterData) {
        this.monsterData = monsterData;
        MonsterList.Instance.AddMonster(this.monsterData);
    }

    private void LateUpdate() {
        Vector3 targetPosition = new(monsterData.position.x, transform.position.y, monsterData.position.y);
        if(photonView.IsMine) {
            transform.position = targetPosition;
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, networkPositionDelta * Time.deltaTime * PhotonNetwork.SerializationRate);
        }
    }
}
