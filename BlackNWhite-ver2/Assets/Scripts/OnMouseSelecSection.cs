using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnMouseSelecSection : MonoBehaviour {

    private GameObject selectObject;

    // Start is called before the first frame update
    private void Start()
    {
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        Debug.Log("eventTrigger name : " + eventTrigger.name);

        selectObject = eventTrigger.transform.Find("Select_" + eventTrigger.name).gameObject;
        Debug.Log("selectObject name : " + selectObject.name);


        EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
        entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_PointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerEnter);

        EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
        entry_PointerExit.eventID = EventTriggerType.PointerExit;
        entry_PointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerExit);
    }

    private void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("PointerEnter");
        selectObject.SetActive(true);
    }

    private void OnPointerExit(PointerEventData data)
    {
        Debug.Log("PointerExit");
        selectObject.SetActive(false);
    }
}
