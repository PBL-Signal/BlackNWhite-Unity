using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using MiniJSON;
using TMPro;

[System.Serializable]
public class ResultData
{
    public int blackPita;
    public int whitePita;
    public int winHodu;
    public int loseHodu;
    public int tieHodu;
    public bool winTeam;
}

public class ResultManager : MonoBehaviour
{
    GameObject GameManager;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;

    private Text blackPitaText;
    private Text whitePitaText;
    private GameObject winBlack;
    private GameObject winWhite;

    private GameObject[] blackPlayers = new GameObject[4];
    private GameObject[] whitePlayers = new GameObject[4];

    public Button homeButton;

    private Color32[] colors = new Color32[12] {new Color32(233, 19, 27, 255), new Color32(236, 126, 14, 255), 
                                                new Color32(248, 245, 88, 255), new Color32(255, 119, 242, 255), 
                                                new Color32(191, 222, 54, 255), new Color32(45, 172, 255, 255), 
                                                new Color32(34, 42, 121, 255), new Color32(45, 233, 208, 255), 
                                                new Color32(184, 139, 255, 255), new Color32(250, 222, 192, 255), 
                                                new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255)};

    void Start()
    {
        Debug.Log("RESULT PAGE LOAD");
        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        blackPitaText = GameObject.Find("Team_Black").transform.Find("Black_Pita").GetComponent<Text>();
        whitePitaText = GameObject.Find("Team_White").transform.Find("White_Pita").GetComponent<Text>();

        homeButton.onClick.AddListener(onClickHomeBtn);

        socketManager.Socket.Emit("Finish_Load_ResultPage");

        // [승패 계산]
        // 1. 타임 아웃으로 인한 게임 종료
        socketManager.Socket.On("Timeout_Gameover", (bool winTeam, int blackScore, int whiteScore) => {
            Debug.Log("Timeout_Gameover CALLED");
            Debug.Log("Timeout_Gameover winTeam" + winTeam.ToString());
            
            Debug.Log("Timeout_Gameover blackScore" + blackScore.ToString());
            
            Debug.Log("Timeout_Gameover whiteScore" + whiteScore.ToString());

            // [결과 출력]
            socketManager.Socket.Emit("Get_Final_RoomTotal");
            // 피타&호두&승리팀 정보, 플레이어 정보
            socketManager.Socket.On("playerInfo", (List<object> blackUsersInfo, List<object> whiteUsersInfo, string data) => {
                ResultData resultdata = JsonUtility.FromJson<ResultData>(data);

                if(blackPitaText != null ){
                    blackPitaText.text = "SCORE : " + blackScore.ToString();
                }

                if(whitePitaText != null ){
                    whitePitaText.text = "SCORE : " + whiteScore.ToString();
                }

                switch(winTeam) {
                    case true:
                        winWhite = GameObject.Find("Team_White").transform.Find("WIN").gameObject;
                        winWhite.SetActive(true);
                        break;
                    case false:
                        winBlack = GameObject.Find("Team_Black").transform.Find("WIN").gameObject;
                        winBlack.SetActive(true);
                        break;
                }

                int i = 0; 
                foreach( Dictionary<string, object> blackuser in blackUsersInfo){
                    string objectStr = "black_player"+ (i+1).ToString();
                    blackPlayers[i] = GameObject.Find("Team_Black").transform.Find(objectStr).gameObject;
                    blackPlayers[i].SetActive(true);
                    blackPlayers[i].GetComponent<Image>().color = colors[(int)blackuser["UsersProfileColor"]];
                    blackPlayers[i].transform.Find("nickname").GetComponent<TextMeshProUGUI>().text = blackuser["nickname"].ToString();

                    switch (winTeam)
                    {
                        case true: // 화이트팀 승리
                            blackPlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.loseHodu.ToString();
                            break;
                        case false: // 블랙팀 승리
                            blackPlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.winHodu.ToString();
                            break;
                        default: 
                            blackPlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.tieHodu.ToString();
                            break;
                    }   
                    i++;                 
                }

                int j = 0; 
                foreach( Dictionary<string, object> whiteuser in whiteUsersInfo){
                    string objectStr = "white_player"+ (j+1).ToString();
                    whitePlayers[j] = GameObject.Find("Team_White").transform.Find(objectStr).gameObject;
                    whitePlayers[j].SetActive(true);
                    whitePlayers[j].GetComponent<Image>().color = colors[(int)whiteuser["UsersProfileColor"]];
                    whitePlayers[j].transform.Find("nickname").GetComponent<TextMeshProUGUI>().text = whiteuser["nickname"].ToString();
                    
                    switch (winTeam)
                    {
                        case true: // 화이트팀 승리
                            whitePlayers[j].transform.Find("hodu").GetComponent<Text>().text = resultdata.winHodu.ToString();
                            break;
                        case false: // 블랙팀 승리
                            whitePlayers[j].transform.Find("hodu").GetComponent<Text>().text = resultdata.loseHodu.ToString();
                            break;
                        default: 
                            whitePlayers[j].transform.Find("hodu").GetComponent<Text>().text = resultdata.tieHodu.ToString();
                            break;
                    }   
                    j++; 
                }              

            });  
        });


        // 2. 모든 회사 몰락으로 인한 게임 종료 -> 블랙팀 승리
        socketManager.Socket.On("Abandon_Gameover", (bool winTeam, int blackScore, int whiteScore) => {
            Debug.Log("Abandon_Gameover CALLED");

            // [결과 출력]
            socketManager.Socket.Emit("Get_Final_RoomTotal");
            // 피타&호두&승리팀 정보, 플레이어 정보
            socketManager.Socket.On("playerInfo", (List<object> blackUsersInfo, List<object> whiteUsersInfo, string data) => {
                ResultData resultdata = JsonUtility.FromJson<ResultData>(data);
                Debug.Log("blackPita : " + blackScore + "whitePita : " + whiteScore);

                if(blackPitaText != null ){
                    blackPitaText.text = "SCORE : " + blackScore.ToString();
                }

                if(whitePitaText != null ){
                    whitePitaText.text = "SCORE : " + whiteScore.ToString();
                }

                switch(winTeam) {
                    case true:
                        winWhite = GameObject.Find("Team_White").transform.Find("WIN").gameObject;
                        winWhite.SetActive(true);
                        break;
                    case false:
                        winBlack = GameObject.Find("Team_Black").transform.Find("WIN").gameObject;
                        winBlack.SetActive(true);
                        break;
                }

                int i = 0; 
                foreach( Dictionary<string, object> blackuser in blackUsersInfo){
                    string objectStr = "black_player"+ (i+1).ToString();
                    blackPlayers[i] = GameObject.Find("Team_Black").transform.Find(objectStr).gameObject;
                    blackPlayers[i].SetActive(true);
                    blackPlayers[i].GetComponent<Image>().color = colors[(int)blackuser["UsersProfileColor"]];
                    blackPlayers[i].transform.Find("nickname").GetComponent<TextMeshProUGUI>().text = blackuser["nickname"].ToString();   

                    blackPlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.winHodu.ToString();  
                    i++;              
                }

                int j = 0; 
                foreach( Dictionary<string, object> whiteuser in whiteUsersInfo){
                    string objectStr = "white_player"+ (j+1).ToString();
                    whitePlayers[j] = GameObject.Find("Team_White").transform.Find(objectStr).gameObject;
                    whitePlayers[j].SetActive(true);
                    whitePlayers[j].GetComponent<Image>().color = colors[(int)whiteuser["UsersProfileColor"]];
                    whitePlayers[j].transform.Find("nickname").GetComponent<TextMeshProUGUI>().text = whiteuser["nickname"].ToString();

                    whitePlayers[j].transform.Find("hodu").GetComponent<Text>().text = resultdata.loseHodu.ToString();
                    j++;
                }
            });  

        });
        







        // // [결과 출력]
        // socketManager.Socket.Emit("Get_Final_RoomTotal");
        // // 피타&호두&승리팀 정보, 플레이어 정보
        // socketManager.Socket.On("playerInfo", (List<object> blackUsersInfo, List<object> whiteUsersInfo, string data) => {
        //     ResultData resultdata = JsonUtility.FromJson<ResultData>(data);
        //     Debug.Log("blackPita : " + resultdata.blackPita + "whitePita : " + resultdata.whitePita);

        //     blackPitaText.text = resultdata.blackPita.ToString() + " pita";
        //     whitePitaText.text = resultdata.whitePita.ToString() + " pita";

        //     int i = 0; 
        //     foreach( Dictionary<string, object> blackuser in blackUsersInfo){
        //         string objectStr = "black_player"+ (i+1).ToString();
        //         blackPlayers[i] = GameObject.Find("Team_Black").transform.Find(objectStr).gameObject;
        //         blackPlayers[i].GetComponent<Image>().color = colors[(int)blackuser["UsersProfileColor"]];
        //         blackPlayers[i].transform.Find("nickname").GetComponent<TextMeshProUGUI>().text = blackuser["nickname"].ToString();

        //         if(resultdata.winTeam == false){
        //             blackPlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.winHodu.ToString();
        //         } else {
        //             blackPlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.loseHodu.ToString();
        //         }
                
        //     }

        //     int j = 0; 
        //     foreach( Dictionary<string, object> whiteuser in whiteUsersInfo){
        //         string objectStr = "black_player"+ (i+1).ToString();
        //         whitePlayers[j] = GameObject.Find("Team_White").transform.Find(objectStr).gameObject;
        //         whitePlayers[j].GetComponent<Image>().color = colors[(int)whiteuser["UsersProfileColor"]];
        //         whitePlayers[j].transform.Find("nickname").GetComponent<TextMeshProUGUI>().text = whiteuser["nickname"].ToString();

        //         if(resultdata.winTeam == true){
        //             whitePlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.winHodu.ToString();
        //         } else {
        //             whitePlayers[i].transform.Find("hodu").GetComponent<Text>().text = resultdata.loseHodu.ToString();
        //         }
        //     }

        // });  
    }

    void onClickHomeBtn() 
    {
        Debug.Log("HOME 버튼 클릭");
        SceneManager.LoadScene("mainHome");
    }
}