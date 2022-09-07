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
using BestHTTP.JSON.LitJson;

public class ClickResponseBtn : MonoBehaviour
{
    GameObject GameManager, GameManager2;
    private SocketManager socketManager = null;

    private GameObject[] sectionList;
    private int[] cardList;
    Dictionary<string, object> cardInfo;
    // private int attackIndex;

    private Button btnResponse, btnPenetration, btnExplore, btnManagement, btnMonitoring;
    public GameObject Attack_Select_Section, ExpandedTab, PenestrationTab, ExploreTab, ManagementTab, MonitoringTab;
    private bool teamValue;
    private string companyName;
    private int attack_index;
    private int company_index;
    private int section_index;

    private GameObject obj_attack_name;
    private GameObject obj_attack_descript;
    private GameObject obj_attack_example;
    private GameObject obj_attack_example_list;
    private GameObject obj_response_button;

    GameObject[] sectionArea;
    GameObject[] attack_button;
    GameObject[] pita_price;
    GameObject[] level;
    GameObject[] cooldown;

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

    int[,] responseCoolTime = {{6, 5, 4, 3, 2}, {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2},
                                {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2},
                                {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2},
                                {6, 5, 4, 3, 2}, {6, 5, 4, 3, 2}};

    int[,] attackCoolTime = {{8, 7, 6, 5, 4}, {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4},
                                {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4},
                                {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4},
                                {8, 7, 6, 5, 4}, {8, 7, 6, 5, 4}};

    int[] attackStepPerNum = {4, 5, 6, 7, 11, 13, 13};
    int[] responseStepPerNum = {0, 4, 5, 6, 7, 11};

    int[] responseActive = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    int[] attackActive = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    string[][] sectionNames = new string[][] { new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Interal2", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec", "Area_Sec2"}};

    PopupUI _popup;

    void Awake(){
        Debug.Log("Awake - MainHome");
        _popup = FindObjectOfType<PopupUI>();
    }



    void Start(){
        Application.runInBackground = true;

        ExpandedTab.SetActive(false);

        GameManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        GameManager2 = GameObject.Find("ClickCompany");
        teamValue = GameManager2.GetComponent<OnClickCompany>().teamValue;
        companyName = GameManager2.GetComponent<OnClickCompany>().companyName;
        Debug.Log("ClickResearchBtn Start company : " + companyName);

        company_index = (int)Convert.ToChar(companyName.Substring(companyName.Length-1, 1)) - 65;

        if (teamValue == false){  // team black
            btnExplore = GameObject.Find("Explore").GetComponent<Button>();
            btnResponse = GameObject.Find("Attack").GetComponent<Button>();
            btnPenetration = GameObject.Find("Research").GetComponent<Button>();
        } else {  // team white
            btnManagement = GameObject.Find("ManageMent").GetComponent<Button>();
            btnMonitoring = GameObject.Find("Monitoring").GetComponent<Button>();
            btnResponse = GameObject.Find("Response").GetComponent<Button>();
            btnPenetration = GameObject.Find("Penetration Testing").GetComponent<Button>();
        }

        obj_attack_name = ExpandedTab.transform.Find("Attack Info Panel").transform.Find("Attack Name").gameObject;
        obj_attack_descript = ExpandedTab.transform.Find("Attack Info Panel").transform.Find("Attack Descript").gameObject;
        obj_attack_example = ExpandedTab.transform.Find("Attack Info Panel").transform.Find("Attack Example").gameObject;
        obj_attack_example_list = ExpandedTab.transform.Find("Attack Info Panel").transform.Find("Attack Example List").gameObject;
        obj_response_button = ExpandedTab.transform.Find("Attack Info Panel").transform.Find("Response Button").gameObject;
        
        socketManager.Socket.On("Short of Money", () => {
            Debug.Log("short of pita!!!");
            _popup.POP("Pita가 부족합니다.");
        });

        socketManager.Socket.On("Event Invalidity", (string invalidityCompany, int invaliditySectionIndex) => {
            if (teamValue){
                _popup.POP(invalidityCompany + "의 " + sectionNames[company_index][invaliditySectionIndex] + "에 대한\n대응이 무효되었습니다.");
            } else {
                _popup.POP(invalidityCompany + "의 " + sectionNames[company_index][invaliditySectionIndex] + "에 대한\n공격이 무효되었습니다.");
            }
            
        });
        
        socketManager.Socket.On("Section Activation List", (string companyNameCheck, bool[] activationList) => UpdateSectionActivation(companyNameCheck, activationList));
    }

    // 공격/대응 버튼 클릭
    public void ButtonExpandedClickled(){
       
        if (!(Attack_Select_Section.activeSelf || ExpandedTab.activeSelf)){
            btnResponse.GetComponent<Image>().color = Color.gray;
            btnPenetration.GetComponent<Image>().color = Color.white;
            // panel.GetComponent<RectTransform>().sizeDelta = new Vector2(onPanelWidth, onPanelHeight);
            ExpandedTab.SetActive(false);
            PenestrationTab.SetActive(false);
            Attack_Select_Section.SetActive(true);

            if (teamValue == false){  // team black
                btnExplore.GetComponent<Image>().color = Color.white;
                ExploreTab.SetActive(false);
                obj_attack_name.GetComponent<Text>().text = "공격 방법 선택";
                obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 White 팀을 공격하세요.\n\n공격을 통해 White 팀의 서비스를 마비시키세요.";

            } else {  // team white
                btnManagement.GetComponent<Image>().color = Color.white;
                btnMonitoring.GetComponent<Image>().color = Color.white;
                ManagementTab.SetActive(false);
                MonitoringTab.SetActive(false);
                obj_attack_name.GetComponent<Text>().text = "공격 대응 방법 선택";
                obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 Black 팀의 공격에 대응하세요.\n\n대응을 통해 공격을 무효화 시키세요.";
            }
            socketManager.Socket.Emit("Section Activation Check", companyName);
            Debug.Log("['Section Activation List'] : " + companyName);
        }   
        else{
            btnResponse.GetComponent<Image>().color = Color.white;
            // panel.GetComponent<RectTransform>().sizeDelta = new Vector2(offPanelWidth, offPanelHeight);
            ExpandedTab.SetActive(false);
            Attack_Select_Section.SetActive(false);
        }
        
    }

    public void OnClickSectionButton(){
        Debug.Log("OnClickSectionButton Object Name : " + EventSystem.current.currentSelectedGameObject.name);

        string click_section_obj = EventSystem.current.currentSelectedGameObject.name;
        click_section_obj = click_section_obj.Replace("Select_", "");

        section_index = Array.IndexOf(sectionNames[company_index], click_section_obj);

        ExpandedTab.SetActive(true);

        obj_attack_example.SetActive(false);
        obj_attack_example_list.SetActive(false);
        obj_response_button.SetActive(false);


        attack_button = GameObject.FindGameObjectsWithTag("Expanded Attack Button");
        pita_price = GameObject.FindGameObjectsWithTag("Expanded Pita Price");
        level = GameObject.FindGameObjectsWithTag("Expanded Level");
        cooldown = GameObject.FindGameObjectsWithTag("Cooldown");
        Debug.Log(attack_button[0] + ", company : " + companyName);
        Debug.Log(pita_price[0] + ", company : " + companyName);
        
        
        string jsonStr = "{\"companyName\" : \"" + companyName + "\"}";
        Debug.Log("OnClickUpgradeButton jsonStr : " + jsonStr);
        socketManager.Socket.Emit("Load Card List", jsonStr);
        socketManager.Socket.On("Card List", (string companyNameCheck, int[] cardLvList) => {
            Debug.Log("Card List cardLvList : " + cardLvList);
            Debug.Log("Card List teamValue : " + teamValue);

            if (companyNameCheck == companyName){
                cardList = cardLvList;
                
                string jsonStr = "{\"companyName\" : \"" + companyName + "\", \"sectionIndex\" : " + section_index + "}";
                Debug.Log("Card List cardInfo json : " + jsonStr);
                socketManager.Socket.Emit("Load Attack Step", jsonStr);
            }
        });

        if (teamValue == true){
            socketManager.Socket.On("Load Response List", (string companyNameCheck, int sectionCheck, int[] responseList, int step) => {
                if (companyNameCheck == companyName && sectionCheck == section_index){
                    Debug.Log("Card List responseStep : " + step.ToString());

                    Debug.Log("Card List : " + cardList[0] + ", company : " + companyName);

                    Debug.Log("Card Button Length : " + attack_button.Length + ", company : " + companyName);
                    Debug.Log("Pita Price Length : " + pita_price.Length + ", company : " + companyName);

                    for (int i = 0; i < responseList.Length; i++){
                        Debug.Log("responseList[" + i.ToString() + "] : " + responseList[i].ToString());
                    }

                    for (int i = 0; i < attack_button.Length; i++){
                        cooldown[i].GetComponent<Image>().fillAmount = 0;

                        pita_price[i].GetComponent<Text>().text = attack_pita[i, cardList[i] - 1].ToString() + " pita";
                        level[i].GetComponent<Text>().text = "Lv. " + cardList[i];

                        if (Array.IndexOf(responseList, i) > -1){
                            if (i >= 4 && i <= 7) {
                                attack_button[i].GetComponent<Image>().color = new Color32(255, 153, 153, 120);
                            } else {
                                attack_button[i].GetComponent<Image>().color = new Color32(255, 255, 255, 120);
                            }

                            Debug.Log("responseStepPerNum[step - 1] : " + responseStepPerNum[step - 1].ToString());
                            if ((step - 1) <= 0 && 0 <= i && i < 4){
                                responseActive[i] = 1;
                            } else if (i >= (responseStepPerNum[step - 1])){
                                responseActive[i] = 1;
                            } else {
                                responseActive[i] = 0;
                            }

                            Debug.Log("Convert.ToBoolean(responseActive[i]).ToString() : " + Convert.ToBoolean(responseActive[i]).ToString());

                            if (attack_index == i){
                                obj_response_button.GetComponent<Button>().interactable = Convert.ToBoolean(responseActive[i]);
                            }
                        } else {
                            Debug.Log("Convert.ToBoolean(responseActive[i]).ToString() : " + Convert.ToBoolean(responseActive[i]).ToString());
                            responseActive[i] = 0;
                            attack_button[i].GetComponent<Image>().color = new Color32(132, 132, 132, 120);
                        }
                    }
                    Debug.Log("responseActive list : " + responseActive.ToString());
                }
            });
        } else {
            socketManager.Socket.On("Attack Step", (string companyNameCheck, int sectionCheck, int step) => {
                if (companyNameCheck == companyName && sectionCheck == section_index){
                    Debug.Log("Card List attackStep : " + step.ToString());

                    Debug.Log("Card List : " + cardList[0] + ", company : " + companyName);

                    Debug.Log("Card Button Length : " + attack_button.Length + ", company : " + companyName);
                    Debug.Log("Pita Price Length : " + pita_price.Length + ", company : " + companyName);

                    
                    for (int i = 0; i < attackStepPerNum[step]; i++){
                        if (cooldown[i] != null && pita_price[i] != null && attack_button != null && attackActive[i] != null){
                            cooldown[i].GetComponent<Image>().fillAmount = 0;
                            if (cardList[i] == 0){
                                attackActive[i] = 0;
                                pita_price[i].GetComponent<Text>().text = "0 pita";
                                attack_button[i].GetComponent<Image>().color = new Color32(132, 132, 132, 120);
                                // attack_button[i].GetComponent<Button>().interactable = false;
                            } else {
                                attackActive[i] = 1;
                                pita_price[i].GetComponent<Text>().text = attack_pita[i, cardList[i] - 1].ToString() + " pita";

                                if (i >= 4 && i <= 7) {
                                    attack_button[i].GetComponent<Image>().color = new Color32(255, 153, 153, 120);
                                } else {
                                    attack_button[i].GetComponent<Image>().color = new Color32(255, 255, 255, 120);
                                }

                                if (attack_index == i){
                                    obj_response_button.GetComponent<Button>().interactable = Convert.ToBoolean(attackActive[i]);
                                }
                            }
                            
                            level[i].GetComponent<Text>().text = "Lv. " + cardList[i];
                        }
                    }

                    for(int i = attackStepPerNum[step]; i < attack_button.Length; i++){
                        if (cooldown[i] != null && pita_price[i] != null && attack_button != null && attackActive[i] != null){
                            cooldown[i].GetComponent<Image>().fillAmount = 0;
                            attackActive[i] = 0;
                            if (cardList[i] == 0){
                                pita_price[i].GetComponent<Text>().text = "0 pita";
                            } else {
                                pita_price[i].GetComponent<Text>().text = attack_pita[i, cardList[i] - 1].ToString() + " pita";
                            }
                            
                            attack_button[i].GetComponent<Image>().color = new Color32(132, 132, 132, 120);
                            level[i].GetComponent<Text>().text = "Lv. " + cardList[i];
                        }
                    }
                }
            });
        }
    }

    public void OnClickBackButton(){
        Debug.Log("OnClickBackButton");

        ExpandedTab.SetActive(false);
    }

    public void OnClickAttackButton() {
        Debug.Log("OnClickAttackButton Object Name : " + EventSystem.current.currentSelectedGameObject.name);

        string click_obj = EventSystem.current.currentSelectedGameObject.name;

        attack_index = Array.IndexOf(attack_name_list, click_obj);
        
        obj_attack_name.GetComponent<Text>().text = attack_name_list[attack_index];
        obj_attack_descript.GetComponent<Text>().text = attack_descript[attack_index];
        obj_attack_example.SetActive(true);
        obj_attack_example_list.SetActive(true);
        obj_response_button.SetActive(true);
        if (teamValue == true){
            Debug.Log("white - click btn : " + attack_index.ToString() + ", activeNum : " + responseActive[attack_index].ToString() + "activeBtn : " + Convert.ToBoolean(responseActive[attack_index]).ToString());
            obj_response_button.GetComponent<Button>().interactable = Convert.ToBoolean(responseActive[attack_index]);
        } else {
            obj_response_button.GetComponent<Button>().interactable = Convert.ToBoolean(attackActive[attack_index]);
        }
    }

    public void OnClickResponseButton(){
        if (teamValue == false){  // team black
            obj_attack_name.GetComponent<Text>().text = "공격 방법 선택";
            obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 White 팀을 공격하세요.\n\n공격을 통해 White 팀의 서비스를 마비시키세요.";
        } else {  // team white
            obj_attack_name.GetComponent<Text>().text = "공격 대응 방법 선택";
            obj_attack_descript.GetComponent<Text>().text = "pita를 사용하여 Black 팀의 공격에 대응하세요.\n\n대응을 통해 공격을 무효화 시키세요.";
        }

        obj_attack_example.SetActive(false);
        obj_attack_example_list.SetActive(false);
        obj_response_button.SetActive(false);
        
        string jsonStr = "{\"teamName\" : \"" + teamValue  + "\", \"attackIndex\" : " + attack_index + ", \"companyName\" : \"" + companyName + "\", \"sectionIndex\" : " + section_index + "}";
        Debug.Log("OnClickUpgradeButton jsonStr : " + jsonStr);

        if (cooldown[attack_index] != null){
            if (teamValue == true){
                socketManager.Socket.Emit("Click Response", jsonStr);

                socketManager.Socket.On("Continue Event", (float cooltime) => {
                    responseActive[attack_index] = 0;
                    StartCoroutine(ChargeCoolDown(cooldown[attack_index], cooltime, attack_index));
                    Debug.Log("cooldown[attack_index], responseCoolTime[attack_index, cardList[attack_index]])" + cooldown[attack_index].name + ", " + cooltime.ToString());
                });
            } else {
                socketManager.Socket.Emit("Click Attack", jsonStr);

                socketManager.Socket.On("Continue Event", (float cooltime) => {
                    attackActive[attack_index] = 0;
                    StartCoroutine(ChargeCoolDown(cooldown[attack_index], cooltime, attack_index));
                    Debug.Log("cooldown[attack_index], attackCoolTime[attack_index, cardList[attack_index]])" + cooldown[attack_index].name + ", " + cooltime.ToString());
                });
            }
        }
        
    }

    IEnumerator ChargeCoolDown(GameObject cooldownObject, float coolDown, int indexNum)
    {
        cooldownObject.GetComponent<Image>().fillAmount = 1;
        float currentCoolDown = coolDown;

        Debug.Log("CoolDown 코루틴 실행 (초) : " + coolDown.ToString());

        while(currentCoolDown > 0f){
            cooldownObject.GetComponent<Image>().fillAmount = (currentCoolDown / coolDown);
            currentCoolDown -= Time.deltaTime;
            
            yield return new WaitForFixedUpdate();
        }

        if (teamValue == true){
            responseActive[indexNum] = 1;
        } else {
            attackActive[indexNum] = 1;
        }

        if (attack_index == indexNum){
            obj_response_button.GetComponent<Button>().interactable = true;
        }

        cooldownObject.GetComponent<Image>().fillAmount = 0;
    }

    void UpdateSectionActivation(string companyNameCheck, bool[] activationList){
        Debug.Log("[UpdateSectionActivation] companyName : " + companyNameCheck);
        if (companyNameCheck == companyName){
            Debug.Log("[Section Activation List]" + activationList.ToString());

            sectionArea = GameObject.FindGameObjectsWithTag("Attack Section");

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((data) => { OnPointerExit((PointerEventData) data); });

            for (int i = 0; i < activationList.Length; i++){
                Debug.Log("activationList[" + i.ToString() + "] : " + activationList[i].ToString());
            }

            for(int i = 0; i < sectionArea.Length; i++){
                Debug.Log("[Section Activation List] section_name : " + sectionArea[i].name);

                if (activationList[i]){
                    Debug.Log("[Section Activation List] activationList[i] : " + activationList[i]);
                    GameObject selectAreaObj = sectionArea[i].transform.Find("Select_" + sectionArea[i].name).gameObject;
                    GameObject disableAreaObj = sectionArea[i].transform.Find("Disable_" + sectionArea[i].name).gameObject;
                    selectAreaObj.SetActive(false);
                    disableAreaObj.SetActive(false);
                    Debug.Log("[Section Activation List] section_name is true : " + sectionArea[i].name);
                    EventTrigger eventTrigger_Sections = sectionArea[i].gameObject.AddComponent<EventTrigger>();
                    eventTrigger_Sections.triggers.Add(pointerEnterEntry);
                    eventTrigger_Sections.triggers.Add(pointerExitEntry);
                } else {
                    GameObject selectAreaObj = sectionArea[i].transform.Find("Select_" + sectionArea[i].name).gameObject;
                    GameObject disableAreaObj = sectionArea[i].transform.Find("Disable_" + sectionArea[i].name).gameObject;
                    selectAreaObj.SetActive(false);
                    disableAreaObj.SetActive(true);
                }
            }
        }
    }

    private void stopPerforming(){
        Debug.Log("공격 혹은 대응으로 대응 혹은 공격을 중지함");

        cooldown[attack_index].GetComponent<Image>().fillAmount = 0;

        string jsonStr = "{\"companyName\" : \"" + companyName + "\"}";
        socketManager.Socket.Emit("Load Card List", jsonStr);
    }

    private void OnPointerEnter(PointerEventData data)
    {
        GameObject enterSection = data.pointerEnter;
        string enterSectionName = enterSection.name;
        Debug.Log("PointerEnter : " + enterSectionName);

        if (enterSectionName.Contains("Area_")){
            GameObject selectAreaObj = enterSection.transform.Find("Select_" + enterSectionName).gameObject;
            selectAreaObj.SetActive(true);

            
            GameObject selectAreaObjText = selectAreaObj.transform.Find("Select Text (TMP)").gameObject;
            selectAreaObjText.SetActive(true);
        }
    }

    private void OnPointerExit(PointerEventData data)
    {
        GameObject enterSection = data.pointerEnter;
        string enterSectionName = enterSection.name;
        Debug.Log("PointerExit : " + enterSectionName);
        GameObject selectAreaObj = enterSection.gameObject;
        if (selectAreaObj.name.Contains("Select_Area_")){
            selectAreaObj.SetActive(false);
        }
    }
}
