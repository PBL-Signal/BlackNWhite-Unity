using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using TMPro;
using MiniJSON;


  
public class ClickNeutralizationBtn : MonoBehaviour
{
    GameObject GameManager, GameManager2;
    private SocketManager socketManager = null;
    public GameObject NeutralizationTab, BlockingCompany;  // Blocking Tab
    private Button btnNeutralization, btnExplore, btnResponse, btnPenetration;
    public GameObject ExpandedTab, PenestrationTab, ExploreTab;

    private string companyName;    
    private string companyKorName;    
    private bool teamValue;

    PopupUI _popup;

    public Animator popupAni;
    public GameObject popupBox;
    public GameObject popupTxt;
    IEnumerator coroutine;

    void Awake(){
        _popup = FindObjectOfType<PopupUI>();
    }
    
    void Start()
    {
        Debug.Log("[TEST] ClickNeutralizationBtn Socket");
        GameManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        btnNeutralization = GameObject.Find("Follow-Up").GetComponent<Button>(); 
        btnExplore = GameObject.Find("Explore").GetComponent<Button>();
        btnResponse = GameObject.Find("Attack").GetComponent<Button>();
        btnPenetration = GameObject.Find("Research").GetComponent<Button>();

        // 버튼 비활성화
        btnNeutralization.interactable = false;

        GameManager2 = GameObject.Find("ClickCompany");
        teamValue = GameManager2.GetComponent<OnClickCompany>().teamValue;
        companyName = GameManager2.GetComponent<OnClickCompany>().companyName;
        Debug.Log("[TEST] !@!@!@ ClickNeutralizationBtn " + ", company : " + companyName);

        //  무력화인지 check
        socketManager.Socket.Emit("Check Neutralization", companyName);
        socketManager.Socket.On("OnNeutralization", (string data) => OnNeutralization(data));


        // 한글 회사 이름 지정 
        var companyAlpha = companyName.Substring(7,1) ; 
        Debug.Log("[TEST] companyAlpha : "+ companyAlpha);
        switch (companyAlpha)
        {
            case "A" :
                    companyKorName = "회사 A";
                    break;
            case "B" :
                    companyKorName = "회사 B";
                    break;
            case "C" :
                    companyKorName = "회사 C";
                    break;
            case "D" :
                    companyKorName = "회사 D";
                    break;
            case "E" :
                    companyKorName = "회사 E";
                    break;
            default:
                companyKorName = "ERROR";
                break;
        }

 
    }



    // onClickTestNeutralizationBtn 버튼 - 특정 회사 IsBlocked로 수정해주는 함수 
    public void onClickTestNeutralizationBtn(){
        Debug.Log("[TEST] onClickTestNeutralizationBtn 스키마 수정");
        socketManager.Socket.Emit("TestNeutralization"); // 나중에 삭제해야 됨
    }

    // 무력화인지 확인하고 받아오는 
    void OnNeutralization(string data)
    {
        Debug.Log("[TEST] OnNeutralization - data : "+ data);
        if(data.Contains("True"))
        {
            Debug.Log("[TEST] 무력화되었습니다-------------^^");
            if (NeutralizationTab!= null){
                Debug.Log("[TEST] 무력화 NeutralizationTab-------------##");
                NeutralizationTab.SetActive(true); // 화면 블록킹
                BlockingCompany.GetComponent<TextMeshProUGUI>().text = companyKorName; // 회사 이름 지정 
                btnNeutralization.GetComponent<Image>().color = Color.yellow; // 버튼 색깔 바꿈

                // 나머지 탭들 다 닫기
                ExploreTab.SetActive(false);
                ExpandedTab.SetActive(false);
                PenestrationTab.SetActive(false);

                // 나머지 버튼 SetActive False
                btnExplore.GetComponent<Image>().color = Color.white;
                btnResponse.GetComponent<Image>().color = Color.white;
                btnPenetration.GetComponent<Image>().color = Color.white;

                btnExplore.interactable = false;
                btnResponse.interactable = false;
                btnPenetration.interactable = false;
                btnNeutralization.interactable = true;
            }
        }
    }
   

    // 사후관리 버튼 클릭 (무력화 해제 시도)
    public void onClickNeutralization(){
        Debug.Log("[TEST] onClickNeutralization!! - companyName : "+ companyName);

        socketManager.Socket.Emit("Try Non-neutralization", companyName);
    
        socketManager.Socket.On("After non-Neutralization",  (string data) => AfternonNeutralization(data));
    }


    // 무력화 해제 시도 후 상태 확인
    void AfternonNeutralization(string data){
        Debug.Log("[TEST] SolvedNeutralization!");

        if(data.Contains("True")){
                // 카운트다운
                coroutine = CountDown();
                StartCoroutine(coroutine);  
        }   
        else{
            _popup.POP("무력화 해제에 실패했습니다.");
        }
    }



    IEnumerator CountDown()
    {
        Debug.Log("CountDown CALLED");
        var time= 9.0f; // 9초 - 0초
        var isPlaying = true;

        popupBox.SetActive(true);
        popupAni.SetBool("isOn", true);
        btnNeutralization.interactable = false;

        while(time>0 && isPlaying)
        {
            time -= Time.deltaTime;
            string second = (time%60).ToString("00");
            string count = ( int.Parse(second)+1 ).ToString();
            popupTxt.GetComponent<TextMeshProUGUI>().text = "무력화 해제 중 (" + count + "초 남음)";
            Debug.Log("카운트다운" + second + "초");
            yield return null;
            

            if(time<=0)
            {
                popupBox.SetActive(false);
                Debug.Log("유니티 타이머 종료");
                if (NeutralizationTab!= null && NeutralizationTab.activeSelf){
                    NeutralizationTab.SetActive(false);
                    btnNeutralization.GetComponent<Image>().color = Color.white;


                    // 나머지 버튼 SetActive true
                    btnExplore.interactable = true;
                    btnResponse.interactable = true;
                    btnPenetration.interactable = true;
                    btnNeutralization.interactable = false;
                   
                }
                yield break;
            }
        }
    }
}
