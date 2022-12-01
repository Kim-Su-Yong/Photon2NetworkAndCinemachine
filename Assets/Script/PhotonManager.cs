using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    //���� ����
    private readonly string version = "1.0";
    //������ �г���
    private string userID = "ZZang";
    public TMP_InputField userIF;
    public TMP_InputField roomNameIF;
    //ArrayList hashtabe, List Dictionary //Ű�� ���� �ѽ��� �̷��.
    public Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    //���Ͽ� ���� �����͸� �����ϱ� ���� ��ųʸ� �ڷ���
    //������ ǥ���� ������
    private GameObject roomItemPrefab;
    //RoomItem �������� �߰��� scrollCount
    public Transform scrollContent;
    void Awake()
    {
        //������ Ŭ���̾�Ʈ �� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.AutomaticallySyncScene = true;
        //���� ���� ����
        PhotonNetwork.GameVersion = version;
        //���� ������ �г��� ����
        PhotonNetwork.NickName = userID;
        //���� �������� �������� �ʴ� ���� Ƚ��
        Debug.Log(PhotonNetwork.SendRate);
        //���� ���� ����
        if(PhotonNetwork.IsConnected == false)
            PhotonNetwork.ConnectUsingSettings();
        //RoomItem ������ �ε�
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");
    }
    private void Start()
    {
        //����� �������� �ε�
        userID = PlayerPrefs.GetString("USER_ID", $"USER_ID");
        userIF.text = userID;
        //���������� �г��� ���
        PhotonNetwork.NickName = userID;
    }
    //���� ������ ������ ȣ��Ǵ� �ݹ��Լ�
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
        //������ ����
        PlayerPrefs.SetString("USER_ID", userID);
        //���� ������ �г��� ���
        PhotonNetwork.NickName = userID;
    }
    //�� ���� �Է� ���θ� Ȯ���ϴ� ����
    string SetRoomName()
    {
        if(string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_{Random.Range(1, 101):000}";
        }
        return roomNameIF.text;
    }

    //�κ� ���� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom(); //������ ������
    }
    //�������� �������� �������� ��� ȣ��Ǵ� �ݹ��Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");
        OnMakeRoomClick(); //�޼��带 ȣ���� ����
        //RoomOptions ro = new RoomOptions();
        //ro.MaxPlayers = 20;
        //ro.IsOpen = true;
        //ro.IsVisible = true;
        ////�� ����
        //PhotonNetwork.CreateRoom("My Room", ro);

    }
    //�� ������ �Ϸ�� �� ȣ��Ǵ� �ݹ��Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($" RoomName : {PhotonNetwork.CurrentRoom.Name}");
    }
    //�뿡 ������ �� ȣ��Ǵ� �ݹ��Լ�
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
            //���� �̵��ϴ� ���� ����Ŭ���� ������ �޽��� ���� �ߴ�
            PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel("BattleField");
        }
    }
    //�� ����� �����ϴ� �ݹ� �Լ�
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //������ RoomItem �������� ������ �ӽ� ����
        GameObject tempRoom = null;
        foreach(var roomInfo in roomList)
        {
            //Debug.Log($"Room ={room.Name}({room.PlayerCount})/{room.MaxPlayers}");
            //�� ������ ��� 
            if(roomInfo.RemovedFromList == true)
            {   //��ųʸ����� ���̸����� �˻��� ����� RoomItem �������� ����
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                //RoomItem ������ ����
                Destroy(tempRoom);
                //��ųʸ����� �ش� �� �̸��� �����͸� ����
                rooms.Remove(roomInfo.Name);
            }
            else //�������� ����� ��� 
            {
                //���̸��� ��ųʸ��� ���� ��� ���� �߰�
                if(rooms.ContainsKey(roomInfo.Name)==false)
                {
                    //RoomInfo �������� scrollContents ������ ����
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    //�� ������ ǥ���ϱ� ���� RoomInfo ���� ����
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    //��ųʸ� �ڷ����� ������ �߰�
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else //���̸��� ��ųʸ��� ���� ��쿡 �� ������ ����
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
        SetUserID(); //������ ����
        //�������� ������ ������ ����
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnMakeRoomClick()
    {
        SetUserID(); //������ ����
        //���� �Ӽ� ����
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        //�� ����
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }
    #endregion
}
