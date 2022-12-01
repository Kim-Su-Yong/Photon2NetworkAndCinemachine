using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class RoomData : MonoBehaviour
{
    private RoomInfo _roomInfo;
    [SerializeField]
    private TMP_Text roomInfoText;
    //���� �Ŵ����� ���� ����
    private PhotonManager photonManager;
    //������Ƽ�� ����
    public RoomInfo RoomInfo
    {
        get { return _roomInfo; }
        set 
        {
            _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name}({_roomInfo.PlayerCount} / {_roomInfo.MaxPlayers})";
            //��ư Ŭ�� �̺�Ʈ�� �Լ� ���� ���� �Ҵ����� ����
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }
    void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }
    void OnEnterRoom(string roomName)
    {
        photonManager.SetUserID();

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //������
        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }
    void Update()
    {
        
    }
}
