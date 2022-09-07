using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickBackground : MonoBehaviour
{
    private GameObject GameManager;
    private bool teamValue;
    string sceneName;

    GameObject ClickCompanyObject;

    void Start(){
        ClickCompanyObject = GameObject.Find("ClickCompany");

        GameManager = GameObject.Find("ClickCompany");
        teamValue = GameManager.GetComponent<OnClickCompany>().teamValue;
        sceneName = GameManager.GetComponent<OnClickCompany>().sceneName;
        Debug.Log("ClickBackground teamValue : " + teamValue);
        Debug.Log("scene name : " + sceneName);

  
    }

    public void SceneClosed()
    {
        Debug.Log("[SceneClosed] UnloadSceneAync_SceneName : " + sceneName);

        GameManager.GetComponent<OnClickCompany>().CloseDetailScene();
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
