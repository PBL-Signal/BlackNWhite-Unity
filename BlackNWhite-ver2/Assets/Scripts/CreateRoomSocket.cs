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
using System.Linq;

public class CreateRoomSocket : MonoBehaviour
{
    public Button CreateRoomBtn;

    public GameObject textSocketIO;
    public GameObject textInfo;
    public GameObject Roomtype;
    private ToggleGroup roomTypeToggleGrp;
    public GameObject socketScript;
 
    public TMP_Dropdown dropdown;
    GameObject GameManager;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;
    public string roomPin;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("CreateRoom Socket");
        
        // roomTypeToggleGrp = GetComponent<ToggleGroup>();
        roomTypeToggleGrp = Roomtype.GetComponent<ToggleGroup>();
        Debug.Log("CreateRoom roomTypeToggleGrp - "+ roomTypeToggleGrp);

        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;
        Debug.Log("CreateRoom GameManager - "+ socketManager);
        
        // socket ID 화면에 출력 
        textSocketIO.GetComponent<TextMeshProUGUI>().text = socketManager.Socket.Id;
    }

    
   public void CreateRoomBtnClick(){

    
        Debug.Log("CreateRoomBtnClick");
        Debug.Log("Dropdown Value: "+ dropdown.options[dropdown.value].text);

        Debug.Log("CreateRoom2 roomTypeToggleGrp - "+ roomTypeToggleGrp);

        Toggle toggle = roomTypeToggleGrp.ActiveToggles().FirstOrDefault();
        Debug.Log("It worked! " + toggle.gameObject.name);

        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("maxPlayer", dropdown.options[dropdown.value].text) ;

        if (toggle.gameObject.name.ToString() == "public"){
            arg.Add("roomType", "public");
        }else{
            arg.Add("roomType", "private");
        }
        

        socketManager.Socket.Emit("createRoom", arg);
        
        //새코드
        socketManager.Socket.On("succesCreateRoom", (string data) => OnsuccesCreateRoom(data));
            
            
    }
   
    
    
    void OnsuccesCreateRoom(string data)
    {   
        Debug.Log("OnsuccesCreateRoom");
        Debug.Log("OnsuccesCreateRoom_roompin : " +data);

       
        SceneManager.LoadScene("WaitingRoomNew");
        //  var roomPermission = Json.Deserialize(data) as Dictionary<string, object>;
    }


    public void HomeClick(){    
        Debug.Log("HomeClick");
        // DontDestroyOnLoad(socketScript);
        SceneManager.LoadScene("MainHome");
    }

    public void OnDestroy()
    {
        Debug.Log("CreateRoomSocket - OnDestroy!!");
    }

    [System.Serializable]
    class SuccesCreateRoom {
        public string roomPin;
    }


}
