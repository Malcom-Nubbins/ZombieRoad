using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GUI
{
    class DisplayToast
    {
        static AndroidJavaObject currentActivity;
        static string toastMessage;

        public static void ShowToast(string message)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                DisplayOnUIThread(message);
            }
        }

        private static void DisplayOnUIThread(string toastString)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            toastMessage = toastString;
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(ShowMessage));
        }

        private static void ShowMessage()
        {
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastMessage);
            AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, javaString, toastClass.GetStatic<int>("LENGTH_SHORT"));
            toast.Call("show");
        }
    }
}
