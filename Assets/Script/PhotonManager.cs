using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    //게임 버전
    private readonly string version = "1.0";
    //유저의 닉네임
    private string userID = "ZZang";
    public TMP_InputField userIF;
    public TMP_InputField roomNameIF;
    //ArrayList hashtabe, List Dictionary //키와 값이 한쌍을 이룬다.
    public Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    //룸목록에 대한 데이터를 저장하기 위한 딕셔너리 자료형
    //룸목록을 표시할 프리팹
    private GameObject roomItemPrefab;
    //RoomItem 프리팹이 추가될 scrollCount
    public Transform scrollContent;
    void Awake()
    {
        //마스터 클라이언트 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        //게임 버전 설정
        PhotonNetwork.GameVersion = version;
        //접속 유저의 닉네임 설정
        PhotonNetwork.NickName = userID;
        //포톤 서버와의 데이터의 초당 전송 횟수
        Debug.Log(PhotonNetwork.SendRate);
        //포톤 서버 접속
        if(PhotonNetwork.IsConnected == false)
            PhotonNetwork.ConnectUsingSettings();
        //RoomItem 프리팹 로드
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");
    }
    private void Start()
    {
        //저장된 유저명을 로드
        userID = PlayerPrefs.GetString("USER_ID", $"USER_ID");
        userIF.text = userID;
        //접속유저의 닉네임 등록
        PhotonNetwork.NickName = userID;
    }
    //포톤 서버에 접속후 호출되는 콜백함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }
    public void SetUserID()
    {
        if(string.IsNullOrEmpty(userIF.text))
        {
            userID = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userID = userIF.text;
        }
        //유저명 저장
        PlayerPrefs.SetString("USER_ID", userID);
        //접속 유저의 닉네임 등록
        PhotonNetwork.NickName = userID;
    }
    //룸 명의 입력 여부를 확인하는 로직
    string SetRoomName()
    {
        if(string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_{Random.Range(1, 101):000}";
        }
        return roomNameIF.text;
    }

    //로비에 접속 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom(); //무작위 방접속
    }
    //랜덤으로 룸입장이 실패했을 경우 호출되는 콜백함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");
        OnMakeRoomClick(); //메서드를 호출할 예정
        //RoomOptions ro = new RoomOptions();
        //ro.MaxPlayers = 20;
        //ro.IsOpen = true;
        //ro.IsVisible = true;
        ////룸 생성
        //PhotonNetwork.CreateRoom("My Room", ro);

    }
    //룸 생성이 완료된 후 호출되는 콜백함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($" RoomName : {PhotonNetwork.CurrentRoom.Name}");
    }
    //룸에 입장한 후 호출되는 콜백함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($" PlayerCount + : {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName},{player.Value.ActorNumber}");
        }
        if(PhotonNetwork.IsMasterClient)
        {
            //씬이 이동하는 동안 포톤클라우드 서버의 메시지 수신 중단
            PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel("BattleField");
        }
    }
    //룸 목록을 수신하는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //삭제된 RoomItem 프리팹을 저장할 임시 변수
        GameObject tempRoom = null;
        foreach(var roomInfo in roomList)
        {
            //Debug.Log($"Room ={room.Name}({room.PlayerCount})/{room.MaxPlayers}");
            //룸 삭제된 경우 
            if(roomInfo.RemovedFromList == true)
            {   //딕셔너리에서 룸이름으로 검색해 저장된 RoomItem 프리팹을 추출
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                //RoomItem 프리팹 삭제
                Destroy(tempRoom);
                //딕셔너리에서 해당 룸 이름의 데이터를 삭제
                rooms.Remove(roomInfo.Name);
            }
            else //룸정보가 변경된 경우 
            {
                //룸이름이 딕셔너리에 없는 경우 새로 추가
                if(rooms.ContainsKey(roomInfo.Name)==false)
                {
                    //RoomInfo 프리팹을 scrollContents 하위에 생성
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    //룸 정보를 표시하기 위해 RoomInfo 정보 전달
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    //딕셔너리 자료형에 데이터 추가
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else //룸이름이 딕셔너리에 없는 경우에 룸 정보를 갱신
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
            Debug.Log($"Room = {roomInfo.Name} ({roomInfo.PlayerCount} / {roomInfo.MaxPlayers})");
        }
    }
    #region UI_BUTTON_EVENT
    public void OnLoginClick()
    {
        SetUserID(); //유저명 저장
        //무작위로 추출한 룸으로 입장
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnMakeRoomClick()
    {
        SetUserID(); //유저명 저장
        //룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //룸 생성
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }
    #endregion
}
