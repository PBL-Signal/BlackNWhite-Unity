using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupUI : MonoBehaviour
{
    [Header("Popup")]
    public Animator popupAni;
    public GameObject popupBox;
    public GameObject popupTxt;

    private WaitForSeconds _UIDelay1 = new WaitForSeconds(0.7f);
    private WaitForSeconds _UIDelay2 = new WaitForSeconds(0.3f);

    // Start is called before the first frame update
    void Start()
    {
        popupBox.SetActive(false);
        Debug.Log("POPUP script - start");
    }

    // 팝업 창 
    public void POP(string message)
    {
        Debug.Log("POPUP script - pop");
        popupTxt.GetComponent<TextMeshProUGUI>().text = message;
        popupBox.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(PopDelay());
    }

    // 딜레이 설정 
    IEnumerator PopDelay(){
        Debug.Log("POPUP script - delay");
        popupBox.SetActive(true);
        popupAni.SetBool("isOn", true);
        yield return _UIDelay1;
        popupAni.SetBool("isOn", false);
        yield return _UIDelay2;
        popupBox.SetActive(false);
    }
}
