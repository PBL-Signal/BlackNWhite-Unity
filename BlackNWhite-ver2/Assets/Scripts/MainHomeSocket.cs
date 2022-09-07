using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SocketIO3;
using System;
using BestHTTP.SocketIO3.Events;
using TMPro;
using MiniJSON;

public class MainHomeSocket : MonoBehaviour
{
    GameObject GameManager;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;
    public TMP_InputField  inputFieldPIN;
    public GameObject textRoomPermission;
    public GameObject socketScript;

    GameObject scrollViewContent; 
    public GameObject roomData;

    public GameObject nickname;
    public GameObject CntPublicRooms;

    PopupUI _popup;

    void Awake(){
        Debug.Log("Awake - MainHome");
        _popup = FindObjectOfType<PopupUI>();
    }

    void Start()
    {
        Debug.Log("!! MainHome Socket");

        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;


        scrollViewContent = GameObject.Find("Content");
        Debug.Log("!! scrollViewContent " + scrollViewContent);

        socketManager.Socket.Emit("checkSession");
        socketManager.Socket.On("sessionInfo", (string data) => sessionInfo(data));

        ClickRefreshRoomList();
    }


    public void ClickRefreshRoomList(){
        Debug.Log("ClickRefreshRoomList");
        socketManager.Socket.Emit("getPublcRooms");
        socketManager.Socket.On("loadPublicRooms",  (List<object> publicRooms) => loadPublicRooms(publicRooms));
    }



    void sessionInfo(string data){
        Debug.Log("!! sessionInfo : " + data);
            
        var session = Json.Deserialize(data) as Dictionary<string, object>;
        Debug.Log("[SessionInfo] nickname: " + session["nickname"]);

        if (nickname){
            nickname.GetComponent<TextMeshProUGUI>().text = session["nickname"].ToString();
        }
    }

    void loadPublicRooms(List<object> publicRooms){

        Debug.Log("[socket] loadPublicRooms");
        

        // 1. 리스트 비워주기 
        // transform [] itemList = scrollViewContent.GetComponentInChildren<transform>();
        if (scrollViewContent!= null){
            var itemList = scrollViewContent.GetComponentsInChildren<Transform>();
            Debug.Log("[itemList] " + itemList);
            
            foreach (var item in itemList)
            {
                // 부모 object는 삭제하지 않음
                if(item !=  scrollViewContent.transform)
                {
                    Debug.Log("[MainHomeSocket] Destroy gameObject(Item)");
                    Destroy(item.gameObject);
                }
            }
       
            
            // Debug.Log("[check2!!] " );
            
            // 2. 데이터 불러와서 scrollView Item으로 넣어주기
            var cnt = 0;
            foreach( Dictionary<string, object> room in publicRooms)
            {
                // Debug.Log("[loadPublicRooms] room " + room);
                // Debug.Log("[loadPublicRooms] room.roomPin  " + room["roomPin"]);
                // Debug.Log("[loadPublicRooms] room.userCnt  " + room["userCnt"]);
            
                cnt += 1;
                var roomItem = Instantiate(roomData, transform);
                roomItem.transform.Find("roomPin").GetComponent<TextMeshProUGUI>().text = room["roomPin"].ToString();
                roomItem.transform.Find("userCnt").GetComponent<TextMeshProUGUI>().text = room["userCnt"].ToString();
                roomItem.transform.Find("maxPlayer").GetComponent<TextMeshProUGUI>().text = room["maxPlayer"].ToString();
                roomItem.transform.SetParent(scrollViewContent.transform);
            }

            CntPublicRooms.GetComponent<TextMeshProUGUI>().text =  cnt.ToString();
        }
       
        
    }

    public void ClickRandomGameStartBtn(){

        Debug.Log("ClickRandomGameStartBtn");

        socketManager.Socket.Emit("randomGameStart");
        socketManager.Socket.On("enterPublicRoom", () => {
            // DontDestroyOnLoad(socketScript);
            SceneManager.LoadScene("WaitingRoomNew");
        });
        
    }

    public void EnterBtnClick(){
        string room = this.inputFieldPIN.text;
        Debug.Log("EnterBtnClick, room : " +  room);

        if (string.IsNullOrEmpty(room)){
            _popup.POP("룸 핀 번호(5자리)를 입력해주세요.");
        }
        else{
            socketManager.Socket.Emit("isValidRoom", room);
            socketManager.Socket.On("room permission", (string data) => CheckRoomPermission(data));
        }
    }

    public void CreateRoomBtnClick(){
        Debug.Log("CreateRoomBtnClick");
        SceneManager.LoadScene("CreateRoom");
    }

    void CheckRoomPermission(string data)
    {
        // var roomPermission = Json.Deserialize(data) as Dictionary<string, object>;
        // var permission= Int32.Parse(roomPermission["permission"].ToString());
        var permission= Int32.Parse(data);

        switch (permission)
        {
            case 1 :
                Debug.Log("[socket- room permission 1]");
                SceneManager.LoadScene("WaitingRoomNew");
                break;
            case 0 :
                Debug.Log("[socket- room permission 0]");
                // Debug.Log("POPUP- " + _popup);
                // _popup.POP("This Room is Full! Try Another Room.");
                _popup.POP("방이 꽉 찼습니다.");
                break;
            case -1 :
                Debug.Log("[socket- room permission -1]");
                // Debug.Log("POPUP- " + _popup);
                _popup.POP("존재하지 않는 방입니다.");
                break;
            default:
                Debug.Log("[socket- room permission ERROR]");
                break;
        }
    }


    public void OnDestroy()
    {
        Debug.Log("MainHomeSocket - OnDestroy!!");
    }

}
