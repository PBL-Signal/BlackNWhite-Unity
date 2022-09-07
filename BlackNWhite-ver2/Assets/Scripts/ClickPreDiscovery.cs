using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using MiniJSON;
using TMPro;

[System.Serializable]
public class CropAreaData
{
    public string Corp;
    public int areaIdx;
}

public class CropAreaVulnData
{
    public string Corp;
    public int areaIdx;
    public int vuln;
    public bool vulnActive;
}


public class ClickPreDiscovery : MonoBehaviour
{
    GameObject GameManager, GameManager2;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;
    private string companyName;   

    private Button btnPreDiscovery;
    private Button btnFollowUp;
    private Button btnAttack;
    private Button btnResearch;

    private Button[] Sections_btn = new Button[5]; // 영역 버튼(박스) 배열

    public GameObject AttackTab, AttackSelectTab, PenestrationTab, ExploreTab;

    private int[] companyCnt = {3,3,3,3,3};
    private string[][] sectionNames = new string[][] { new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}};
    private string[] vulnArray = {"Reconnaissance", "Credential Access", "Discovery", "Collection"};
    public int companyIdx = -1;

    PopupUI _popup;

    void Awake(){
        Debug.Log("Awake - MainHome");
        _popup = FindObjectOfType<PopupUI>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager2 = GameObject.Find("ClickCompany");
        companyName = GameManager2.GetComponent<OnClickCompany>().clickCompanyName;
        companyName = companyName.Replace("Company", "company");

        ExploreTab.SetActive(false);

        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        companyIdx = (int)Convert.ToChar(companyName.Substring(companyName.Length-1,1)) - 65 ;

        
        // #5
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener( (data) => { OnPointerEnterD( (PointerEventData)data );});

        // #7
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener( (data) => { OnPointerExitD( (PointerEventData)data );});

        for(int i=0; i<companyCnt[companyIdx]; i++){
            Sections_btn[i] = ExploreTab.transform.Find(sectionNames[companyIdx][i]).GetComponent<Button>(); // #3
            EventTrigger eventTrigger_Sections = Sections_btn[i].gameObject.AddComponent<EventTrigger>(); // #4
            eventTrigger_Sections.triggers.Add(pointerEnterEntry); // #6
            eventTrigger_Sections.triggers.Add(pointerExitEntry); // #8

            Sections_btn[i].onClick.AddListener(onClickAreaPreDiscoveryBtn);
            
        }

        btnPreDiscovery = GameObject.Find("Explore").GetComponent<Button>();
        btnPreDiscovery.onClick.AddListener(OnClickBtnPreDiscovery);
        btnFollowUp = GameObject.Find("Follow-Up").GetComponent<Button>();
        btnAttack = GameObject.Find("Attack").GetComponent<Button>();
        btnResearch = GameObject.Find("Research").GetComponent<Button>();

        socketManager.Socket.On("Discovery Start", (string emitCorpName, int areaIdx) => {
            Debug.Log("start emitCorpName : " + emitCorpName);
            Debug.Log("start area idx : " + areaIdx);

            if(emitCorpName == companyName){
                setVulnDiscovery(areaIdx);  
            }       
        });

        socketManager.Socket.On("Area_VulnActive", (string emitCorpName, int areaIdx, bool vulnActive) => {
            Debug.Log("emitCorpName : " + emitCorpName);
            Debug.Log("area idx : " + areaIdx);
            Debug.Log("vuln active : " + vulnActive);

            if(emitCorpName == companyName){
                setVulnActive(areaIdx, vulnActive);  
            }       
        });

        

        socketManager.Socket.On("Short of Money", () => {
            _popup.POP("Pita가 부족합니다.");
        });

        socketManager.Socket.On("already done", () => {
            _popup.POP("이미 수행되었습니다.");
        });
    }

    public void OnPointerEnterD(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;
        //Debug.Log("Enter " + enterObjName);

        if(enterObjName.Contains("_Level") || enterObjName.Contains("Text")){
            enterObj = enterObj.transform.parent.gameObject;
        }

        if(enterObj.name.Contains("Area")){
            int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == enterObj.name);

            TextMeshProUGUI clickText = enterObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            clickText.gameObject.SetActive(true);

            Sections_btn[s_idx].transform.Find("Text").GetComponent<Text>().color = new Color(255, 10, 10);
            Sections_btn[s_idx].GetComponent<Image>().sprite = Resources.Load<Sprite>("64") as Sprite;

        } 
    }

    public void OnPointerExitD(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;
        //Debug.Log("Exit " + enterObj.name);

        if(enterObjName.Contains("_Level") || enterObjName.Contains("Text")){
            enterObj = enterObj.transform.parent.gameObject;
        }

        if(enterObj.name.Contains("Area")){
            int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == enterObj.name);

            TextMeshProUGUI clickText = enterObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            clickText.gameObject.SetActive(false);

            Sections_btn[s_idx].transform.Find("Text").GetComponent<Text>().color = new Color(0, 0, 0);
            Sections_btn[s_idx].GetComponent<Image>().sprite = Resources.Load<Sprite>("32 Light") as Sprite;   
        }   
    }

    public void OnClickBtnPreDiscovery(){

        // PreDiscovery Init(start)
        socketManager.Socket.Emit("PreDiscovery_Start", companyName);
        // ##  update 임시 방편
        socketManager.Socket.On("PreDiscovery_Start_Emit", (string data) => {
            CropAreaVulnData corpvulndata = JsonUtility.FromJson<CropAreaVulnData>(data);
            int s_idx = corpvulndata.areaIdx;
            setVulnText(s_idx, corpvulndata.vuln, corpvulndata.vulnActive);
        });

        if(!ExploreTab.activeSelf) {
            btnPreDiscovery.GetComponent<Image>().color = Color.gray;
            btnFollowUp.GetComponent<Image>().color = Color.white;
            btnResearch.GetComponent<Image>().color = Color.white;
            btnAttack.GetComponent<Image>().color = Color.white;

            ExploreTab.SetActive(true);
            AttackTab.SetActive(false);
            AttackSelectTab.SetActive(false);
            PenestrationTab.SetActive(false);

        } else {
            ExploreTab.SetActive(false);
            btnPreDiscovery.GetComponent<Image>().color = Color.white;
        }
    }

    public void onClickAreaPreDiscoveryBtn(){
        string areaName = EventSystem.current.currentSelectedGameObject.transform.name; // 클릭한 오브젝트 이름

        int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == areaName);

        CropAreaData mCorpAreaData = new CropAreaData();
        mCorpAreaData.Corp = companyName;
        mCorpAreaData.areaIdx = s_idx; // 영역 이름이 아닌 인덱스


        socketManager.Socket.Emit("Get_VulnActive", JsonUtility.ToJson(mCorpAreaData));
        socketManager.Socket.On("Area_VulnActive", (string emitCorpName, int areaIdx, bool vulnActive) => {
            if(emitCorpName == companyName){
                setVulnActive(areaIdx, vulnActive);  
            }       
        });
    }

    private void setVulnText(int areaIdx, int vuln, bool isActive){
        string area = sectionNames[companyIdx][areaIdx];
        string areaText = area + "_Level";
        string areaStartText = area + "_Level_Start";

        ExploreTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().text = "취약점 : " + vulnArray[vuln];
        ExploreTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().color = new Color(255, 0, 0); 
        ExploreTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().gameObject.SetActive(isActive);
    }

    private void setVulnActive(int areaIdx, bool vulnActive){
        string area = sectionNames[companyIdx][areaIdx];
        string areaText = area + "_Level";
        string areaStartText = area + "_Level_Start";
        
        ExploreTab.transform.Find(area).transform.Find(areaStartText).GetComponent<Text>().gameObject.SetActive(!vulnActive);
        ExploreTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().gameObject.SetActive(vulnActive);
    }

    private void setVulnDiscovery(int areaIdx){
        string area = sectionNames[companyIdx][areaIdx];
        string areaText = area + "_Level_Start";

        ExploreTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().text = "취약점 : 탐색 중...";
        ExploreTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().color = new Color(255, 0, 0); 
        ExploreTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().gameObject.SetActive(true);
    }
}
