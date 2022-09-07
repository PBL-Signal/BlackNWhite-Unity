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

public class ClickBlackArchiBtn : MonoBehaviour
{

  public GameObject AttackScenarioTab, AttackMatrixTab, ArchitectureTab;
  private Button btnScenario, btnMatrix, btnArchi;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("[Script] ClickArchiBtn - start");
        btnArchi = GameObject.Find("Architecture").GetComponent<Button>();
        btnMatrix = GameObject.Find("AttackMatrix").GetComponent<Button>();
        btnScenario = GameObject.Find("AttackScenario").GetComponent<Button>();

    }

   
   // Architecture 버튼 클릭
    public void ButtonArchiClickled(){
        Debug.Log("ButtonArchiClickled");
        AttackScenarioTab.SetActive(false);
        AttackMatrixTab.SetActive(false);
        ArchitectureTab.SetActive(true);
    }
}
