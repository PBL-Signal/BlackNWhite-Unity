using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickGameLogEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject logBtn;
    [SerializeField]
    private GameObject chatBtn;

    [SerializeField]
    private GameObject logView;
    [SerializeField]
    private GameObject chatView;

    void Start()
    {
        Application.runInBackground = true;

        // 시작 시 log 활성화, chat은 비활성화 (버튼은 반대로 -> interactable)
        logBtn.GetComponent<Button>().interactable = false;
        logView.SetActive(true);
        chatBtn.SetActive(true);
        chatView.transform.SetSiblingIndex(2);

        chatBtn.GetComponent<Button>().interactable = true;

        Debug.Log("logView 순위 : " + logView.transform.GetSiblingIndex() + "chatView 순위 : " + chatView.transform.GetSiblingIndex());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && chatBtn.activeSelf)
        {
            ClickChatBtn();
        }
    }

    public void ClickLogBtn()
    {
        logBtn.GetComponent<Button>().interactable = false;

        chatBtn.GetComponent<Button>().interactable = true;
        chatView.transform.SetSiblingIndex(2);
        Debug.Log("logView 순위 : " + logView.transform.GetSiblingIndex() + "chatView 순위 : " + chatView.transform.GetSiblingIndex());
    }

    public void ClickChatBtn()
    {
        logBtn.GetComponent<Button>().interactable = true;
        logView.transform.SetSiblingIndex(2);

        chatBtn.GetComponent<Button>().interactable = false;
        Debug.Log("logView 순위 : " + logView.transform.GetSiblingIndex() + "chatView 순위 : " + chatView.transform.GetSiblingIndex());
    }
}
