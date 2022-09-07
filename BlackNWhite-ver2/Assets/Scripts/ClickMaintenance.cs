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
public class CorpData
{
    public string Corp;
    public int areaIdx;
    public string level;
    public int vuln;
}

public class ClickMaintenance : MonoBehaviour
{
    public GameObject penTab;
    public GameObject resTab;
    public GameObject resSelTab;
    public GameObject monTab;
    public GameObject manTab;
    
    GameObject GameManager, GameManager2;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;

    private string companyName;

    private Button btnMaintenance;
    private Button btnResponse;
    private Button btnPentrationTesting;
    private Button btnMonitoring;

    private Button[] Sections_btn = new Button[5]; // 영역 버튼(박스) 배열

    private int[] companyCnt = {3,3,3,3,3};
    private string[][] sectionNames = new string[][] { new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}};
    public int companyIdx = -1;

    Dictionary<string, object> sectionInfo;

    PopupUI _popup;

    void Awake(){
        Debug.Log("Awake - MainHome");
        _popup = FindObjectOfType<PopupUI>();
    }

    void Start()
    {
        GameManager2 = GameObject.Find("ClickCompany");
        companyName = GameManager2.GetComponent<OnClickCompany>().clickCompanyName;
        companyName = companyName.Replace("Company", "company");

        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        companyIdx = (int)Convert.ToChar(companyName.Substring(companyName.Length-1,1)) - 65 ;

        // #5
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener( (data) => { OnPointerEnterM( (PointerEventData)data );});

        // #7
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener( (data) => { OnPointerExitM( (PointerEventData)data );});


        for(int i=0; i<companyCnt[companyIdx]; i++){
            Debug.Log("@@@@@ FIND 회사 IDX @@@@@ " + sectionNames[companyIdx][i]);

            Sections_btn[i] = manTab.transform.Find(sectionNames[companyIdx][i]).GetComponent<Button>(); // #3
            EventTrigger eventTrigger_Sections = Sections_btn[i].gameObject.AddComponent<EventTrigger>(); // #4
            eventTrigger_Sections.triggers.Add(pointerEnterEntry); // #6
            eventTrigger_Sections.triggers.Add(pointerExitEntry); // #8
            Sections_btn[i].onClick.AddListener(onClickArea);

            
        }

        btnMaintenance = GameObject.Find("ManageMent").GetComponent<Button>();
        btnMaintenance.onClick.AddListener(OnClickBtnMaintenance);
        btnResponse = GameObject.Find("Response").GetComponent<Button>();
        btnPentrationTesting = GameObject.Find("Penetration Testing").GetComponent<Button>();
        btnMonitoring = GameObject.Find("Monitoring").GetComponent<Button>();

        socketManager.Socket.On("New_Level", (string emitCorpName, string data) => {
            Debug.Log("emitCorpName : " + emitCorpName + "level : " + data);

            if(emitCorpName == companyName){
                Debug.Log("emitCorpName 동일");
                string[] dataSplit = data.Split('-');
                updateLevel(Int32.Parse(dataSplit[0]), dataSplit[1]); // sectionIdx, level      
            }
        });

        socketManager.Socket.On("Short of Money", () => {
            Debug.Log("short of pita!!!");
            _popup.POP("Pita가 부족합니다.");
        });

        socketManager.Socket.On("Out of Level", () => {
            Debug.Log("Out of Level!!!");
            _popup.POP("이미 최대 레벨입니다.");
        });
    }

    // ##
    void updateLevel(int areaIdx, string level){
        string area = sectionNames[companyIdx][areaIdx];
        string areaText = sectionNames[companyIdx][areaIdx] + "_Level";
        manTab.transform.Find(area).transform.Find(areaText).GetComponent<Text>().text = "Lv. " + level;
    }

    // ## 영역 클릭하면 선택한 영역 이름을 서버에 emit
    public void onClickArea(){
        string areaName = EventSystem.current.currentSelectedGameObject.transform.name; // 클릭한 오브젝트 이름
        Debug.Log("AREA CLICK! : " + areaName);

        int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == areaName);
        Debug.Log("######## AREA CLICK! IDX ######## : " + s_idx);

        CropAreaData mCorpAreaData = new CropAreaData();
        mCorpAreaData.Corp = companyName;
        mCorpAreaData.areaIdx = s_idx;

        socketManager.Socket.Emit("Section_Name", JsonUtility.ToJson(mCorpAreaData));

        socketManager.Socket.On("New_Level", (string emitCorpName, string data) => {
            Debug.Log("emitCorpName : " + emitCorpName + "level : " + data);

            if(emitCorpName == companyName){
                Debug.Log("emitCorpName 동일");
                string[] dataSplit = data.Split('-');
                updateLevel(Int32.Parse(dataSplit[0]), dataSplit[1]); // sectionIdx, level      
            }
        });
    }


    public void OnPointerEnterM(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;
        //Debug.Log("Enter " + enterObjName);

        if(enterObjName.Contains("_Level") || enterObjName.Contains("Text") || enterObjName.Contains("TMP")){
            enterObj = enterObj.transform.parent.gameObject;
        }

        if(enterObj.name.Contains("Area")){
            int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == enterObj.name);
            
            TextMeshProUGUI clickText = enterObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            clickText.gameObject.SetActive(true);
            
            Sections_btn[s_idx].transform.Find("Text").GetComponent<Text>().color = new Color(255, 10, 10);
            Sections_btn[s_idx].transform.Find(enterObj.name+"_Level").GetComponent<Text>().color = new Color(255, 10, 10);
            Sections_btn[s_idx].GetComponent<Image>().sprite = Resources.Load<Sprite>("64") as Sprite;
        } 
    }

    public void OnPointerExitM(PointerEventData eventData)
    { 
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;

        if(enterObjName.Contains("_Level") || enterObjName.Contains("Text") || enterObjName.Contains("TMP")){
            enterObj = enterObj.transform.parent.gameObject;
        }

        if(enterObj.name.Contains("Area")){
            int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == enterObj.name);
            //Button pitaBtn = Sections_pita_btn[s_idx];
            //pitaBtn.gameObject.SetActive(false); 

            TextMeshProUGUI clickText = enterObj.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            clickText.gameObject.SetActive(false);

            Sections_btn[s_idx].transform.Find("Text").GetComponent<Text>().color = new Color(0, 0, 0);
            Sections_btn[s_idx].transform.Find(enterObj.name+"_Level").GetComponent<Text>().color = new Color(0, 0, 0); 
            Sections_btn[s_idx].GetComponent<Image>().sprite = Resources.Load<Sprite>("32 Light") as Sprite; 
        }
    }

    public void OnClickBtnMaintenance(){
        penTab.SetActive(false);
        resTab.SetActive(false);
        resSelTab.SetActive(false);
        monTab.SetActive(false);

        // Maintannace Init(start)
        socketManager.Socket.Emit("Section_Start", companyName);
        // ##  update 임시 방편
        socketManager.Socket.On("Area_Start_Emit", (string data) => {
            CorpData corpdata = JsonUtility.FromJson<CorpData>(data);
            Debug.Log("[Maintenace] section-level>>>> " + corpdata.level); 
            int s_idx = corpdata.areaIdx;
            //int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == corpdata.area);
            updateLevel(s_idx, corpdata.level);
        });

        btnMonitoring.GetComponent<Image>().color = Color.white;
        btnResponse.GetComponent<Image>().color = Color.white;
        btnPentrationTesting.GetComponent<Image>().color = Color.white;

        if(!manTab.activeSelf) {
            btnMaintenance.GetComponent<Image>().color = Color.gray;
            manTab.SetActive(true);
        } else {
            btnMaintenance.GetComponent<Image>().color = Color.white;
            manTab.SetActive(false);
        }
    }
}
