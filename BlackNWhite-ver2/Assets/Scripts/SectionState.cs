using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using MiniJSON;

public class SectionState : MonoBehaviour
{
    GameObject GameManager, GameManager2;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;
    private string companyName;    

    public GameObject mainTab;
    
    private Button[] Sections_Destroy_border = new Button[5]; // 영역 테두리 버튼(박스) 배열
    private Button[] Sections_btn = new Button[5]; // 영역 버튼(박스) 배열
    private Button[] Sections_bubble_btn = new Button[5]; // 영역 말풍선 버튼 배열
    private Button[] Sections_issue_btn = new Button[5]; // issueNum 버튼 배열
    
    private int[] companyCnt = {3,3,3,3,3};
    private string[][] sectionNames = new string[][] { new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}};
    string[] attack_name_list = {"Reconnaissance", "Credential Access", "Discovery", "Collection", "Resource Development", "Initial Access", "Execution", "Privilege Escalation", "Persistence", "Defense Evasion", "Command and Control", "Exfiltration", "Impact"};
    private string[] vulnArray = {"Reconnaissance", "Credential Access", "Discovery", "Collection"};
    public int companyIdx = -1;

    private bool teamValue;
    Coroutine[] mCoroutine;
    
    void Start()
    {
        GameManager2 = GameObject.Find("ClickCompany");
        companyName = GameManager2.GetComponent<OnClickCompany>().clickCompanyName;
        companyName = companyName.Replace("Company", "company");
        teamValue = GameManager2.GetComponent<OnClickCompany>().teamValue;

        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        companyIdx = (int)Convert.ToChar(companyName.Substring(companyName.Length-1,1)) - 65 ;

        mCoroutine = new Coroutine[companyCnt[companyIdx]];

        // #5
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener( (data) => { OnPointerEnter( (PointerEventData)data );});

        // #7
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener( (data) => { OnPointerExit( (PointerEventData)data );});

        

        if (teamValue == false){  // team black
            for(int i=0; i<companyCnt[companyIdx]; i++){
                string sectionName = sectionNames[companyIdx][i];
                Sections_btn[i] = mainTab.transform.Find(sectionName).GetComponent<Button>(); // #2
                Sections_Destroy_border[i] = mainTab.transform.Find(sectionName).transform.Find(sectionName + "_Destroy").GetComponent<Button>(); // #1
            }
            
        } else if (teamValue == true) {  // team white
            for(int i=0; i<companyCnt[companyIdx]; i++){
                string sectionName = sectionNames[companyIdx][i];

                Sections_btn[i] = mainTab.transform.Find(sectionName).GetComponent<Button>(); // #2
                EventTrigger eventTrigger_Sections = Sections_btn[i].gameObject.AddComponent<EventTrigger>(); // #4
                eventTrigger_Sections.triggers.Add(pointerEnterEntry); // #6
                eventTrigger_Sections.triggers.Add(pointerExitEntry); // #8
                                
                Sections_Destroy_border[i] = mainTab.transform.Find(sectionName).transform.Find(sectionName + "_Destroy").GetComponent<Button>(); // #1
                Sections_bubble_btn[i] = mainTab.transform.Find(sectionName).transform.Find(sectionName + "_Bubble").GetComponent<Button>(); // #3
                Sections_issue_btn[i] = mainTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").GetComponent<Button>();

                setIssueNum();
                setVuln();
            }
        } else {
            Debug.Log("TeamValue Error"); 
        }
        areaDestory();

        socketManager.Socket.On("addLog",  (List<object> teamResults) => {
            if(teamValue == true){
                setIssueNum();
            }
            areaDestory();
        });

        socketManager.Socket.On("updateSectionState", (string emitCorpName, int emitSectionIdx) => { // responseJson.companyName, responseJson.sectionIndex
            Debug.Log("updateSectionState CALLED"+ emitCorpName + emitSectionIdx.ToString());
            if(emitCorpName == companyName){
                areaRecovery(emitSectionIdx);    
            }            
        });
    }
    private void setVuln(){
        socketManager.Socket.Emit("PreDiscovery_Start", companyName);
        socketManager.Socket.On("PreDiscovery_Start_Emit", (string data) => {
            CropAreaVulnData corpvulndata = JsonUtility.FromJson<CropAreaVulnData>(data);
            int s_idx = corpvulndata.areaIdx;
            string area = sectionNames[companyIdx][s_idx];
            Text vulnTextObj = mainTab.transform.Find(area).transform.Find("Vuln_Text").GetComponent<Text>();
            // if(vulnTextObj != null){
            //     vulnTextObj.text = "취약점 : " + vulnArray[corpvulndata.vuln];
            //     vulnTextObj.color = new Color(255, 0, 0); 
            // }

            vulnTextObj.text = "취약점 : " + vulnArray[corpvulndata.vuln];
            vulnTextObj.color = new Color(255, 0, 0); 

            // mainTab.transform.Find(area).transform.Find(vulnText).GetComponent<Text>().text = "취약점 : " + vulnArray[corpvulndata.vuln];
            // mainTab.transform.Find(area).transform.Find(vulnText).GetComponent<Text>().color = new Color(255, 0, 0); 
        });
    }
    
    private void setIssueNum(){
        socketManager.Socket.Emit("Get_Issue_Count", companyName);
        socketManager.Socket.On("Issue_Count", (int[] cntArr) => {
            int[] IssueNums = new int[5];
            for(int i=0; i<cntArr.Length; i++){
                IssueNums[i] = cntArr[i];
            }

            Color zeroColor = new Color(79/255f, 79/255f, 79/255f);
            Color nonZeroColor = new Color(255/255f, 57/255f, 57/255f);

            for(int i=0; i<cntArr.Length; i++){
                string sectionName = sectionNames[companyIdx][i];
                if(IssueNums[i] == 0) {
                    mainTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").GetComponent<Image>().color = zeroColor;
                }else {
                    mainTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").GetComponent<Image>().color = nonZeroColor;
                }
                mainTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").transform.Find(sectionName + "_Issue_Text").GetComponent<Text>().text = IssueNums[i].ToString();
            }            
        });
    }

    void areaDestory()
    {
        socketManager.Socket.Emit("Get_Section_Destroy_State", companyName);
        socketManager.Socket.On("Section_Destroy_State", (string data) => {
            Debug.Log("COROUTINE CALLED 1");
            var sectionsData = Json.Deserialize(data) as Dictionary<string, object>;
            
            for(int i=0; i<companyCnt[companyIdx]; i++)
            {
                bool destroyStatus = (bool) (((Dictionary<string, object>) ((List<object>) sectionsData["sections"])[i])["destroyStatus"]); 
                if(destroyStatus)
                {
                    Debug.Log("COROUTINE START" + i.ToString());
                    mCoroutine[i] = StartCoroutine(flikcerBorder(Sections_btn[i], Sections_Destroy_border[i]));
                } 
            }     
        });
    }

    void areaRecovery(int emitSectionIdx){

        if(mCoroutine[emitSectionIdx] != null){
            Debug.Log("COROUTINE  IS NOT NULL" + emitSectionIdx.ToString());
            StopCoroutine(mCoroutine[emitSectionIdx]);
            Sections_btn[emitSectionIdx].GetComponent<Image>().color = Color.gray;
            Sections_Destroy_border[emitSectionIdx].gameObject.SetActive(false);
        }

        // socketManager.Socket.Emit("Get_Section_Destroy_State", companyName);
        // socketManager.Socket.On("Section_Destroy_State", (string data) => {
        //     Debug.Log("COROUTINE CALLED 2");
        //     var sectionsData = Json.Deserialize(data) as Dictionary<string, object>;

        //     for(int i=0; i<companyCnt[companyIdx]; i++){
        //         bool destroyStatus = (bool) (((Dictionary<string, object>) ((List<object>) sectionsData["sections"])[i])["destroyStatus"]); 
        //         Debug.Log("COROUTINE TEST " +  destroyStatus.ToString() + i.ToString());
        //         if (destroyStatus == false) {
        //             Debug.Log("else if STOP COROUTINE TRUE? " + i.ToString());
        //             Debug.Log("STOP COROUTINE" +  i.ToString());
        //             if(mCoroutine[i] != null){
        //                 Debug.Log("COROUTINE  IS NOT NULL" + i.ToString());
        //                 StopCoroutine(mCoroutine[i]);
        //                 Sections_btn[i].GetComponent<Image>().color = Color.gray;
        //                 Sections_Destroy_border[i].gameObject.SetActive(true);
        //             }
        //             // if(mCoroutine != null){
        //             //     Debug.Log("STOP COROUTINE");
        //             //     StopCoroutine(mCoroutine);
        //             //     Sections_Destroy_border[i].GetComponent<Image>().color = Color.blue;
        //             // }
        //         }
        //     }
        // });
    }


    IEnumerator flikcerBorder(Button border, Button block)
    {
        while(true)
        {
            border.GetComponent<Image>().color = Color.red;
            block.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            border.GetComponent<Image>().color = Color.gray;
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    void OnPointerEnter(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;

        if(enterObjName=="BubbleText"){
            enterObj = enterObj.transform.parent.parent.gameObject;
        }else if(enterObjName.Contains("_Level") || enterObjName.Contains("Text") || enterObjName.Contains("Destroy") || enterObjName.Contains("Issue")){
            enterObj = enterObj.transform.parent.gameObject;
        }

        if(enterObj.name.Contains("Area")){
            int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == enterObj.name);
            Button bubbleBtn = Sections_bubble_btn[s_idx];

            socketManager.Socket.Emit("Get_Section_Attacked_Name", companyName);
            socketManager.Socket.On("Section_Attacked_Name", (string data) => {
                var sectionsData = Json.Deserialize(data) as Dictionary<string, object>;
                
                string last_attack = ((Dictionary<string, object>)(((Dictionary<string, object>) ((List<object>) sectionsData["sections"])[s_idx])["response"]))["last"].ToString();

                if (last_attack == "-1"){
                    Sections_bubble_btn[s_idx].transform.Find("BubbleText").GetComponent<Text>().text = "진행 중인 공격이 없습니다."; 
                } else {
                    Sections_bubble_btn[s_idx].transform.Find("BubbleText").GetComponent<Text>().text = attack_name_list[int.Parse(last_attack)] + " 공격 진행 중...";  
                }
            });

            bubbleBtn.gameObject.SetActive(true);
        } 
    }

    void OnPointerExit(PointerEventData eventData)
    { 
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;

        if(enterObjName=="BubbleText"){
            enterObj = enterObj.transform.parent.parent.gameObject;
        }else if(enterObjName.Contains("_Level") || enterObjName.Contains("Text") || enterObjName.Contains("Destroy") || enterObjName.Contains("Issue")){
            enterObj = enterObj.transform.parent.gameObject;
        }

        if(enterObjName=="BubbleText"){
            enterObj = enterObj.transform.parent.parent.gameObject;
        }else if(enterObjName.Contains("_Level") || enterObjName.Contains("Text") || enterObjName.Contains("Destroy")){
            enterObj = enterObj.transform.parent.gameObject;
        }

        if(enterObj.name.Contains("Area")){
            int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == enterObj.name);
            Button bubbleBtn = Sections_bubble_btn[s_idx];
            bubbleBtn.gameObject.SetActive(false); 
        }
    }


    
}
