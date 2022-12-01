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
    //포톤 매니저에 접근 변수
    private PhotonManager photonManager;
    //프로퍼티의 정의
    public RoomInfo RoomInfo
    {
        get { return _roomInfo; }
        set 
        {
            _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name}({_roomInfo.PlayerCount} / {_roomInfo.MaxPlayers})";
            //버튼 클릭 이벤트에 함수 연결 동적 할당으로 연결
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
        //룸접속
        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }
    void Update()
    {
        
    }
}
