using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeColor : MonoBehaviour
{
    public Button btn1;
    public TMP_Text txt1;
    
    bool isClicked = false;

    public void ButtonClicked(){
        // ColorBlock OnColor = btn1.colors;

        // OnColor.normalColor = new Color32(69,199,247,225);
        // btn1.color = OnColor;
        if (! isClicked){
            btn1.GetComponent<Image>().color = Color.green;
            txt1.text = "ON";
            isClicked = true;
            // Debug.Log("HEllo11");
        }   
        else{
            btn1.GetComponent<Image>().color = Color.red;
            txt1.text = "OFF";
            isClicked = false;
            // Debug.Log("HEllo22");
        }
        // btn.color = Random.ColorHSV();
        
    }

}
