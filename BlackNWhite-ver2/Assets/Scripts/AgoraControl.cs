using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
using TMPro;
using MiniJSON;
#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
using agora_gaming_rtc;



public class AgoraControl : MonoBehaviour
{
    
    [SerializeField]
    private string AppID = "c00442285cf7479498eb060d982c473c";

    private IRtcEngine mRtcEngine = null;

    void Awake()
    {
        CheckAppId();
        
    }

    void Start()
    {
        // #if (UNITY_2018_3_OR_NEWER)
        //     if (Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
            
        //     } else {
        //         Permission.RequestUserPermission(Permission.Microphone);
        //     }
        // #endif
        
        //joinChannel.onClick.AddListener(JoinChannel);
        //leaveChannel.onClick.AddListener(LeaveChannel);
        //muteButton.onClick.AddListener(MuteButtonTapped);

        mRtcEngine = IRtcEngine.GetEngine(AppID);
        getSdkVersion();
        JoinChannel();
        //versionText.GetComponent<Text>().text = "Version : " + getSdkVersion();


        // Print LOG
        mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) =>
        {
            string joinSuccessMessage = string.Format("joinChannel callback uid: {0}, channel: {1}, version: {2}", uid, channelName, getSdkVersion());
            Debug.Log(joinSuccessMessage);
            //mShownMessage.GetComponent<Text>().text = (joinSuccessMessage);
            //muteButton.enabled = true;
        };

        mRtcEngine.OnLeaveChannel += (RtcStats stats) =>
        {
            string leaveChannelMessage = string.Format("onLeaveChannel callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}", stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate);
            Debug.Log(leaveChannelMessage);
            //mShownMessage.GetComponent<Text>().text = (leaveChannelMessage);
            //muteButton.enabled = false;

            // reset the mute button state
            if (isMuted)
            {
                MuteButtonTapped();
            }
        };

        mRtcEngine.OnUserJoined += (uint uid, int elapsed) =>
        {
            string userJoinedMessage = string.Format("onUserJoined callback uid {0} {1}", uid, elapsed);
            Debug.Log(userJoinedMessage);
        };

        mRtcEngine.OnUserOffline += (uint uid, USER_OFFLINE_REASON reason) =>
        {
            string userOfflineMessage = string.Format("onUserOffline callback uid {0} {1}", uid, reason);
            Debug.Log(userOfflineMessage);
        };

        mRtcEngine.OnVolumeIndication += (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume) =>
        {
            if (speakerNumber == 0 || speakers == null)
            {
                Debug.Log(string.Format("onVolumeIndication only local {0}", totalVolume));
            }

            for (int idx = 0; idx < speakerNumber; idx++)
            {
                string volumeIndicationMessage = string.Format("{0} onVolumeIndication {1} {2}", speakerNumber, speakers[idx].uid, speakers[idx].volume);
                Debug.Log(volumeIndicationMessage);
            }
        };

        mRtcEngine.OnUserMutedAudio += (uint uid, bool muted) =>
        {
            string userMutedMessage = string.Format("onUserMuted callback uid {0} {1}", uid, muted);
            Debug.Log(userMutedMessage);
        };
        
        // Reports the statistics of the RtcEngine once every two seconds.
        mRtcEngine.OnRtcStats += (RtcStats stats) =>
        {
            string rtcStatsMessage = string.Format("onRtcStats callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}, tx(a) kbps: {5}, rx(a) kbps: {6} users {7}",
                stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate, stats.txAudioKBitRate, stats.rxAudioKBitRate, stats.userCount);
            Debug.Log(rtcStatsMessage);

            int lengthOfMixingFile = mRtcEngine.GetAudioMixingDuration();
            int currentTs = mRtcEngine.GetAudioMixingCurrentPosition();

            string mixingMessage = string.Format("Mixing File Meta {0}, {1}", lengthOfMixingFile, currentTs);
            Debug.Log(mixingMessage);
        };

        mRtcEngine.OnRequestToken += () =>
        {
            string requestKeyMessage = string.Format("OnRequestToken");
            Debug.Log(requestKeyMessage);
        };

        mRtcEngine.OnConnectionInterrupted += () =>
        {
            string interruptedMessage = string.Format("OnConnectionInterrupted");
            Debug.Log(interruptedMessage);
        };

        mRtcEngine.OnConnectionLost += () =>
        {
            string lostMessage = string.Format("OnConnectionLost");
            Debug.Log(lostMessage);
        };
    }

    //=========================================================================================

    public void JoinChannel()
    {
        // string channelName = mChannelNameInputField.text.Trim();
        string channelName = "test";
        Debug.Log(string.Format("tap joinChannel with channel name {0}", channelName));

        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        mRtcEngine.JoinChannel(channelName, "extra", 0);
    }

    public void LeaveChannel()
    {
        // mRtcEngine.LeaveChannel();
        // //string channelName = mChannelNameInputField.text.Trim();
        // Debug.Log(string.Format("left channel name {0}", channelName));
    }

    bool isMuted = false;
    void MuteButtonTapped()
    {
        string labeltext = isMuted ? "Mute" : "Unmute";
        //Text label = muteButton.GetComponentInChildren<Text>();
        // if (label != null)
        // {
        //     label.text = labeltext;
        // }
        isMuted = !isMuted;
        mRtcEngine.EnableLocalAudio(!isMuted);
    }

    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();
        }
    }

    private void CheckAppId()
    {
        Debug.Assert(AppID.Length > 10, "Please fill in your AppId first on Game Controller object.");
        GameObject go = GameObject.Find("AppIDText");
        if (go != null)
        {
            Text appIDText = go.GetComponent<Text>();
            if (appIDText != null)
            {
                if (string.IsNullOrEmpty(AppID))
                {
                    appIDText.text = "AppID: " + "UNDEFINED!";
                    appIDText.color = Color.red;
                }
                else
                {
                    appIDText.text = "AppID: " + AppID.Substring(0, 4) + "********" + AppID.Substring(AppID.Length - 4, 4);
                }
            }
        }
    }

    public string getSdkVersion()
    {
        string ver = IRtcEngine.GetSdkVersion();
        Debug.Log("AGORA : " + ver);
        return ver;
    }
}
