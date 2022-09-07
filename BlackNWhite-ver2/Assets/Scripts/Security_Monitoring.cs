using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Security_Monitoring : MonoBehaviour
{
    public Button analyzeButton;
    public Button maintenanceButton;
    public TextMeshProUGUI warningMessage;
    public ScrollRect analysisScrollView;
    public GameObject scrollViewContent;
    public GameObject analysisData;

    private Button[] areaButtons = new Button[3]; // 영역 버튼(박스) 배열
    private GameObject[] area = new GameObject[3]; 
    private String[] areaNames = {"DMZ Button", "Internal Button", "Security Button"};

    private TextMeshProUGUI title;
    private TextMeshProUGUI level;
    private bool analyzeBool = false;

    void Start()
    {
        title = GameObject.Find("Newtork Name Text").GetComponent<TextMeshProUGUI>();
        level = GameObject.Find("Newtork Level Text").GetComponent<TextMeshProUGUI>();


        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener( (data) => { OnPointerEnterArea( (PointerEventData)data );});

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener( (data) => { OnPointerExitArea( (PointerEventData)data );});
        
        int index = 0;
        foreach (string item in areaNames)
        {
            area[index] = GameObject.Find(item);
            areaButtons[index] = GameObject.Find(item).GetComponent<Button>();
            EventTrigger eventTrigger_Sections = areaButtons[index].gameObject.AddComponent<EventTrigger>();
            eventTrigger_Sections.triggers.Add(pointerEnterEntry);
            eventTrigger_Sections.triggers.Add(pointerExitEntry);
            areaButtons[index].onClick.AddListener(OnClickArea);
            index++;
        }  
    }

    // 영역 클릭 -> 레벨 보임
    public void OnClickArea()
    {
        string areaName = EventSystem.current.currentSelectedGameObject.transform.name; // 클릭한 오브젝트 이름

        string[] words = areaName.Split(' ');
        title.text = words[0] + " Level : ";
        level.text = "Lv. 1";
    }

    // 분석 버튼 클릭 -> Click 메시지 사라짐, 분석 결과 출력
    public void OnClickAnalyzeButton(){
        analyzeBool = true;

        warningMessage.gameObject.SetActive(!analyzeBool);

        LoadAnalysisResults();
        analysisScrollView.gameObject.SetActive(analyzeBool);
    }

    public void OnPointerEnterArea(PointerEventData eventData)
    {
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;

        //int index = Array.IndexOf(areaNames, enterObjName);
        int index = Array.FindIndex(areaNames, x => x == enterObjName);
        if(index > -1){
            Color color = area[index].GetComponent<Image>().color;
            color.a = 0.8f;
            area[index].GetComponent<Image>().color = color;
        }

    }

    public void OnPointerExitArea(PointerEventData eventData)
    { 
        GameObject enterObj = eventData.pointerEnter;
        string enterObjName = enterObj.name;

        int index = Array.FindIndex(areaNames, x => x == enterObjName);
        if(index > -1){
            Color color = area[index].GetComponent<Image>().color;
            color.a = 0f;
            area[index].GetComponent<Image>().color = color;
        }

    }

    void LoadAnalysisResults()
    {
        Debug.Log("LoadAnalysisResults");

        // 1. 리스트 비워주기 
        if (scrollViewContent!= null)
        {
            var itemList = scrollViewContent.GetComponentsInChildren<Transform>();
            Debug.Log("[itemList] " + itemList);
            
            foreach (var item in itemList)
            {
                // 부모 object는 삭제하지 않음
                if(item !=  scrollViewContent.transform)
                {
                    Debug.Log("Destroy gameObject(Item)");
                    Destroy(item.gameObject);
                }
            }
       
            
            // 2. 데이터 불러와서 scrollView Item으로 넣어주기
            //foreach( Dictionary<string, object> room in publicRooms)
            for(int i=0; i<30; i++)
            {

            
                var roomItem = Instantiate(analysisData, transform);
                roomItem.transform.Find("Time").GetComponent<TextMeshProUGUI>().text = "12:34:20";
                roomItem.transform.Find("Area").GetComponent<TextMeshProUGUI>().text = "Internal";
                roomItem.transform.Find("Detail").GetComponent<TextMeshProUGUI>().text = "Persistence Attack is detected.";
                roomItem.transform.SetParent(scrollViewContent.transform);
            }
        }
    }

}