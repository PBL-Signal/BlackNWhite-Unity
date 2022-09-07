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

public class WaitingRoomNewSocket : MonoBehaviour
{
    // onWaitingRoomNewButtonClicks
    private bool ready = false;
    private bool teamChange = false;

    private Button BtnSwitchTeam;
    private Button BtnReady;

    private string playerInfoStr; // 추가 
    GameObject GameManager;
    GameObject SocketObjectManager;
    public GameObject roomPin;
    private SocketManager socketManager = null;

    public GameObject black_player1, black_player2, black_player3, black_player4, white_player1, white_player2, white_player3, white_player4;

    private Color32[] colors = new Color32[12] {new Color32(233, 19, 27, 255), new Color32(236, 126, 14, 255), 
                                                new Color32(248, 245, 88, 255), new Color32(255, 119, 242, 255), 
                                                new Color32(191, 222, 54, 255), new Color32(45, 172, 255, 255), 
                                                new Color32(34, 42, 121, 255), new Color32(45, 233, 208, 255), 
                                                new Color32(184, 139, 255, 255), new Color32(250, 222, 192, 255), 
                                                new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255)};

    private Color32[] status_color = new Color32[3] {new Color32(255, 121, 121, 255), new Color32(162, 233, 54, 255), new Color32(250, 216, 55, 255)};
    System.Random rand = new System.Random();
    private Color32 active_panel_color= new Color32(71, 124, 241, 71);
 
    public Dictionary<string, string> userDic = new Dictionary<string, string>();   // UI 몇번째에 어떤 user가 접속해있는지 저장 (obj번째, userID)
    private string clientUserID ;

    public Animator popupAni;
    public GameObject popupBox;
    public GameObject popupTxt;
    IEnumerator coroutine;

    // Queue<int> blackPlayerIdx = new Queue<int>(); // team 별 UI 컴포넌트 번째수 할당하기 위한 큐
    // Queue<int> whitePlayerIdx = new Queue<int>();
    
    // UI player 대응 컴포넌트 idx 할당
    // int PlaceUser(Boolean team){
    //     if(team){
    //         Debug.Log("PlaceUser blackPlayerIdx.Count" + blackPlayerIdx.Count);
    //         return blackPlayerIdx.Dequeue();
    //     }else{
    //         Debug.Log("PlaceUser whitePlayerIdx.Count" + whitePlayerIdx.Count);
    //         return whitePlayerIdx.Dequeue();
    //     }
    // }

    // UI player 대응 컴포넌트 idx 제거
    // void DeplaceUser(Boolean team, int idx){
    //     Debug.Log("DeplaceUser idx ! : " +idx + "team : " + team);
    //     if(team){
    //         blackPlayerIdx.Enqueue(idx);
    //          Debug.Log("$$DeplaceUser blackPlayerIdx.Count" + blackPlayerIdx.Count);
    //     }else{
    //         whitePlayerIdx.Enqueue(idx);
    //         Debug.Log("$$DeplaceUser whitePlayerIdx.Count" + whitePlayerIdx.Count);
    //     }
    // }

   string SearchPlayerObj(Boolean team, string place){
        if (team){
            return "white_player"+place;
        }else{
            return "black_player"+place;
        }
    }

    public void HomeClick(){    
        Debug.Log("HomeClick");
        socketManager.Socket.Emit("leaveRoom");
        // SceneManager.LoadScene("MainHome");
    }

    void Awake(){
        Debug.Log("AwakeAwake AWAKE Awake AWAKE  AWAKE ");
        ready = false;
        teamChange = false;
        userDic.Clear();
    }

    void Start()
    {
       
        Application.runInBackground = true;

        // // UI player 대등 컴포넌트 idx 할당
        // for (int i =1; i<5; i++){
        //     blackPlayerIdx.Enqueue(i);
        //     whitePlayerIdx.Enqueue(i);
        // }
        
        Debug.Log("WaitingRoomNewSocket Socket");

        GameManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        roomPin = GameObject.Find("RoomPin");
        BtnSwitchTeam =  GameObject.Find("BtnSwitchTeam").GetComponent<Button>();
        BtnReady =  GameObject.Find("BtnReady").GetComponent<Button>();
        socketManager.Socket.Emit("add user");


        socketManager.Socket.On("login", (string data) => Login(data));
        socketManager.Socket.On("updateUI", (string data) => UpdateUI(data));
        socketManager.Socket.On("user joined", (string data) => UserJoined(data));
        socketManager.Socket.On("updateTeamChange", (string data) => UpdateTeamChange(data));
        socketManager.Socket.On("onTeamChangeType2", () => onTeamChangeType2());
        socketManager.Socket.On("onGameStart", () => onGameStart());
        socketManager.Socket.On("loadMainGame", (string team) => loadMainGame(team));
        // socketManager.Socket.Once("onBlackGameStart", () => onBlackGameStart());
        // socketManager.Socket.Once("onWhiteGameStart", () => onWhiteGameStart());
        socketManager.Socket.On("userLeaved", (string data) => userLeaved(data));
        socketManager.Socket.On("logout", () => logout());
        socketManager.Socket.On("countGameStart", () => clickStartGame()); // 게임 스타트 버튼과 socketOn 둘 다 걸려있음
    }
    

    void LoadRoom(string data)
    {
        Debug.Log("LoadRoom!! data : " + data);
    }

    void UserJoined(string data)
    {
        Debug.Log("UserJoined!!");
        Debug.Log("UserJoined!! data : " + data);

        var user = Json.Deserialize(data) as Dictionary<string, object>;
        
        Debug.Log("user[nickname]: " + user["nickname"]);
        Debug.Log("user[userID]: " + user["userID"]);
        Debug.Log("user[color]: " + user["color"]);
        Debug.Log("user[status]: " + user["status"]);
        Debug.Log("user[place]: " + user["place"]);

        
        int statusIndex;
        string colorIndex;
        string nickname_obj;
        string status_obj;
        string color_obj;

        
        string nickname = (string)user["nickname"];
        int status = int.Parse(user["status"].ToString());
        string userID = user["userID"].ToString();
        string color = user["color"].ToString();
        Boolean team = (Boolean)user["team"];
        string place = user["place"].ToString();


        AllocatePlayer(nickname, status, userID, color, team, place);
    }

    // 다른 사용자가 나갔을 경우
    void userLeaved(string userID)
    {
        Debug.Log("[userLeaved]!! data : " + userID);
        Debug.Log("userLeaved-clientUserID : " + clientUserID);

        // // 현재 연결된 사용자인 경우 메인홈으로 이동
        // if (userID == clientUserID){
        //     Debug.Log("userLeaved!! 나 나감~!! : ");
        //     SceneManager.LoadScene("MainHome");
        // }
        // else{
        //     DeallocatePlayer(userID);    
        // }
        DeallocatePlayer(userID);   
    }


    // 현재 접속한 클라이언트가 나갔을 경우 << 수정 필요 >> emit하는 거 필요함 
    void logout()
    {
        // Debug.Log("userLeaved!! data : " + userID);
        Debug.Log("logout!! 나 나감~!! : "); 
        userDic.Clear();
        DestroyImmediate(this.gameObject);
        SceneManager.LoadScene("MainHome", LoadSceneMode.Single);
    }



    void UpdateUI(string data)
    {
        Debug.Log("UpdateUI!!");
        Debug.Log("UpdateUI!! data : " + data);
        var user = Json.Deserialize(data) as Dictionary<string, object>;
        // Debug.Log("user[nickname]: " + user["nickname"]);
        // Debug.Log("user[userID]: " + user["userID"]);
        // Debug.Log("user[color]: " + user["color"]);
        // Debug.Log("user[status]: " + user["status"]);
        // Debug.Log("user[place]: " + user["place"]);

        
        // string player_obj = SearchPlayerObj(user[team], user[place]);
        string player_num = userDic[user["userID"].ToString()];

        // 2. UI 오브젝트 status 변경 
        string status_obj =  player_num + "_status";
        int status = int.Parse(user["status"].ToString());
        GameObject.Find(status_obj).GetComponent<Image>().color = status_color[status_color_change(status)];
        Debug.Log("[player_num] : " + player_num + " player_obj : " + status_obj + "status : " + status);

        // 3. UI 오브젝트 profile 변경
        string color_obj =  player_num + "_profile";
        GameObject.Find(color_obj).GetComponent<Image>().color = colors[int.Parse(user["color"].ToString())]; 

    }

    // 유저가 WaitingRoom 첫 입장시 해당 룸의 정보(+플레이어 정보) 받아옴
    void Login(string data)
    {
        
        Debug.Log("Login!!");
        Debug.Log("Login!! data : " + data);
        Debug.Log("INITIALIZE INITIALIZE INITIALIZE");
        ready = false;
        teamChange = false;
        
        // 새 코드 v2
        var rooms = Json.Deserialize(data) as Dictionary<string, object>;
        Debug.Log("rooms.room : " + rooms["room"]);

        // Room Pin 화면에 출력
        if (roomPin){
            roomPin.GetComponent<TextMeshProUGUI>().text = rooms["room"].ToString();
        }
       
        // maxPlayer수 만큼 UI 할당
        InitRoomUI(int.Parse(rooms["maxPlayer"].ToString()));

        clientUserID = rooms["clientUserID"].ToString();

        Dictionary<string, object> users =  (Dictionary<string, object>) rooms["users"];
        // Debug.Log("users : " + users.Keys);



        foreach(KeyValuePair<string, object> user in users){
            Debug.Log("user@Key : " + user.Key);
            // Debug.Log("user@Value.nickname : " + ((Dictionary<string, object>)user.Value)["nickname"]);
            // Debug.Log("user@Value.status : " + ((Dictionary<string, object>)user.Value)["status"]);

            string nickname = (string)((Dictionary<string, object>)user.Value)["nickname"];
            // Debug.Log("user@nickname : " + nickname);

            int status = int.Parse(((Dictionary<string, object>)user.Value)["status"].ToString());
            // Debug.Log("user@status : " + status);

            string userID = user.Key;
            // Debug.Log("user@socket_id : " + socket_id);

            string color = ((Dictionary<string, object>)user.Value)["color"].ToString();
            // Debug.Log("user@color : " + color);

            Boolean team = (Boolean)((Dictionary<string, object>)user.Value)["team"];

            string place = ((Dictionary<string, object>)user.Value)["place"].ToString();

            // 새 버전
            AllocatePlayer(nickname, status, userID, color, team, place);
          
        }

        // foreach (string s in userDic.Keys){
        //     Debug.Log("userDic " + s + " " + userDic[s]);
        // }

        // // profile color onClick 연결
        Debug.Log("호출됨!!!!!!");
        // Debug.Log("check____ userDic " + userDic);
        // Debug.Log("check____ userDic.Keys " + userDic.Keys);
        // Debug.Log("check____ clientUserID " + clientUserID);
        string player_num = userDic[clientUserID];
        string my_color_obj =  player_num + "_color_change";

        GameObject.Find(player_num).transform.Find(my_color_obj).gameObject.SetActive(true);
        Button player_profile = GameObject.Find(my_color_obj).GetComponent<Button>();
        player_profile.onClick.AddListener(OnChangeColorEnter);


    }



    // login한 후 바로 maxPlayer만큼 UI 자리 할당함
    void InitRoomUI(int maxPlayer){
        Debug.Log("InitRoomUI!!!!!! maxPlayer : " + maxPlayer);

        for (int i =1; i <= maxPlayer/2; i++){

            // 0. panel 색 활성화 
            string black_panel_obj = "panel_black_player"+ i.ToString();
            string white_panel_obj = "panel_white_player"+ i.ToString();

            GameObject.Find(black_panel_obj+"_1").GetComponent<Image>().color = active_panel_color;
            GameObject.Find(black_panel_obj+"_2").GetComponent<Image>().color = active_panel_color;

            GameObject.Find(white_panel_obj+"_1").GetComponent<Image>().color = active_panel_color;
            GameObject.Find(white_panel_obj+"_2").GetComponent<Image>().color = active_panel_color;
        }
    }


    // status 별 색상 지정
    int status_color_change(int status){
        int index = 0;

        if (status == 0){
            index = 0;
        } else if(status == 1){
            index = 1;
        } else if(status == 2){
            index = 2;
        }

        Debug.Log("[status_color_change] color index : " + index);

        return index;
    }

    void AllocatePlayer(string nickname, int status, string userID, string color, Boolean team, string place){
            
            int statusIndex;
            string colorIndex;
            string nickname_obj;
            string status_obj;
            string color_obj;
            Debug.Log("[AllocatePlayer] 호출됨!!! userID : " + userID);

            if (! userDic.ContainsKey(userID)){

                Debug.Log("[AllocatePlayer] ! userDic.ContainsKey(userID)");
                // int playerIdx = PlaceUser(team);
                string player_obj = SearchPlayerObj(team,place);
                
                userDic.Add(userID, player_obj);

                // 0. player object 활성화
                if (team){
                    GameObject.Find("Team_White").transform.Find(player_obj).gameObject.SetActive(true);
                }else{
                    GameObject.Find("Team_Black").transform.Find(player_obj).gameObject.SetActive(true);
                }
              
                //  1. 닉네임 연결
                nickname_obj = player_obj + "_nickname";
                //  Debug.Log("nicknam_obj : " + nickname_obj );
                GameObject.Find(nickname_obj).GetComponent<TextMeshProUGUI>().text = nickname;

                // 2. status 연결
                status_obj =  player_obj + "_status";
                statusIndex = status_color_change(status);
                // Debug.Log("status_obj : " + status_obj );
                GameObject.Find(status_obj).GetComponent<Image>().color = status_color[statusIndex];

                // 3. profile 색 연결
                color_obj =  player_obj + "_profile";
                // Debug.Log("color_obj : " + color_obj );
                colorIndex = color;
                GameObject.Find(color_obj).GetComponent<Image>().color = colors[int.Parse(colorIndex)];

                Debug.Log("~!~!AllocatePlayer완료 nickname_obj : "+ nickname_obj);
            }else{
                Debug.Log("[AllocatePlayer] 중복 발생!");
            }

    }
  
    void DeallocatePlayer(string userID){
            if (userDic.ContainsKey(userID)){
                // 과정 2. 1번으로 얻은 socket.id로 해당 플레이어의 기존 위치를 unassign후 새 ui 위치 할당
                // 이전 위치 ui unsign 
                string player_num = userDic[userID.ToString()];
                int prevIdx = int.Parse(player_num.Substring(12, 1));  
                Debug.Log("DeallocatePlayer player_num : "+ player_num);

                //  1. 닉네임 연결 해제
                string nickname_obj = player_num + "_nickname";
                if(GameObject.Find(nickname_obj)!=null){
                    GameObject.Find(nickname_obj).GetComponent<TextMeshProUGUI>().text = "Player ";
                }

                // 2. status 연결 해제
                string status_obj =  player_num + "_status";
                int statusIndex = status_color_change(0);
                GameObject.Find(status_obj).GetComponent<Image>().color = status_color[statusIndex];

                // 3. profile 색 연결 해제
                string color_obj = player_num + "_profile";
                GameObject.Find(color_obj).GetComponent<Image>().color = new Color32(56, 56, 56, 255);

                Debug.Log("여기까지 잘됨/!! 이전 playernum "+ player_num);

                // 현재 연결된 사용자 정보가 바뀐경우 
                if (userID == clientUserID){
                    ready = false;
                    teamChange = false;
                    Debug.Log("상태 바꿈");
                    
                    // 4. color change 연결 해제        
                    string prevPlayer_obj =  player_num + "_color_change";
                    GameObject.Find(player_num).transform.Find(prevPlayer_obj).gameObject.SetActive(false); // false로v
                }

                // player object 비활성화
                if (player_num.Contains("white")){
                    GameObject.Find("Team_White").transform.Find(player_num).gameObject.SetActive(false);
                }else{
                    GameObject.Find("Team_Black").transform.Find(player_num).gameObject.SetActive(false);
                }
              

                // userDic에서 삭제
                userDic.Remove(userID);
                    foreach (string s in userDic.Keys){
                    Debug.Log("!!userDic " + s + " " + userDic[s]);
                }
            }else{
                Debug.Log("[DeallocatePlayer] 중복 없음!");
            }
    }
  
   // "프로필 색상 변경" 버튼 클릭시 동작 함수
    public void OnChangeColorEnter()
    {
        Debug.Log("OnChangeColorEnter");
        // 새버전 0627
        socketManager.Socket.Emit("changeProfileColor");
    }


    // 'TeamChange'의  type2 : 웨이팅 리스트에서 기다리다가 다른 사람이 나가 teamchange가 된 경우
    void onTeamChangeType2()
    {
        socketManager.Socket.Emit("updateSocketTeam");
        BtnReady.interactable  = true; // 레디 버튼 비활성화 해제
    }


    // 'TeamChange'의  type1 : 다른 팀에 자리가 있어서 바로 자리가 변경된 경우 
    void TeamChangeType1 (Dictionary<string, object> user){

        // 과정 1. 받은 사용자 정보 기반으로 socket.id와 team 정보 가져오기
        string nickname = (string)user["nickname"];
            Debug.Log("user@nickname : " + nickname);

        int status = int.Parse(user["status"].ToString());
            Debug.Log("user@status : " + status);

        string userID = user["userID"].ToString();
            Debug.Log("user@userID : " + userID);

        string color = user["color"].ToString();
            Debug.Log("user@color : " + color);

        Boolean team = (Boolean)user["team"];

        string place = user["place"].ToString();
        Debug.Log("user@place : " + place);

        // 과정 2. 1번으로 얻은 socket.id로 해당 플레이어의 기존 위치를 unassign후 새 ui 위치 할당
        DeallocatePlayer(userID);

        // 과정 3. 정보 ui에 띄우기
        AllocatePlayer(nickname, status, userID, color, team, place);

        // 현재 연결된 사용자 정보가 바뀐경우 
        if (userID == clientUserID){
            // profile color onClick 연결
            string player_num = userDic[clientUserID];
            string my_color_obj =  player_num + "_color_change";

            GameObject.Find(player_num).transform.Find(my_color_obj).gameObject.SetActive(true);
            Button player_profile = GameObject.Find(my_color_obj).GetComponent<Button>();
            player_profile.onClick.AddListener(OnChangeColorEnter);

            BtnReady.interactable  = true; // 레디 버튼 비활성화 해제
        }
    }



    // 'TeamChange' 에 대해 서버측에서 처리한 후 듣는 listener 
    public void UpdateTeamChange(string data)
    {
        Debug.Log("updateTeamChange!!");
        Debug.Log("updateTeamChange!! data : " + data);
       
        var teamChangeInfo = Json.Deserialize(data) as Dictionary<string, object>;
        
        Debug.Log("teamChangeInfo[player1] : " + teamChangeInfo["type"]);
       
      
        // 사용자 1명 위치 변경
        if (teamChangeInfo["type"].ToString() == "1"){
            Debug.Log("--------TYPE1---------");
            Debug.Log("teamChangeInfo[player1] : " + teamChangeInfo["player1"]);

            Dictionary<string, object> user=  (Dictionary<string, object>) teamChangeInfo["player1"];
            TeamChangeType1(user);
        }


        // 사용자 2명 위치 변경
        if (teamChangeInfo["type"].ToString() == "2"){
                Debug.Log("--------TYPE2---------");
                Debug.Log("teamChangeInfo[player1] : " + teamChangeInfo["player1"]);
                Debug.Log("teamChangeInfo[player2] : " + teamChangeInfo["player2"]);

                // 과정 1. 받은 사용자 정보 기반으로 socket.id와 team 정보 가져오기
                Dictionary<string, object> user1 =  (Dictionary<string, object>) teamChangeInfo["player1" ];
                Dictionary<string, object> user2 =  (Dictionary<string, object>) teamChangeInfo["player2" ];

                DeallocatePlayer(user1["userID"].ToString());
                DeallocatePlayer(user2["userID"].ToString());

                string nickname1 = (string)user1["nickname"];
                string nickname2 = (string)user2["nickname"];
                    // Debug.Log("user@nickname : " + nickname);

                int status1 = int.Parse(user1["status"].ToString());
                int status2 = int.Parse(user2["status"].ToString());
                    // Debug.Log("user@status : " + status);

                string user1ID = user1["userID"].ToString();
                string user2ID = user2["userID"].ToString();
                    // Debug.Log("user@userID : " + userID);

                string color1 = user1["color"].ToString();
                string color2 = user2["color"].ToString();
                    // Debug.Log("user@color : " + color);

                Boolean team1 = (Boolean)user1["team"];
                Boolean team2 = (Boolean)user2["team"];

                string place1 = user1["place"].ToString();
                string place2 = user2["place"].ToString();
                // Debug.Log("user@place : " + place);

                // // 과정 2. 1번으로 얻은 socket.id로 해당 플레이어의 기존 위치를 unassign후 새 ui 위치 할당
                DeallocatePlayer(user1ID);
                DeallocatePlayer(user2ID);

                // 과정 3. 정보 ui에 띄우기
                AllocatePlayer(nickname1, status1, user1ID, color1, team1, place1);
                AllocatePlayer(nickname2, status2, user2ID, color2, team2, place2);

                // 현재 연결된 사용자 정보가 바뀐경우 
                if (user1ID == clientUserID ||  user2ID == clientUserID){
                    // profile color onClick 연결
                    string player_num = userDic[clientUserID];
                    string my_color_obj =  player_num + "_color_change";

                    GameObject.Find(player_num).transform.Find(my_color_obj).gameObject.SetActive(true);
                    Button player_profile = GameObject.Find(my_color_obj).GetComponent<Button>();
                    player_profile.onClick.AddListener(OnChangeColorEnter);

                    BtnReady.interactable  = true; // 레디 버튼 비활성화 해제
                }
        }
    }

    

    // "Team Change" 버튼 클릭 시 동작 함수
     public void OnTeamChangeEnter()
    {
        Debug.Log("Click TeamChange Button");

        // 1. 변수값 변경
        int changeStatus;

        if (teamChange == false) {
            ready = false;
            teamChange = true;
            changeStatus = 2;   // 팀 바꾸기 on
            BtnReady.interactable  = false;
        } else {
            ready = false;
            teamChange = false;
            changeStatus = 0;  // 팀 바꾸기 off 후 비 ready
            BtnReady.interactable  = true;
        }


        // 2. UI 오브젝트 색 변경 
        string player_num = userDic[clientUserID];
        string player_obj =  player_num + "_status";
               
        GameObject.Find(player_obj).GetComponent<Image>().color = status_color[status_color_change(changeStatus)];
        Debug.Log("[player_num] : " + player_num + " player_obj : " + player_obj +"changeStatus : "+ changeStatus);

        
        // 3. 다른 USER에게도 변경사실 알리기위해 서버 전송
        socketManager.Socket.Emit("changeTeamStatus", changeStatus);
      
    }


    // "Ready" 버튼 클릭시 동작 함수
    public void OnReadyEnter()
    {
        Debug.Log("Click Ready Button");

        int changeStatus;

        // 1. 변수값 변경
        if (ready == false) { // 레디 활성화 됨 
            ready = true;
            teamChange = false;
            changeStatus = 1;
            BtnSwitchTeam.interactable  = false;
        } else { 
            ready = false;
            teamChange = false;
            changeStatus = 0;
            BtnSwitchTeam.interactable  = true;
        }
        Debug.Log("[ready click]\nready status : " + ready + "\nteamChange : " + teamChange + "changeStatus :" + changeStatus);

        // 2. UI 오브젝트 색 변경 
        string player_num = userDic[clientUserID];
        string player_obj =  player_num + "_status";
               
        GameObject.Find(player_obj).GetComponent<Image>().color = status_color[status_color_change(changeStatus)];
        Debug.Log("[player_num] : " + player_num + " player_obj : " + player_obj);
        
        
        // 3. 다른 USER에게도 변경사실 알리기
        socketManager.Socket.Emit("changeReadyStatus", changeStatus);

    }

    // 게임 시작 조건되었을때 호출되는 함수 (+강제 시작 버튼과도 연결됨)
    public void clickStartGame(){
        socketManager.Socket.Emit("Game Start");
    }


    public void onGameStart(){
        Debug.Log("[onGameStart] 서버로부터 On 성공");

        socketManager.Socket.Emit("joinTeam"); // 모든 클라이언트가 서버쪽에 1:1 요청함
        // string player_num = userDic[clientUserID];
        // Debug.Log("!!!----player_num " + player_num);
        // if(player_num.Contains("black"))
        // {
        //     Debug.Log("여기까지는 됨1 - Black으로");
        //     SceneManager.LoadScene("MainMapScene_Black");
        // } else {
        //     Debug.Log("여기까지는 됨2 - White으로");
        //     SceneManager.LoadScene("MainMapScene_White");
        // }
    }

    public void loadMainGame(string team){
        Debug.Log("[loadMainGame] team : " +  team);
        // 버튼 비활성화 
        BtnSwitchTeam.interactable  = false;
        BtnReady.interactable = false;
        
        string player_num = userDic[clientUserID];
        string my_color_obj =  player_num + "_color_change";

        Button player_profile = GameObject.Find(my_color_obj).GetComponent<Button>();
        BtnReady.interactable = false;


        // 카운트 다운 
        coroutine = CountDown(team);
        StartCoroutine(coroutine);  
 
    }

    IEnumerator CountDown(string team)
    {
        Debug.Log("CountDown CALLED");
        var time= 5.0f;
        var isPlaying = true;

        popupBox.SetActive(true);
        popupAni.SetBool("isOn", true);


        while(time>0 && isPlaying)
        {
            time -= Time.deltaTime;
            string second = (time%60).ToString("00");
            popupTxt.GetComponent<TextMeshProUGUI>().text = "카운트다운" + second + "초";
            Debug.Log("카운트다운" + second + "초");
            yield return null;
            

            if(time<=0)
            {
                Debug.Log("유니티 타이머 종료");
                if(team.Contains("false"))
                {
                    Debug.Log("여기까지는 됨1 - Black으로");
                    SceneManager.LoadScene("MainMapScene_Black", LoadSceneMode.Single);
                } else {
                    Debug.Log("여기까지는 됨2 - White으로");
                    SceneManager.LoadScene("MainMapScene_White", LoadSceneMode.Single);
                }
                yield break;
            }
        }
    }
}