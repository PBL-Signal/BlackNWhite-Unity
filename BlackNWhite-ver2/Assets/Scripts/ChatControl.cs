using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using TMPro;
using MiniJSON;

public class ChatControl : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private GameObject textChatPrefab;
    [SerializeField]
    private Transform scrollViewContent;

    private string nickname;

    GameObject GameManager;
    private SocketManager socketManager = null;

    private string[] colors = new string[12] {"FF131B", "EC7E0E", "F8F558", "FF77F2", "BFDE36", "2DACFF", 
                                                "222A79", "2DE9D0", "B88BFF", "FADEC0", "FFFFFF", "000000"};
    
    void Start()
    {
        Application.runInBackground = true;
        
        GameManager = GameObject.Find("Socket");
        socketManager = GameManager.GetComponent<SocketInit>().socketManager;

        socketManager.Socket.On("sessionInfo", (string data) => {
            var session = Json.Deserialize(data) as Dictionary<string, object>;
            Debug.Log("[Chat Control - SectionState] nickname: " + session["nickname"]);
            nickname = session["nickname"].ToString();
        });

        socketManager.Socket.On("Update Chat", (string time, string nickname, int colorIdx, string chat) => UpdateChat(time, nickname, colorIdx, chat));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputField.isFocused == false)
        {
            inputField.ActivateInputField();
        }
    }

    public void OnEndEditEventMethod() 
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendChat();
        }
    }

    public void SendChat()
    {
        if (inputField.text.Equals(""))
        {
            return;
        }

        socketManager.Socket.Emit("Send Chat", inputField.text);
        inputField.text = "";
    }


    public void UpdateChat(string time, string nickname, int colorIdx, string chat) {
        GameObject clone = Instantiate(textChatPrefab, scrollViewContent);
        Debug.Log("user profile color : " + colorIdx);
        clone.GetComponent<TextMeshProUGUI>().text = $"{time}    <color=#{colors[colorIdx]}>{nickname}</color> : {chat}";
    }
}
