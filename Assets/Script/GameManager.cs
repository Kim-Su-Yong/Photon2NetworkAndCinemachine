using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviour
{

    void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        CreatePlayer();
    }
    void CreatePlayer()
    {
        //출현 위치 정보를 배열에 저장
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        //네트워크 상에서 캐릭터 생성
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
        //마스터 클라이언트인 경우에 룸에 입장한 후 전투씬에 로딩한다.
    }
}
