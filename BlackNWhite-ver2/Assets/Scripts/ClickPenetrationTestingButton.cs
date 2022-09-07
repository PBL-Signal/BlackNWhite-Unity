using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using MiniJSON;
using TMPro;

public class ClickPenetrationTestingButton : MonoBehaviour
{
    GameObject GameManager, GameManager2;
    private SocketManager socketManager = null;
    private int[] cardList;
    Dictionary<string, object> cardInfo;
    // private int attackIndex;

    private Button btnResponse, btnPenetration, btnExplore, btnManagement, btnMonitoring;
    public GameObject Attack_Select_Section, ExpandedTab, PenestrationTab, ExploreTab, ManagementTab, MonitoringTab;
    private int attack_index;
    private bool teamValue;
    private string companyName;

    private GameObject obj_attack_name;
    private GameObject obj_attack_descript;
    private GameObject obj_attack_example;
    private GameObject obj_attack_example_list;
    private GameObject obj_upgrade_button;

    GameObject[] attack_button;
    GameObject[] pita_price;
    GameObject[] level;

    string[] attack_name_list = {"Reconnaissance", "Credential Access", "Discovery", "Collection", 
                                    "Resource Development", "Initial Access", "Execution", 
                                    "Privilege Escalation", "Persistence", "Defense Evasion", "Command and Control",
                                    "Exfiltration", "Impact"};

    int[,] attack_pita = {{1, 2, 3, 4, 5}, {1, 2, 3, 4, 5}, {1, 2, 3, 4, 5}, {1, 2, 3, 4, 5},
                            {3, 4, 5, 6, 7}, {3, 4, 5, 6, 7}, {2, 3, 4, 5, 6},
                            {4, 5, 6, 7, 8}, {2, 3, 4, 5, 6}, {2, 3, 4, 5, 6}, {2, 3, 4, 5, 6},
                            {5, 6, 7, 8, 9}, {5, 6, 7, 8, 9}};

    string[] attack_descript = {"공격 1 설명", "공격 2 설명", "공격 3 설명", "공격 4 설명",
                                "공격 5 설명", "공격 6 설명", "공격 7 설명",
                                "공격 8 설명", "공격 9 설명", "공격 10 설명", "공격 11 설명",
                                "공격 11 설명", "공격 12 설명"};

    int[,] coolTime = {{5, 4, 3, 2, 1}, {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1},
                            {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1},
                            {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1},
                            {5, 4, 3, 2, 1}, {5, 4, 3, 2, 1}};  

    // private float offPanelWidth = 0; // 파넬의 가로 길이
    // private float offPanelHeight = 620; // 파넬의 세로 길이
    // private float onPanelWidth = 1500 ; // 파넬의 가로 길이
    // private float onPanelHeight = 620; // 파넬의 세로 길이

    PopupUI _popup;

    void Awake(){
        Debug.Log("Awake - MainHome");
        _popup = FindObjectOfType<PopupUI>();
    }


    void Start(){
        Application.runInBackground = true;

        PenestrationTab.SetActive(false);

        GameManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        GameManager2 = GameObject.Find("ClickCompany");
        teamValue = GameManager2.GetComponent<OnClickCompany>().teamValue;
        companyName = GameManager2.GetComponent<OnClickCompany>().companyName;
        Debug.Log("ClickResearchBtn Start" + ", company : " + companyName);

        if (teamValue == false){  // team black
            btnExplore = GameObject.Find("Explore").GetComponent<Button>();
            btnResponse = GameObject.Find("Attack").GetComponent<Button>();
            btnPenetration = GameObject.Find("Research").GetComponent<Button>();
        } else {  // team white
            btnResponse = GameObject.Find("Response").GetComponent<Button>();
            btnPenetration = GameObject.Find("Penetration Testing").GetComponent<Button>();
            btnManagement = GameObject.Find("ManageMent").GetComponent<Button>();
            btnMonitoring = GameObject.Find("Monitoring").GetComponent<Button>();
        }


        obj_attack_name = PenestrationTab.transform.Find("Attack Info Panel").transform.Find("Attack Name").gameObject;
        obj_attack_descript = PenestrationTab.transform.Find("Attack Info Panel").transform.Find("Attack Descript").gameObject;
        obj_attack_example = PenestrationTab.transform.Find("Attack Info Panel").transform.Find("Attack Example").gameObject;
        obj_attack_example_list = PenestrationTab.transform.Find("Attack Info Panel").transform.Find("Attack Example List").gameObject;
        obj_upgrade_button = PenestrationTab.transform.Find("Attack Info Panel").transform.Find("Upgrade Button").gameObject;

        socketManager.Socket.On("Short of Money", () => {
            _popup.POP("Pita가 부족합니다.");
        });
    }

    void Update(){
        // socketManager.Open();


    }

    // 연구 버튼 클릭
    public void ButtonPenetrationClickled(){
       
        if (!PenestrationTab.activeSelf){
            btnPenetration.GetComponent<Image>().color = Color.gray;
            btnResponse.GetComponent<Image>().color = Color.white;
            // panel.GetComponent<RectTransform>().sizeDelta = new Vector2(onPanelWidth, onPanelHeight);
            Attack_Select_Section.SetActive(false);
            ExpandedTab.SetActive(false);
            PenestrationTab.SetActive(true);

            if (teamValue == false){   // team black
                btnExplore.GetComponent<Image>().color = Color.white;
                ExploreTab.SetActive(false);
                obj_attack_name.GetComponent<Text>().text = "연구 할 공격 유형 선택";
                obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 특정 유형의 공격에 대해 연구를 진행하세요.\n\n연구를 통해 해당 공격의 지식을 향상시켜 공격 시간을 단축시키세요.";
            } else {   // team white
                btnManagement.GetComponent<Image>().color = Color.white;
                btnMonitoring.GetComponent<Image>().color = Color.white;
                ManagementTab.SetActive(false);
                MonitoringTab.SetActive(false);
                obj_attack_name.GetComponent<Text>().text = "모의해킹 할 공격 유형 선택";
                obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 특정 유형의 공격에 대해 모의해킹을 진행하세요.\n\n모의해킹을 통해 해당 공격의 지식을 향상시켜 대응 시간을 단축시키세요.";
            }

            obj_attack_example.SetActive(false);
            obj_attack_example_list.SetActive(false);
            obj_upgrade_button.SetActive(false);

            attack_button = GameObject.FindGameObjectsWithTag("Penetration Attack Button");
            pita_price = GameObject.FindGameObjectsWithTag("Penetration Pita Price");
            level = GameObject.FindGameObjectsWithTag("Penetration Level");
            Debug.Log(attack_button[0] + ", company : " + companyName);
            Debug.Log(pita_price[0] + ", company : " + companyName);
            
            string jsonStr = "{\"companyName\" : \"" + companyName + "\"}";
            Debug.Log("OnClickUpgradeButton jsonStr : " + jsonStr);
            socketManager.Socket.Emit("Load Card List", jsonStr);
            socketManager.Socket.On("Card List", (string companyNameCheck, int[] cardLvList) => {
                if (companyNameCheck == companyName){
                    cardList = cardLvList;
                    Debug.Log("Card List : " + cardLvList + ", company : " + companyName);

                    Debug.Log("Card Button Length : " + attack_button.Length + ", company : " + companyName);
                    Debug.Log("Pita Price Length : " + pita_price.Length + ", company : " + companyName);

                    for(int i = 0; i < attack_button.Length; i++){
                        
                        // string levelNum = (((Dictionary<string, object>) ((List<object>) cardInfo["attackCard"])[i])["level"]).ToString();

                        // attack_button[i].GetComponent<Button>().interactable = (Boolean) (((Dictionary<string, object>) ((List<object>) cardInfo["attackCard"])[i])["activity"]);
                        // pita_price[i].GetComponent<Text>().text = (((Dictionary<string, object>) ((List<object>) cardInfo["attackCard"])[i])["pita"]).ToString() + " pita";

                        if (cardLvList[i] != null && pita_price[i] != null && attack_button[i] != null && level[i] != null){
                            if (cardLvList[i] == 0){
                                pita_price[i].GetComponent<Text>().text = attack_pita[i, cardLvList[i]].ToString() + " pita";
                                attack_button[i].GetComponent<Image>().color = new Color32(132, 132, 132, 120);
                                // attack_button[i].GetComponent<Button>().interactable = false;
                            } else {
                                if (cardLvList[i] == 5){
                                    pita_price[i].GetComponent<Text>().text = "0 pita";
                                    // Debug.Log("cardLvList[" + i.ToString() + "] : " + cardLvList[i].ToString());
                                    // obj_upgrade_button.GetComponent<Button>().interactable = false;
                                } else {
                                    pita_price[i].GetComponent<Text>().text = attack_pita[i, cardLvList[i]].ToString() + " pita";
                                }

                                if (i >= 4 && i <= 7) {
                                    attack_button[i].GetComponent<Image>().color = new Color32(255, 153, 153, 120);
                                } else {
                                    attack_button[i].GetComponent<Image>().color = new Color32(255, 255, 255, 120);
                                }
                            }
                            
                            level[i].GetComponent<Text>().text = "Lv. " + cardLvList[i];
                        }
                    }
                }
            });
        }   
        else{
            btnPenetration.GetComponent<Image>().color = Color.white;
            // panel.GetComponent<RectTransform>().sizeDelta = new Vector2(offPanelWidth, offPanelHeight);
            PenestrationTab.SetActive(false);
        }
        // btn.color = Random.ColorHSV();
        
    }

    public void OnClickAttackButton() {
        Debug.Log("OnClickAttackButton Object Name : " + EventSystem.current.currentSelectedGameObject.name);

        string click_obj = EventSystem.current.currentSelectedGameObject.name;

        attack_index = Array.IndexOf(attack_name_list, click_obj);
        
        obj_attack_name.GetComponent<Text>().text = attack_name_list[attack_index];
        obj_attack_descript.GetComponent<Text>().text = attack_descript[attack_index];
        obj_attack_example.SetActive(true);
        obj_attack_example_list.SetActive(true);
        obj_upgrade_button.SetActive(true);

        if (level[attack_index].GetComponent<Text>().text == "Lv. 5"){
            obj_upgrade_button.GetComponent<Button>().interactable = false;
        } else {
            obj_upgrade_button.GetComponent<Button>().interactable = true;
        }
    }


    public void OnClickUpgradeButton(){
        if (teamValue == false){   // team black
            obj_attack_name.GetComponent<Text>().text = "연구 할 공격 유형 선택";
            obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 특정 유형의 공격에 대해 연구를 진행하세요.\n\n연구를 통해 해당 공격의 지식을 향상시켜 공격 시간을 단축시키세요.";
        } else {   // team white
            obj_attack_name.GetComponent<Text>().text = "모의해킹 할 공격 유형 선택";
            obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 특정 유형의 공격에 대해 모의해킹을 진행하세요.\n\n모의해킹을 통해 해당 공격의 지식을 향상시켜 대응 시간을 단축시키세요.";
        }

        obj_attack_example.SetActive(false);
        obj_attack_example_list.SetActive(false);
        obj_upgrade_button.SetActive(false);

        string jsonStr = "{\"teamName\" : \"" + teamValue + "\", \"attackIndex\" : " + attack_index + ", \"companyName\" : \"" + companyName + "\"}";
        Debug.Log("OnClickUpgradeButton jsonStr : " + jsonStr);

        socketManager.Socket.Emit("Click Upgrade Attack", jsonStr);
    }
}
