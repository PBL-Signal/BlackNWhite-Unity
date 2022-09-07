using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using MiniJSON;
using TMPro;
public class ClickWhiteArchiBtn : MonoBehaviour
{
   
  public GameObject ScenarioTab, MatrixTab, ArchitectureTab, MonitorTab;
  private Button btnScenario, btnMatrix, btnArchi, btnMonitor;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("[Script] ClickWhiteArchiBtn - start");
        btnArchi = GameObject.Find("Architecture").GetComponent<Button>();
        btnMatrix = GameObject.Find("DefenseMatrix").GetComponent<Button>();
        btnScenario = GameObject.Find("AttackScenario").GetComponent<Button>();
        btnMonitor = GameObject.Find("SecurityMonitoring").GetComponent<Button>();

    }

   
   // Architecture 버튼 클릭
    public void ButtonArchiClickled(){
        Debug.Log("ButtonArchiClickled");
        ScenarioTab.SetActive(false);
        MatrixTab.SetActive(false);
        ArchitectureTab.SetActive(true);
        MonitorTab.SetActive(false);
    }
}
