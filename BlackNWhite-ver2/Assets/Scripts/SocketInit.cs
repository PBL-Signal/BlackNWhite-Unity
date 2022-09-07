using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using UnityEngine.UI;
using TMPro;
using System;
using BestHTTP.SocketIO3.Transports;

public class SocketInit : MonoBehaviour
{

    public TMP_InputField  inputFieldNickname;
    public TMP_InputField  inputFieldPIN;

    public string socketId;
    // public GameObject textSocketIO;

    public string inputNickname;
    public string inputPIN;

    public string playerInfo;

    public GameObject socketScript;

    private string address = "";
    public SocketManager socketManager;
    private Button BtnEnter;

    String sessionID ; // <변경 필요> localstorage로 바꿔야 함

    void Start()
    {
        Application.runInBackground = true;
        
        // SocketIO3Connect();
        if(!string.IsNullOrWhiteSpace(sessionID)){ // 이거 안하면 서버 재실행 할 때마다 session이 새로 생성됨
            Debug.Log("sessionID 있음");
            SocketIO3Connect("sessionID", sessionID);
        }

        BtnEnter =  GameObject.Find("BtnEnter").GetComponent<Button>();
        inputFieldNickname = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        //inputFieldPIN = GameObject.Find("InputPIN (TMP)").GetComponent<TMP_InputField>();
    }
     private void SocketIO3Connect(String optionName, String optionValue)
    {
        // 소켓 옵션 설정 
        Debug.Log("SocketIO3Connect 호출됨!@!@");

        
        SocketOptions options = new SocketOptions();
        options.AutoConnect = true;
        options.ConnectWith = TransportTypes.WebSocket;
        if (optionName.Equals("sessionID")){
            options.Auth = (socketManager, socket) => new { sessionID = optionValue }; // 페이로드 닉네임으로 설정
        }
        if (optionName.Equals("username")){
            options.Auth = (socketManager, socket) => new { username = optionValue }; // 페이로드 닉네임으로 설정
        }

        // 소켓 옵션으로 소켓 연결 
        socketManager = new SocketManager(new System.Uri("http://127.0.0.1:8080"), options);
        // socketManager = new SocketManager(new System.Uri("http://mylb-1262334413.us-east-1.elb.amazonaws.com/"), options);
  
     
        // 소켓 on 코드
        socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);


        // socketManager.Socket.On("PlayerConnected", (string data) => PlayerConnected(data));     
        //socketManager.Socket.On("session", (string data) => session(data));     
        socketManager.Socket.On("connect", () => {
               // textSocketIO.GetComponent<TextMeshProUGUI>().text = socketManager.Socket.Id;
                socketId = socketManager.Socket.Id;
                Debug.Log("HANDSHAKE" + socketManager.Handshake.Sid);
                DontDestroyOnLoad(socketScript);
                SceneManager.LoadScene("MainHome");
                SceneManager.LoadScene("ActionBar", LoadSceneMode.Additive);
        });

        // AutoConnect is turned off, Open must be called
        //socketManager.Open();
    }

    void Update() {
        // socketManager.Open();
    }

     private void OnNameSpaceConnected(ConnectResponse res)
    {
        Debug.Log("OnNameSpaceConnected! Socket.IO" + res);
    }


    private void OnConnected(ConnectResponse res)
    {
        Debug.Log("Connected! Socket.IO" + res);
    }

    private void PlayerConnected(string data)
    {
        Debug.Log("Player Connected!! data : " + data);
    }

    private void session(string data)
    {
        Debug.Log("[socketInit] session data : " + data);
        // Debug.Log("session sessionID : " + data.sessionID + " userID : "+ data.userID + " username : " + data.username );
        
        // attach the session ID to the next reconnection attempts
        // socket.auth = { sessionID };
        // store it in the localStorage
        // save the ID of the user
        // socket.userID = userID;

        // SceneManager.LoadScene("MainHome");
        // DontDestroyOnLoad(socketScript);

    }
    
     public void EnterBtnClick(){

        Debug.Log("EnterBtnClick");

        Debug.Log("name"+ this.inputFieldNickname.text);
        inputNickname = this.inputFieldNickname.text;

       if (String.IsNullOrWhiteSpace(inputNickname)){
            Debug.Log("NULL임!");
       }else{
            // SocketIO3Connect(inputNickname);
            SocketIO3Connect("username", inputNickname);
            BtnEnter.interactable  = false;
        }
   
        // socketManager.Socket.auth = inputNickname;
        // socketManager.Socket.connect();

        //socketManager.Socket.On("PlayersData", (string data) => playerInfo = data);
        
        
        // socketManager.Socket.Emit("check session");  
        // socketManager.Socket.On("session", (string data) => session(data));     
       

        //emit nickname, PIN (JSON)
        // Dictionary<string, object> arg = new Dictionary<string, object>();
        // arg.Add("nickname", this.inputFieldNickname.text);
        // socketManager.Socket.Emit("join", arg);

        // DontDestroyOnLoad(socketScript);
        // SceneManager.LoadScene("MainHome");

         
    }
    // void ReceivePlayerInfo(string data)
    // {
    //     var playerinfo = Json.Deserialize(data) as Dictionary<string, object>;
    //     Debug.Log("playerinfo.nickname : " + (string) (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[0])["nickname"]));


    //     // PlayerData player = JsonUtility.FromJson<PlayerData>(data);
    //     // string player_str = JsonUtility.ToJson(player);
    //     // Debug.Log("player_str : " + player_str);
    //     // Data playerinfo = JsonUtility.FromJson<Data>(data);
    //     // Debug.Log("playerinfo.nickname : " + playerinfo.palyer[0].nickname);

    // }
    private void Destory()
    {
        Debug.Log("SocketInit - Destroy!!");
        if (this.socketManager != null)
        {
            this.socketManager.Close();
            this.socketManager = null;
        }
    }
}