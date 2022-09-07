using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using TMPro;
using MiniJSON;

public class ActionBarControl : MonoBehaviour
{
    GameObject GameManager;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;

    public GameObject actionBarDDOL;
    public GameObject actionBar;
    public GameObject LimitedTime;
    public GameObject AddedSettings;
    public GameObject GameLog;
    public GameObject teamTXT;
    public GameObject teamName;
    public GameObject logData;
    Transform scrollViewContent;
    ScrollRect scrollRect;

    private Transform playTime;
    public Transform Nickname;
    
    private bool teamValue;
    private string nickNameStr;

    IEnumerator coroutine;

    public GameObject pitaInfoObject;
    private GameObject pitaNumObject;


    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(actionBarDDOL);
        Debug.Log("Action BAR 실행");
    }

    void Start()
    {
        Application.runInBackground = true;
        
        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        socketManager.Socket.On("MainGameStart", (string roomData) => {
            Debug.Log("MainGameStart roomData json : " + roomData);
            var roomJson = Json.Deserialize(roomData) as Dictionary<string, object>;

            teamValue = (bool) roomJson["teamName"];
            Debug.Log("[Actionbar] teamValue : " + teamValue.ToString());

            AddGameLog(teamValue);
        });

        socketManager.Socket.Emit("checkSession");
        socketManager.Socket.On("sessionInfo", (string data) => sessionInfo(data));
        actionBarDDOL = GameObject.Find("DDOL");
        Nickname = actionBarDDOL.transform.Find("Canvas").transform.Find("UpperBar").transform.Find("UserInfo").transform.Find("UserName");

        scrollViewContent = actionBarDDOL.transform.Find("Canvas").transform.Find("GameLog").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content");
        scrollRect = actionBarDDOL.transform.Find("Canvas").transform.Find("GameLog").transform.Find("Scroll View").GetComponent<ScrollRect>();
        

        socketManager.Socket.On("Visible LimitedTime", (string team) => {
            Debug.Log("Visible LimitedTime - teamName " + team);

            teamTXT.GetComponent<TextMeshProUGUI>().text = "TEAM";
            
            if(team.Contains("false"))
            {
                Debug.Log("false - Black으로");
                teamName.GetComponent<TextMeshProUGUI>().text = "BLACK";
        
            } else {
                Debug.Log("true- White으로");
                teamName.GetComponent<TextMeshProUGUI>().text = "WHITE";
            }
            LimitedTime.SetActive(true);
            pitaInfoObject.SetActive(true);
            GameLog.SetActive(true);
        });

        socketManager.Socket.On("Visible AddedSettings", () => {
            Debug.Log("Visible AddedSettings");
            if(AddedSettings != null){
                AddedSettings.SetActive(true);
            }
            
        });

        socketManager.Socket.On("Timer START", () => {
            Debug.Log("Timer START CALLED");
            coroutine = myTimer();
            StartCoroutine(coroutine);  
        });

        socketManager.Socket.On("Timer END", () => {
            Debug.Log("Timer END CALLED");
            StopCoroutine(coroutine);
        });

        // socketManager.Socket.On("Result_PAGE", () => {
        //     Debug.Log("Result_PAGE CALLED");
        //     StopCoroutine(coroutine);
        //     LimitedTime.SetActive(false);
        //     GameLog.SetActive(false);
        // });

        socketManager.Socket.Emit("checkSession");
        socketManager.Socket.On("sessionInfo", (string data) => {
            var session = Json.Deserialize(data) as Dictionary<string, object>;
            Debug.Log("[SectionState] nickname: " + session["nickname"]);
            nickNameStr = session["nickname"].ToString();
        });

        socketManager.Socket.On("is_All_Sections_Destroyed_Nickname", (string nickname, string corpName) => {
            Debug.Log("[SectionState] is_All_Sections_Destroyed CALLED");
            if(nickname == nickNameStr){
                Debug.Log("[SectionState] nickname SAME & EMIT");
                socketManager.Socket.Emit("is_All_Sections_Destroyed", corpName);
            }
        });

        socketManager.Socket.On("Load_ResultPage", ()=>{
            Debug.Log("[Actionbar] Load_ResultPage CALLED");
            SceneManager.LoadScene("ResultPage");
            LimitedTime.SetActive(false);
            pitaInfoObject.SetActive(false);
            GameLog.SetActive(false);
        });


        pitaNumObject = pitaInfoObject.transform.Find("Pita Num").gameObject;
        socketManager.Socket.On("Load Pita Num", (string pitaNum) => {
            pitaNumObject.GetComponent<Text>().text = pitaNum;
        });
    }

    void Update()
    {
        socketManager.Socket.On("Update Pita", (string pitaNum) => UpdatePitaUI(pitaNum));
    }

    void UpdatePitaUI(string pitaNum){
        if(pitaNumObject){
            pitaNumObject.GetComponent<Text>().text = pitaNum;
        }
        
    }

    private void AddGameLog(bool teamValue){
        Debug.Log("[AddGameLog Called]");
        
        if(teamValue != null){
            Debug.Log("AddGameLog" + teamValue.ToString());
            if (scrollViewContent!= null){
                var itemList = scrollViewContent.GetComponentsInChildren<Transform>();
                Debug.Log("[itemList] " + itemList);

                Debug.Log("AddGameLog CALLED");
                socketManager.Socket.On("addLog",  (List<object> teamResults) => {
                    Debug.Log("[addLog] result >> "+ teamResults.ToString());
                    
                    foreach( Dictionary<string, object> teamResult in teamResults)
                    {
                        var logItem = Instantiate(logData, transform);
    
                        logItem.transform.Find("time").GetComponent<TextMeshProUGUI>().text = teamResult["time"].ToString();
                        logItem.transform.Find("user").GetComponent<TextMeshProUGUI>().text = teamResult["nickname"].ToString();
                        logItem.transform.Find("company").GetComponent<TextMeshProUGUI>().text = teamResult["targetCompany"].ToString();
                        logItem.transform.Find("section").GetComponent<TextMeshProUGUI>().text = teamResult["targetSection"].ToString();
                        logItem.transform.Find("detail").GetComponent<TextMeshProUGUI>().text = teamResult["detail"].ToString();
                        
                        logItem.transform.SetParent(scrollViewContent.transform);
                        // RectTransform rt = logItem.GetComponent<RectTransform>();
                        // rt.anchoredPosition = Vector2.zero;
                        // rt.localScale = Vector3.one;
                        // rt.sizeDelta = Vector2.zero;

                        scrollRect = actionBarDDOL.transform.Find("Canvas").transform.Find("GameLog").transform.Find("Scroll View").GetComponent<ScrollRect>();
                        scrollRect.verticalNormalizedPosition  = 0;
                    } 
                });

            }
        }
        scrollRect = actionBarDDOL.transform.Find("Canvas").transform.Find("GameLog").transform.Find("Scroll View").GetComponent<ScrollRect>();
        scrollRect.verticalNormalizedPosition  = 0;
    }

    void sessionInfo(string data){            
        var session = Json.Deserialize(data) as Dictionary<string, object>;
        Debug.Log("[SessionInfo] nickname: " + session["nickname"]);

        if (Nickname){
            Nickname.GetComponent<TextMeshProUGUI>().text = session["nickname"].ToString();
        }
    }

    IEnumerator myTimer()
    {
        Debug.Log("myTimer CALLED");
        var time= 600f;
        var isPlaying = true;

        actionBarDDOL = GameObject.Find("DDOL");
        playTime = actionBarDDOL.transform.Find("Canvas").transform.Find("UpperBar").transform.Find("LimitedTime").transform.Find("leftTime");
        
        while(time>0 && isPlaying)
        {
            time -= Time.deltaTime;
            string minute = Mathf.Floor(time/60).ToString("00");
            string second = (time%60).ToString("00");
            if(playTime){
                playTime.transform.GetComponent<TextMeshProUGUI>().text = minute + ":" + second;
                yield return null;
            }

            if(time<=0)
            {
                Debug.Log("유니티 타이머 종료");
            }
        }
    }
       
 
}
