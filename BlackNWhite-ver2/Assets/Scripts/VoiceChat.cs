using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;
using agora_utilities;
#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class VoiceChat : MonoBehaviour
{
    [SerializeField]
    private IRtcEngine mRtcEngine { get; set; }
    private string AppID = "c00442285cf7479498eb060d982c473c";
    //private string channelName = "";

    public Button micBtn;
    public Button audioBtn;
    bool micState = false;  // 마이크
    bool audioState = true; // 스피커


    // load agora engine
    public void loadEngine(string appId)
    {
        // start sdk
        Debug.Log("initializeEngine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    // public void join(string channel, bool enableVideoOrNot, bool muted = false)
    // {
    //     Debug.Log("calling join (channel = " + channel + ")");

    //     mRtcEngine.JoinChannel(channel, "extra", 0);
    // }


    void Start()
    {
        Debug.Log("test ActionBar");

        micBtn.onClick.AddListener(OnClickMic);
        audioBtn.onClick.AddListener(OnClickAudio);

        if (mRtcEngine != null)
        {
            Debug.Log("Leave & Destroy at Start");
            mRtcEngine.LeaveChannel();
            IRtcEngine.Destroy();
        }

        mRtcEngine = IRtcEngine.GetEngine(AppID);
        JoinChannel();

        mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) =>
        {
            string joinSuccessMessage = string.Format("joinChannel callback uid: {0}, channel: {1}, version: ", uid, channelName);
            Debug.Log(joinSuccessMessage);
        };

        // // 말하면 색 변환
        // mRtcEngine.OnVolumeIndication += (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume) =>
        // {
        //     for (int idx = 0; idx < speakerNumber; idx++)
        //     {
        //         string volumeIndicationMessage = string.Format("{0} onVolumeIndication {1} {2}", speakerNumber, speakers[idx].uid, speakers[idx].volume);
        //         Debug.Log(volumeIndicationMessage);
        //     }

        //     if(totalVolume >= 10)
        //     {
        //         ColorBlock Vol_UP = swtichBtn.colors;
        //         Vol_UP.normalColor = new Color32(69, 199, 247, 255);
        //         swtichBtn.colors = Vol_UP;
        //     }
        //     else
        //     {
        //         ColorBlock Vol_DW = swtichBtn.colors;
        //         Vol_DW.normalColor = new Color32(255, 255, 255, 255);
        //         swtichBtn.colors = Vol_DW;
        //     }
        // };
    }

    public void JoinChannel()
    {
        string channelName = "test";
        Debug.Log(string.Format("tap joinChannel with channel name {0}", channelName));

        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        mRtcEngine.JoinChannel(channelName, "extra", 0);
        mRtcEngine.EnableAudioVolumeIndication(11, 3, !micState);
        mRtcEngine.EnableLocalAudio(audioState);
        mRtcEngine.MuteLocalAudioStream(micState);
    }

    void OnApplicationQuit()
    {
        Debug.Log("Engine Destroy call!!");
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
            Debug.Log("Engine Destroy");
        }
    }


    public void OnClickMic()
    {
        micState = !micState;
        Debug.Log("Mic : " + micState);
        mRtcEngine.MuteLocalAudioStream(micState);
        // mRtcEngine.EnableAudioVolumeIndication(11, 3, !micState);
    }
    
    public void OnClickAudio()
    {
        audioState = !audioState;
        Debug.Log("Audio : " + audioState);
        mRtcEngine.MuteAllRemoteAudioStreams(audioState);
    }

    // public void swtichChannel()
    // {
    //     Debug.Log("Swtich Channel CALL");
    //     mRtcEngine.LeaveChannel();
    //     Debug.Log("Leave Channel");
    //     mRtcEngine.JoinChannel("aaa", "extra", 0);
        
    // }

    

    

}
