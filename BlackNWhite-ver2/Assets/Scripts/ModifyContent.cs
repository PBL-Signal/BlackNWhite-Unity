using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyContent : MonoBehaviour
{
    public Text attackName, AttackDetail;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClickPoint(){
        Debug.Log("point click!");

        attackName.GetComponent<Text>().text=attackName.GetComponent<Text>().name;
        AttackDetail.GetComponent<Text>().text="변경";

    }
}
