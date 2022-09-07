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

public class RoomListsSocket : MonoBehaviour
{
    
    public Button EnterBtn;
    public Button CreateRoomBtn;
    public TMP_InputField  inputFieldPIN;
    public string inputPIN;
    public GameObject textSocketIO;
    public GameObject textSessionID;
    public GameObject textRoomPermission;
    GameObject GameManager;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;
    public string playerInfo;
    public GameObject socketScript;

    // Start is called before the first frame update
    void Start()
    {
        inputFieldPIN = GameObject.Find("InputPIN (TMP)").GetComponent<TMP_InputField>();
        
        GameManager = GameObject.Find("Socket");
     
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        // socket ID 화면에 출력 
        // textSocketIO.GetComponent<TextMeshProUGUI>().text = socketManager.Socket.Id;
        socketManager.Socket.Emit("check session");
        // socketManager.Socket.On("session", (string data) => session(data));     

        Debug.Log("RoomLists Socket");
        // socketManager.Socket.Emit("testtest", "testtestwaiting");
    }

    private void session(string data)
    {
        Debug.Log("session data : " + data);

        var session = Json.Deserialize(data) as Dictionary<string, object>;
        Debug.Log("!session session : " + session);
        // Debug.Log("session.sessionID : " + (string) (((Dictionary<string, object>) ((List<object>) session["sessionID"])[0]));
    
        // textSession.GetComponent<TextMeshProUGUI>().text = socketManager.Socket.Id; // session ID
    
    }
  

}
