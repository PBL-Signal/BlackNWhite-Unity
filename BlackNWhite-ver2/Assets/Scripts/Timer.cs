using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using MiniJSON;

public class Timer : MonoBehaviour
{
    GameObject GameManager;
    GameObject SocketObjectManager;
    private SocketManager socketManager = null;

    IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("Socket");
        SocketObjectManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        socketManager.Socket.On("Timer START", () => {
            Debug.Log("Timer START CALLED");
            coroutine = myTimer();
            //StartCoroutine(coroutine);  
        });

        socketManager.Socket.On("Timer END", () => {
            Debug.Log("Timer END CALLED");
            //StopCoroutine(coroutine);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator myTimer()
    {
        Debug.Log("myTimer CALLED");
        var time= 100f; // 1분 40초
        var isPlaying = true;
        
        while(time>0 && isPlaying)
        {
            time -= Time.deltaTime;
            string minute = Mathf.Floor(time/60).ToString("00");
            string second = (time%60).ToString("00");
            Debug.Log(minute + ":" + second);
            yield return null;

            if(time<=0)
            {
                Debug.Log("유니티 타이머 종료");
            }
        }
    }
}
