using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class onClickMainBtn : MonoBehaviour
{
  
    public Button btn1;
    public Sprite activeImg;
    public Sprite nonActiveImg;
    bool micState = false;
    bool audioState = true;

    public void micButtonClicked(){
        if (!micState){
            btn1.GetComponent<Image>().sprite = activeImg ;
            micState = true;
        }   
        else{
            btn1.GetComponent<Image>().sprite = nonActiveImg;
            micState = false;
        }
    }

    public void audioButtonClicked(){
        if (audioState){
            btn1.GetComponent<Image>().sprite = nonActiveImg ;
            audioState = false;
        }   
        else{
            btn1.GetComponent<Image>().sprite = activeImg;
            audioState = true;
        }
    }

}
