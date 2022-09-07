using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using TMPro;
using System;
using MiniJSON;

public class OnClickCompany : MonoBehaviour
{
    GameObject GameManager;
    GameObject GameManager2;
    private SocketManager socketManager = null;

    GameObject[] companyButton = new GameObject[5];
    GameObject clickCompany;

    private string playerInfoStr;
    private string socketId;

    public bool teamValue;
    public string companyName;
    private Dictionary<string, object> teamProfiles;
    private string[] userID;
    // private int[,] perCompanyNum = {{0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}};
    // private List<List<int>> perCompanyNum = new List<List<int>>();
    private int[][] perCompanyNum = new int[5][];

    public string clickCompanyName;
    public string sceneName;

    GameObject ClickCompanyObject;
    
    GameObject profileObject;
    GameObject[] profileObjectTag;

    public Button resultButton;

    private Color32[] colors = new Color32[12] {new Color32(233, 19, 27, 255), new Color32(236, 126, 14, 255), 
                                                new Color32(248, 245, 88, 255), new Color32(255, 119, 242, 255), 
                                                new Color32(191, 222, 54, 255), new Color32(45, 172, 255, 255), 
                                                new Color32(34, 42, 121, 255), new Color32(45, 233, 208, 255), 
                                                new Color32(184, 139, 255, 255), new Color32(250, 222, 192, 255), 
                                                new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 255)};

    void Start()
    {
        resultButton.onClick.AddListener(EndGame);
        Application.runInBackground = true;

        GameManager = GameObject.Find("Socket");
        ClickCompanyObject = GameObject.Find("ClickCompany");

        socketManager = GameManager.GetComponent<SocketInit>().socketManager;
        socketId = GameManager.GetComponent<SocketInit>().socketId;
        
        for (int i = 0; i < 5; i++){
            string company = "Company" + Convert.ToChar(65 + i);
            companyButton[i] = GameObject.Find(company);
        }

        socketManager.Socket.Emit("InitGame");
        socketManager.Socket.On("MainGameStart", (string roomData) => {
            Debug.Log("MainGameStart roomData json : " + roomData);
            var roomJson = Json.Deserialize(roomData) as Dictionary<string, object>;

            teamValue = (bool) roomJson["teamName"];
            Debug.Log("teamValue : " + teamValue);

            int teamNum = Int32.Parse((string) roomJson["teamNum"].ToString());

            List<string> userIDList = new List<string>();
            for (int i = 0; i < teamNum; i++){
                string id = (string) ((List<object>) roomJson["userID"])[i];
                userIDList.Add(id);
                Debug.Log("teamProfiles json : " + id);
            }

            userID = userIDList.ToArray();

            teamProfiles = (Dictionary<string, object>) roomJson["teamProfileColor"];
        });

        // socketManager.Socket.Emit("On Main Map");
        socketManager.Socket.On("Company Status", (Boolean[] CompanyStatus) => {
            for (int i = 0; i < 5; i++){
                Debug.Log("[Company Status] Company Name : " + companyButton[i].name);
                Debug.Log("[Company Status] CompanyStatus[i] : " + CompanyStatus[i]);

                if (CompanyStatus[i] == true){
                    companyButton[i].GetComponent<Image>().color = Color.red;
                    companyButton[i].GetComponent<Button>().interactable = false;
                } else {
                    companyButton[i].GetComponent<Image>().color = Color.white;
                    companyButton[i].GetComponent<Button>().interactable = true;
                }
            }
        });

        string location = "";
        int locationNum;
        int personNum;
        // 현재는 percomapnynum을 모두 지우고 다시 생성하고의 방식임
        // 비효율적이므로 더 좋은 방법 찾아보기
        socketManager.Socket.On("Load User Location", (string locationData) => {
            Debug.Log("locationData : " + locationData);

            for (int i = 0; i < 5; i++){
                perCompanyNum[i] = new int[] {0, 0, 0, 0};
            }

            profileObjectTag = GameObject.FindGameObjectsWithTag("ProfileImage");
            if (profileObjectTag.Length > 0){
                for (int i = 0; i < profileObjectTag.Length; i++){
                    profileObjectTag[i].SetActive(false);
                }
            }

            var locationDataJson = Json.Deserialize(locationData) as Dictionary<string, object>;

            foreach(var userId in userID){
                Debug.Log("uderId : " + userId);
                location = locationDataJson[userId].ToString();
                Debug.Log("location : " + location);

                if (location != ""){
                    locationNum = (int)Convert.ToChar(location.Substring(location.Length-1, 1)) - 65;
                    Debug.Log("loaction Num : " + locationNum.ToString());
                    personNum = Array.IndexOf(perCompanyNum[locationNum], 0);
                    perCompanyNum[locationNum][personNum] += 1;
                    Debug.Log("personNum : " + personNum.ToString() + ", perCompanyNum : " + perCompanyNum[locationNum][personNum].ToString());

                    string objectName = "Person" + (personNum + 1);
                    Debug.Log("objectName : " + objectName);

                    profileObject = GameObject.Find(location).transform.Find("Location").transform.Find(objectName).gameObject;
                    Debug.Log("GameObject.Find(location).transform.Find('Location') : " + GameObject.Find(location).transform.Find("Location").gameObject.name);
                    Debug.Log("profileObject.name : " + profileObject.name);
                    profileObject.SetActive(true);
                    Debug.Log("Int32.Parse(teamProfiles[userId].ToString()) : " + teamProfiles[userId].ToString());
                    profileObject.GetComponent<Image>().color = colors[Int32.Parse(teamProfiles[userId].ToString())];
                } else {
                    Debug.Log("메인에 있음");
                }
            }
        });
    }

    public void SceneChange()
    {
        clickCompany = EventSystem.current.currentSelectedGameObject;
        Debug.Log("Click Compnay : " + clickCompany.ToString());
        clickCompanyName = clickCompany.transform.name;
        companyName = clickCompanyName.Replace("Company", "company");
        Debug.Log("teamValue in SceneChange : " + teamValue);

        socketManager.Socket.Emit("Select Company", clickCompanyName);

        Debug.Log("[OnClickCompany] Company NAME "+ clickCompany);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("DetailCompanyView"));
        if (teamValue == false){   // Black team menu로 이동
            sceneName = "Detail" + clickCompanyName + "View_Black";
            Debug.Log("sceneName : " + sceneName);
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        } else {   // White team menu로 이동
            sceneName = "Detail" + clickCompanyName + "View_White";
            Debug.Log("sceneName : " + sceneName);
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }

    public void CloseDetailScene(){
        socketManager.Socket.Emit("Back to MainMap");
    }

    public void EndGame()
    {
        socketManager.Socket.Emit("All_abandon_test");
        //SceneManager.LoadScene("ResultPage");
    }
}
