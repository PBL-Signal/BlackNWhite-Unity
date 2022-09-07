using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseEvent : MonoBehaviour {

    public GameObject comDetailObject;

    // Start is called before the first frame update
    private void Start()
    {
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry_PointerEnter = new EventTrigger.Entry();
        entry_PointerEnter.eventID = EventTriggerType.PointerEnter;
        entry_PointerEnter.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerEnter);

        EventTrigger.Entry entry_PointerExit = new EventTrigger.Entry();
        entry_PointerExit.eventID = EventTriggerType.PointerExit;
        entry_PointerExit.callback.AddListener((data) => { OnPointerExit((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerExit);

        Transform comDetailTrans = transform.Find("Company_Detail");
        comDetailObject = comDetailTrans.gameObject;
    }

    private void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("PointerEnter");
        comDetailObject.SetActive(true);
    }

    private void OnPointerExit(PointerEventData data)
    {
        Debug.Log("PointerExit");
        comDetailObject.SetActive(false);
    }
}
