using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomName;
    public TMP_Text connectInfo;
    public Button exitBtn;
    void Awake()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        CreatePlayer();
        SetRoomInfo();
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
    void SetRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount} / {room.MaxPlayers})";
    }
    public void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }
    //포톤 룸에서 퇴장했을때 호출되는 콜백 함수
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    //룸으로 새로운 네트워크 유저가 접속했을때 호출되는 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
    }
}
