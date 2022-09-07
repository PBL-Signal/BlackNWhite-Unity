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

public class ClickMonitoring : MonoBehaviour
{
    GameObject GameManager, GameManager2;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;
    private string companyName;    

    public GameObject penTab;
    public GameObject resTab;
    public GameObject monTab;
    public GameObject manTab;
    public GameObject monitoringData; //프리팹
    Transform listParent; 
    
    private Button btnResponse;
    private Button btnPentrationTesting;
    private Button btnMaintenance;
    private Button btnMonitoring;

    private Button[] Sections_issue_btn = new Button[5]; // issueNum 버튼 배열
    private Button[] Sections_btn = new Button[5]; // 영역 버튼(박스) 배열

    private int[] companyCnt = {3,3,3,3,3};
    private string[][] sectionNames = new string[][] { new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[] {"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}, new string[]{"Area_DMZ", "Area_Interal", "Area_Sec"}};
    public int companyIdx = -1;
    string[] attack_name_list = {"Reconnaissance", "Credential Access", "Discovery", "Collection", "Resource Development", "Initial Access", "Execution", "Privilege Escalation", "Persistence", "Defense Evasion", "Command and Control", "Exfiltration", "Impact"};
    

    private Text Title;
    private Text noSelectWarn;
    private Vector3 textPos;
    // private Text Time;
    // private Text Detail;

    Text blockedNum;

    void Start()
    {
        GameManager2 = GameObject.Find("ClickCompany");
        companyName = GameManager2.GetComponent<OnClickCompany>().companyName;
        Debug.Log("@@@@@ 회사 명 @@@@@ " + companyName);

        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        btnMonitoring = GameObject.Find("Monitoring").GetComponent<Button>();
        btnMonitoring.onClick.AddListener(OnClickMonitoringBtn);
        btnResponse = GameObject.Find("Response").GetComponent<Button>();
        btnPentrationTesting = GameObject.Find("Penetration Testing").GetComponent<Button>();
        btnMaintenance = GameObject.Find("ManageMent").GetComponent<Button>();

        Debug.Log("On Monitoring companyName : " + companyName);
        blockedNum = monTab.transform.Find("Company_Blocked").transform.Find("CompanyBlockedNum").GetComponent<Text>();
        setBlockedNum();

        companyIdx = (int)Convert.ToChar(companyName.Substring(companyName.Length-1,1)) - 65 ;
        Debug.Log("@@@@@ 회사 IDX @@@@@ " + companyIdx);

        for(int i=0; i<companyCnt[companyIdx]; i++){
            Debug.Log("@@@@@ FIND 회사 IDX @@@@@ " + sectionNames[companyIdx][i]);
            string sectionName = sectionNames[companyIdx][i];
            // if(Sections_btn[i]){
            //     Sections_btn[i] = monTab.transform.Find(sectionName).GetComponent<Button>(); // #2
            //     Sections_btn[i].onClick.AddListener(onClickAreaMontitoring); // #3
            //     Sections_issue_btn[i] = monTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").GetComponent<Button>(); // #1
            // }
            Sections_btn[i] = monTab.transform.Find(sectionName).GetComponent<Button>(); // #2
            Sections_btn[i].onClick.AddListener(onClickAreaMontitoring); // #3
            Sections_issue_btn[i] = monTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").GetComponent<Button>(); // #1
            
        }

        Title = monTab.transform.Find("monResult").transform.Find("Title").GetComponent<Text>();
        noSelectWarn = monTab.transform.Find("monResult").transform.Find("noSelectWarn").GetComponent<Text>();
        //Time = monTab.transform.Find("monResult").transform.Find("Time").GetComponent<Text>();
        //Detail = monTab.transform.Find("monResult").transform.Find("Detail").GetComponent<Text>();
        
        listParent = monTab.transform.Find("monResult");
        textPos = monTab.transform.Find("monResult").position + new Vector3(110 -100, 60);

        Title.gameObject.SetActive(false);
        //Time.gameObject.SetActive(false);
        //Detail.gameObject.SetActive(false);

        
    }

    void Update()
    {
        
    }

    private void OnClickMonitoringBtn()
    {
        penTab.SetActive(false);
        resTab.SetActive(false);
        manTab.SetActive(false);

        btnResponse.GetComponent<Image>().color = Color.white;
        btnPentrationTesting.GetComponent<Image>().color = Color.white;
        btnMaintenance.GetComponent<Image>().color = Color.white;

        if(!monTab.activeSelf) {
            monTab.SetActive(true);
            btnMonitoring.GetComponent<Image>().color = Color.gray;

            for(int i=0; i<Sections_issue_btn.Length; i++) {
                if(Sections_issue_btn[i]) {
                    Sections_issue_btn[i].gameObject.SetActive(true);
                } else {
                    Debug.Log("No More game object");
                }
            }
            
            
            setIssueNum();
        } else {
            btnMonitoring.GetComponent<Image>().color = Color.white;
            monTab.SetActive(false);
        }
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
                    monTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").GetComponent<Image>().color = zeroColor;
                }else {
                    monTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").GetComponent<Image>().color = nonZeroColor;
                }
                monTab.transform.Find(sectionName).transform.Find(sectionName + "_Issue").transform.Find(sectionName + "_Issue_Text").GetComponent<Text>().text = IssueNums[i].ToString();
            }            
        });
    }

    private void onClickAreaMontitoring(){
        string areaName = EventSystem.current.currentSelectedGameObject.name; // 클릭한 오브젝트 이름
        Debug.Log("AREA CLICK! : " + areaName);
        Title.text = areaName.ToString();

        int s_idx = Array.FindIndex(sectionNames[companyIdx], x => x == areaName);
        socketManager.Socket.Emit("Get_Issue", companyName, s_idx);
        socketManager.Socket.On("Get_Issue_Detail", (int arrLen, int[] details) => {
            Debug.Log("AAA " + details[0].ToString());
            for(int i=0; i<arrLen; i++){
                var attackIdx = details[i];
                var monitoringItem = Instantiate(monitoringData, transform);
                monitoringItem.transform.position = textPos + new Vector3(0, -70.0f, 0);
                monitoringItem.transform.Find("AttackedTime").GetComponent<TextMeshProUGUI>().text = "11:11";
                monitoringItem.transform.Find("AttackDetail").GetComponent<TextMeshProUGUI>().text = attack_name_list[attackIdx] + "공격";
                monitoringItem.transform.SetParent(listParent.transform);
                textPos = monitoringItem.transform.position;
            }
                    
        });

        Title.gameObject.SetActive(true);
        //Time.gameObject.SetActive(true);
        //Detail.gameObject.SetActive(true);
        noSelectWarn.gameObject.SetActive(false);
    }

    private void setBlockedNum(){        
        Debug.Log("On Monitoring companyName : " + companyName);
        socketManager.Socket.Emit("On Monitoring", companyName);
        socketManager.Socket.On("Blocked Num", (int company_blockedNum) => {
            Debug.Log("company blocked num : " + company_blockedNum.ToString());

            blockedNum.text = company_blockedNum.ToString();

        });
    }

}