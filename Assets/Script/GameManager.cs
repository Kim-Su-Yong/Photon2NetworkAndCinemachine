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
        //���� ��ġ ������ �迭�� ����
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        //��Ʈ��ũ �󿡼� ĳ���� ����
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
        //������ Ŭ���̾�Ʈ�� ��쿡 �뿡 ������ �� �������� �ε��Ѵ�.
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
    //���� �뿡�� ���������� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    //������ ���ο� ��Ʈ��ũ ������ ���������� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
    }
}
