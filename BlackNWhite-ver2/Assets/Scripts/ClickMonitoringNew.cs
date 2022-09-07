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

public class ClickMonitoringNew : MonoBehaviour
{
    GameObject GameManager, GameManager2;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;
    private string companyName;    

    public GameObject penTab;
    public GameObject resTab;
    public GameObject monTab;
    public GameObject manTab;
    public GameObject ExpTab;
    public GameObject ExpSelSecTab;
    public GameObject monitoringData; //프리팹
    Transform scrollViewContent;
    
    private Button btnResponse;
    private Button btnPentrationTesting;
    private Button btnMaintenance;
    private Button btnMonitoring;

    private int[] companyCnt = {3,3,3,3,3};
    private string[][] sectionNames = new string[][] { new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}};
    private string[] typeNames = new string[] {"Detected", "Response", "Damage"};
    public int companyIdx = -1;
    string[] attack_name_list = {"Reconnaissance", "Credential Access", "Discovery", "Collection", "Resource Development", "Initial Access", "Execution", "Privilege Escalation", "Persistence", "Defense Evasion", "Command and Control", "Exfiltration", "Impact"};

    private Button Area_All_Filter_Btn;
    private Button[] Monitoring_Result_Filter_Btn = new Button[5]; // 영역 필터 버튼 배열
    private Button[] Monitoring_Result_Type_Filter_Btn = new Button[3]; // 타입 필터 버튼 배열

    private string currentClickArea = "Area_All";


    private TextMeshProUGUI blockedNum;

    void Start()
    {
        Debug.Log("###### CLICK MONITORING NEW ######");
        GameManager2 = GameObject.Find("ClickCompany");
        companyName = GameManager2.GetComponent<OnClickCompany>().companyName;

        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        blockedNum = monTab.transform.Find("Company_Blocked").transform.Find("CompanyBlockedNum").GetComponent<TextMeshProUGUI>();
        Debug.Log("On Monitoring companyName : " + companyName);
        setBlockedNum();

        btnMonitoring = GameObject.Find("Monitoring").GetComponent<Button>();
        btnMonitoring.onClick.AddListener(OnClickMonitoringBtn);
        btnResponse = GameObject.Find("Response").GetComponent<Button>();
        btnPentrationTesting = GameObject.Find("Penetration Testing").GetComponent<Button>();
        btnMaintenance = GameObject.Find("ManageMent").GetComponent<Button>();

        companyIdx = (int)Convert.ToChar(companyName.Substring(companyName.Length-1,1)) - 65 ;

        Area_All_Filter_Btn = monTab.transform.Find("ExpTab").transform.Find("ButtonGroup").transform.Find("Area_All_Filter_Button").GetComponent<Button>();
        Area_All_Filter_Btn.onClick.AddListener(OnClickSectionFilterBtn);   
        for(int i=0; i<companyCnt[companyIdx]; i++){
            Monitoring_Result_Filter_Btn[i] = monTab.transform.Find("ExpTab").transform.Find("ButtonGroup").transform.Find(sectionNames[companyIdx][i]+"_Filter_Button").GetComponent<Button>();
            Monitoring_Result_Filter_Btn[i].onClick.AddListener(OnClickSectionFilterBtn);   
        }

        for(int i=0; i<3; i++){
            Monitoring_Result_Type_Filter_Btn[i] = monTab.transform.Find("ExpTab").transform.Find("ButtonGroup").transform.Find(typeNames[i]+"_Filter_Button").GetComponent<Button>();
            Monitoring_Result_Type_Filter_Btn[i].onClick.AddListener(OnClickTypeFilterBtn);   
        }

        

        scrollViewContent = monTab.transform.Find("ExpTab").transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content");
        Debug.Log("[ExpTab] scrollViewContent " + scrollViewContent);

        ClickRefreshMonitoringLog();
        
        socketManager.Socket.On("addLog",  (List<object> logArr) => ClickRefreshMonitoringLog() );
    }

    private void ClickRefreshMonitoringLog(){
        Debug.Log("ClickRefreshMonitoringLog");
        //socketManager.Socket.Emit("Put_MonitoringLog", companyName);
        socketManager.Socket.Emit("Get_MonitoringLog", companyName);
        socketManager.Socket.On("MonitoringLog",  (List<object> MonResults) => loadMonResult(MonResults));
    }

    private void loadMonResult(List<object> MonResults, string filterName = "Area_All", string typeName = null){
        Debug.Log("[ExpTab socket] loadMonResult");

        // 1. 리스트 비워주기 
        // transform [] itemList = scrollViewContent.GetComponentInChildren<transform>();
        if (scrollViewContent!= null){
            var itemList = scrollViewContent.GetComponentsInChildren<Transform>();
            Debug.Log("[itemList] " + itemList);
            
            foreach (var item in itemList)
            {
                // 부모 object는 삭제하지 않음
                if(item !=  scrollViewContent.transform)
                {
                    Debug.Log("[ExpTab] Destroy gameObject(Item)");
                    Destroy(item.gameObject);
                }
            }
       

            if(filterName == "Area_All") {
                // 2. 데이터 불러와서 scrollView Item으로 넣어주기
                foreach( Dictionary<string, object> monResult in MonResults){
                    Debug.Log("MonitoringLog  monResult : " + monResult);

                    if(typeName == null) {
                        Debug.Log("Area_ALL & typeNULL");

                        var monitoringItem = Instantiate(monitoringData, transform);
                    
                        monitoringItem.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = monResult["time"].ToString();
                        monitoringItem.transform.Find("Section").GetComponent<TextMeshProUGUI>().text = monResult["targetSection"].ToString();
                        monitoringItem.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = monResult["actionType"].ToString();
                        monitoringItem.transform.Find("Detail").GetComponent<TextMeshProUGUI>().text = monResult["detail"].ToString();
                        
                        monitoringItem.transform.SetParent(scrollViewContent.transform, false);
                        // RectTransform rt = monitoringItem.GetComponent<RectTransform>();
                        // rt.anchoredPosition = Vector2.zero;
                        // rt.localScale = Vector3.one;
                        // rt.sizeDelta = Vector2.zero;    
                        
                    } else {
                        if(monResult["actionType"].ToString() == typeName){
                            Debug.Log("Area_ALL & type Not NULL");
                            Debug.Log("&&&&&& FLILTER ALL TYPE : " + typeName);
                            var monitoringItem = Instantiate(monitoringData, transform);
                    
                            monitoringItem.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = monResult["time"].ToString();
                            monitoringItem.transform.Find("Section").GetComponent<TextMeshProUGUI>().text = monResult["targetSection"].ToString();
                            monitoringItem.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = monResult["actionType"].ToString();
                            monitoringItem.transform.Find("Detail").GetComponent<TextMeshProUGUI>().text = monResult["detail"].ToString();
                            
                            monitoringItem.transform.SetParent(scrollViewContent.transform, false);
                            // RectTransform rt = monitoringItem.GetComponent<RectTransform>();
                            // rt.anchoredPosition = Vector2.zero;
                            // rt.localScale = Vector3.one;
                            // rt.sizeDelta = Vector2.zero;      
                        }
                    }             
                }  
            } else {
                // 2. 데이터 불러와서 scrollView Item으로 넣어주기
                foreach( Dictionary<string, object> monResult in MonResults){
                    Debug.Log("MonitoringLog  monResult : " + monResult);

                    if(typeName == null) {
                        Debug.Log("Area_OOO & typeNULL");
                        if(monResult["targetSection"].ToString() == filterName){
                            var monitoringItem = Instantiate(monitoringData, transform);
                        
                            monitoringItem.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = monResult["time"].ToString();
                            monitoringItem.transform.Find("Section").GetComponent<TextMeshProUGUI>().text = monResult["targetSection"].ToString();
                            monitoringItem.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = monResult["actionType"].ToString();
                            monitoringItem.transform.Find("Detail").GetComponent<TextMeshProUGUI>().text = monResult["detail"].ToString();
                            
                            monitoringItem.transform.SetParent(scrollViewContent.transform, false);
                            // RectTransform rt = monitoringItem.GetComponent<RectTransform>();
                            // rt.anchoredPosition = Vector2.zero;
                            // rt.localScale = Vector3.one;
                            // rt.sizeDelta = Vector2.zero;    
                        }
                    } else {
                        if(monResult["targetSection"].ToString() == filterName && monResult["actionType"].ToString() == typeName){
                            Debug.Log("Area_OOO & type Not NULL");
                            Debug.Log("%%%% FLILTER ALL TYPE : " + typeName);
                            var monitoringItem = Instantiate(monitoringData, transform);
                        
                            monitoringItem.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = monResult["time"].ToString();
                            monitoringItem.transform.Find("Section").GetComponent<TextMeshProUGUI>().text = monResult["targetSection"].ToString();
                            monitoringItem.transform.Find("Type").GetComponent<TextMeshProUGUI>().text = monResult["actionType"].ToString();
                            monitoringItem.transform.Find("Detail").GetComponent<TextMeshProUGUI>().text = monResult["detail"].ToString();
                            
                            monitoringItem.transform.SetParent(scrollViewContent.transform, false);
                            // RectTransform rt = monitoringItem.GetComponent<RectTransform>();
                            // rt.anchoredPosition = Vector2.zero;
                            // rt.localScale = Vector3.one;
                            // rt.sizeDelta = Vector2.zero;      
                        }
                    }                     
                }  

            }
        }
    }

    private void OnClickSectionFilterBtn()
    {
        Debug.Log("OnClickSectionFilterBtn CALLED" );
        string areaName = EventSystem.current.currentSelectedGameObject.name;
        string[] dataSplit = areaName.Split('_');
        areaName = "Area_" + dataSplit[1];
        currentClickArea = areaName;

        socketManager.Socket.Emit("Get_MonitoringLog", companyName);
        socketManager.Socket.On("MonitoringLog",  (List<object> MonResults) => loadMonResult(MonResults, areaName));
    }

    private void OnClickTypeFilterBtn()
    {
        
        string typeName = EventSystem.current.currentSelectedGameObject.name;
        string[] dataSplit = typeName.Split('_');
        typeName = dataSplit[0];
        Debug.Log("OnClickTypeFilterBtn CALLED >>> " + currentClickArea + typeName );

        socketManager.Socket.Emit("Get_MonitoringLog", companyName);
        socketManager.Socket.On("MonitoringLog",  (List<object> MonResults) => loadMonResult(MonResults, currentClickArea, typeName));
    }

    private void OnClickMonitoringBtn()
    {
        ClickRefreshMonitoringLog();
        ExpTab.SetActive(true);
        penTab.SetActive(false);
        resTab.SetActive(false);
        manTab.SetActive(false);
        ExpSelSecTab.SetActive(false);

        btnResponse.GetComponent<Image>().color = Color.white;
        btnPentrationTesting.GetComponent<Image>().color = Color.white;
        btnMaintenance.GetComponent<Image>().color = Color.white;

        if(!monTab.activeSelf) {
            monTab.SetActive(true);
            btnMonitoring.GetComponent<Image>().color = Color.gray;

        } else {
            btnMonitoring.GetComponent<Image>().color = Color.white;
            monTab.SetActive(false);
        }
    }


    private void setBlockedNum(){        
        Debug.Log("On Monitoring companyName : " + companyName);
        socketManager.Socket.Emit("On Monitoring", companyName);
        socketManager.Socket.On("Blocked Num", (string company_blockedNum) => {
            Debug.Log("company blocked num : " + company_blockedNum);
            if(blockedNum != null) {
                Debug.Log("company blocked name : " + blockedNum.name);
                
                blockedNum.text = company_blockedNum;
            }
        });
    }

}