using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using MiniJSON;

public class OnWaitingRoomButtonClick : MonoBehaviour
{
    GameObject GameManager;
    GameObject GameManager2;
    private SocketManager socketManager = null;
    private string socketId;
    private string playerInfoStr;
    private bool ready = false;
    private bool teamChange = false;
    private int colorIndex;

    // private Color32 red = new Color32(233, 19, 27, 255);
    // private Color32 orange = new Color32(236, 126, 14, 255);
    // private Color32 yellow = new Color32(248, 245, 88, 255);
    // private Color32 pink = new Color32(255, 119, 242, 255);
    // private Color32 lime = new Color32(191, 222, 54, 255);
    // private Color32 blue = new Color32(45, 172, 255, 255);
    // private Color32 navy = new Color32(34, 42, 121, 255);
    // private Color32 mint = new Color32(45, 233, 208, 255);
    // private Color32 purple = new Color32(184, 139, 255, 255);
    // private Color32 beige = new Color32(250, 222, 192, 255);
    // private Color32 white = new Color32(255, 255, 255, 255);
    // private Color32 black = new Color32(0, 0, 0, 255);

    // private Color32[] colors = new Color32[] {red, orange, yellow, pink, lime, blue, navy, mint, purple, beige, white, black};
    private Color32[] colors = new Color32[12] {new Color32(233, 19, 27, 255), new Color32(236, 126, 14, 255), 
                                                new Color32(248, 245, 88, 255), new Color32(255, 119, 242, 255), 
                                                new Color32(191, 222, 54, 255), new Color32(45, 172, 255, 255), 
                                                new Color32(34, 42, 121, 255), new Color32(45, 233, 208, 255), 
                                                new Color32(184, 139, 255, 255), new Color32(250, 222, 192, 255), 
                                                new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255)};


    System.Random rand = new System.Random();

    // var renderer = GetComonent<Renderer>();

    void Start()
    {
        GameManager = GameObject.Find("Socket");
        GameManager2 = GameObject.Find("MainHomeSocket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;
        socketId = GameManager.GetComponent<SocketInit>().socketId;
        playerInfoStr = GameManager.GetComponent<SocketInit>().playerInfo;
        Debug.Log("waiting room button : " + playerInfoStr);

        // GameManager = GameObject.Find("Canvas");
        // GameManager.GetComponent<LoadScene>().limitedTime.active = true;
    }

    public void OnReadyEnter()
    {
        Debug.Log("Click Ready Button");

        if (ready == false) {
            ready = true;
            teamChange = false;
            // gameObject.GetComponent<Image>().color = new Color32(162, 233, 54, 255);

        } else {
            ready = false;
            teamChange = false;
            // gameObject.GetComponent<Image>().color = new Color32(255, 121, 121, 255);

        }
        Debug.Log("[ready click]\nready status : " + ready + "\nteamChange : " + teamChange);
        
        var playerinfo = Json.Deserialize(playerInfoStr) as Dictionary<string, object>;
        
        for (int i = 0; i < 8; i++){
            if ((string) (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[i])["socket"]) == socketId){
                // (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[i])["readyStatus"]) = ready;
                // Debug.Log("playerinfo.readyStatus : " + (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[i])["readyStatus"]).ToString());
                // var jsonStr = Json.Serialize(playerinfo);

                string jsonStr = "{\"playerNum\" : " + i + ", \"readyStatus\" : \"" + ready + "\", \"teamStatus\" : \"" + teamChange + "\"}";

                socketManager.Socket.Emit("changeStatus", jsonStr);

                break;
            }
        }

    }

    public void OnTeamChangeEnter()
    {
        Debug.Log("Click TeamChange Button");

        if (teamChange == false) {
            ready = false;
            teamChange = true;
            // gameObject.GetComponent<Image>().color = new Color32(250, 216, 55, 255);

        } else {
            ready = false;
            teamChange = false;
            // gameObject.GetComponent<Image>().color = new Color32(255, 121, 121, 255);
        }
        var playerinfo = Json.Deserialize(playerInfoStr) as Dictionary<string, object>;
        Debug.Log("[teamchange click]\nready status : " + ready + "\nteamChange : " + teamChange);
        
        for (int i = 0; i < 8; i++){
            if ((string) (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[i])["socket"]) == socketId){
                // (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[i])["teamStatus"]) = teamChange;
                string jsonStr = "{\"playerNum\" : " + i + ", \"readyStatus\" : \"" + ready + "\", \"teamStatus\" : \"" + teamChange + "\"}";

                socketManager.Socket.Emit("changeStatus", jsonStr);

                break;
            }
        }
    }


    public void OnChangeColorEnter(){
        colorIndex = rand.Next(11);

        Debug.Log("Click Profile Change Array : " + colors.ToString());
        Debug.Log("Click Profile colorIndex : " + colorIndex.ToString());

        // if (colorIndex == 2 | colorIndex == 3 | colorIndex == 4 | colorIndex == 7 | colorIndex == 9 | colorIndex == 9){
        //     profileChangeColor.GetComponent<Image>().color = new Color32(77, 77, 77, 255);
        // } else {
        //     profileChangeColor.GetComponent<Image>().color = new Color32(212, 212, 212, 255);
        // }

        // playerProfile.GetComponent<Image>().color = colors[colorIndex];

        var playerinfo = Json.Deserialize(playerInfoStr) as Dictionary<string, object>;
        Debug.Log("Click profile change socket id  : " + (string) (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[0])["socket"]));
        
        
        for (int i = 0; i < 8; i++){
            if ((string) (((Dictionary<string, object>) ((List<object>) playerinfo["player"])[i])["socket"]) == socketId){
                Debug.Log("Click profile change player num  : " + i.ToString());
                
                string jsonStr = "{\"playerNum\" : " + i + ", \"statusCate\" : \"color\", \"value\" : " + colorIndex + "}";

                socketManager.Socket.Emit("changeColor", jsonStr);

                break;
            }
        }
    }

    public void StartGame(){
        SceneManager.LoadScene("MainMapScene");
    }
}