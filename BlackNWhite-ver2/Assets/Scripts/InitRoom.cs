using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BestHTTP.SocketIO3;
using System;
using BestHTTP.SocketIO3.Events;
using TMPro;
using MiniJSON;

public class InitRoom : MonoBehaviour
{   
    public ScrollRect ParentSR;
    GameObject GameManager;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;

    PopupUI _popup;

    void Awake(){
        _popup = FindObjectOfType<PopupUI>();
    }

    void Start()
    {
        Debug.Log("InitRoom - Start!");
        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;
    }

    public void EnterBtnClick(){
        Debug.Log("EnterBtnClick");

        string room = transform.parent.gameObject.transform.Find("roomPin").GetComponent<TextMeshProUGUI>().text.ToString();   ;
        socketManager.Socket.Emit("isValidRoom", room);
        socketManager.Socket.On("room permission", (string data) => CheckRoomPermission(data));
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
                Debug.Log("POPUP- " + _popup);
                // _popup.POP("This Room is Full! Try Another Room.");
                _popup.POP("방이 꽉 찼습니다.");
                break;
            case -1 :
                Debug.Log("[socket- room permission -1]");
                Debug.Log("POPUP- " + _popup);
                _popup.POP("존재하지 않는 방입니다.");
                break;
            default:
                Debug.Log("[socket- room permission ERROR]");
                break;
        }
    }

    // 버튼으로 인해 먹지 않는 스크롤 문제 해결
    
// public class InitRoom : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
// {   
    // private void Awake()
    // {
    //     Debug.Log("[AWAKE]");

    //     ParentSR = transform.parent.parent.parent.parent.GetComponent<ScrollRect>();
    //     Debug.Log("[ParentSR]" + ParentSR);
    // }
    
    // public void OnBeginDrag(PointerEventData e)
    // {
    //     ParentSR.OnBeginDrag(e);
    // }
    // public void OnDrag(PointerEventData e)
    // {
    //     ParentSR.OnDrag(e);
    // }
    // public void OnEndDrag(PointerEventData e)
    // {
    //     ParentSR.OnEndDrag(e);
    // }
    


}
