#if UNITY_IOS
using System.Runtime.InteropServices;
#endif
using System;
using UnityEngine;


public static class NativeShare
{
    public static void Share(string filePath)
    {
#if UNITY_EDITOR
        Debug.Log("Nope");
#elif UNITY_ANDROID
        ShareAndroid("#BobbleCloud", "#BobbleCloud", filePath);
#elif UNITY_IOS
        ShareIOS("#BobbleCloud", "Share", filePath);
#endif
    }

#if UNITY_ANDROID
    public static void ShareAndroid(string body, string url, string filePath)
    {
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
        using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
        {
            using (intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND")))
            { }
            using (intentObject.Call<AndroidJavaObject>("setType", "image/png"))
            { }
            using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body))
            { }

            //if (!string.IsNullOrEmpty(url))
            //{
            //    // attach url
            //    using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
            //    using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", url))
            //    using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject))
            //    { }
            //}
            //else if (filePath != null)
            if (filePath != null)
            {
                // attach extra files (pictures, pdf, etc.)
                using (AndroidJavaClass fileProviderClass = new AndroidJavaClass("android.support.v4.content.FileProvider"))
                using (AndroidJavaObject unityContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
                using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uris = new AndroidJavaObject("java.util.ArrayList"))
                {
                    string packageName = unityContext.Call<string>("getPackageName");
                    string authority = packageName + ".provider";

                    AndroidJavaObject fileObj = new AndroidJavaObject("java.io.File", filePath);
                    AndroidJavaObject uriObj = fileProviderClass.CallStatic<AndroidJavaObject>("getUriForFile", unityContext, authority, fileObj);

                    int FLAG_GRANT_READ_URI_PERMISSION = intentObject.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION");
                    intentObject.Call<AndroidJavaObject>("addFlags", FLAG_GRANT_READ_URI_PERMISSION);

                    using (intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObj))
                    { }
                }
            }

            // finally start application
            AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share");
            currentActivity.Call("startActivity", jChooser);
        }
        //AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //intentObject.Call<AndroidJavaObject>("setAction",
        //                                     intentClass.GetStatic<string>("ACTION_SEND"));
        //AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        //AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse",
        //                                                                     "file://" + filePath);
        //intentObject.Call<AndroidJavaObject>("putExtra",
        //                                     intentClass.GetStatic<string>("EXTRA_STREAM"),
        //                                     uriObject);
        //intentObject.Call<AndroidJavaObject>("putExtra",
        //                                     intentClass.GetStatic<string>("EXTRA_SUBJECT"),
        //                                     body);
        //intentObject.Call<AndroidJavaObject>("putExtra",
        //                                     intentClass.GetStatic<string>("EXTRA_TEXT"),
        //                                     url);
        //intentObject.Call<AndroidJavaObject>("setType", "image/*");
        //AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser",
        //                                                                      intentObject,
        //                                                                      "Share");
        //currentActivity.Call("startActivity", chooser);
    }
#endif

#if UNITY_IOS
    public struct ConfigStruct
    {
        public string title;
        public string message;
    }

    [DllImport("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

    public struct SocialSharingStruct
    {
        public string text;
        public string subject;
        public string filePath;
    }

    [DllImport("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);

    public static void ShareIOS(string title, string message)
    {
        ConfigStruct conf = new ConfigStruct
        {
            title = title,
            message = message
        };
        showAlertMessage(ref conf);
    }

    public static void ShareIOS(string body, string subject, string filePath)
    {
        SocialSharingStruct conf = new SocialSharingStruct
        {
            text = body
        };

        conf.filePath = filePath;
        conf.subject = subject;

        showSocialSharing(ref conf);
    }
#endif
}
