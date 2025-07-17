using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Android;
using System.Globalization;

using System; 
using System.Collections.Generic;


/**
 * INNavigation.cs
 *
 * Created by Ivan Sugrobov.
 * Copyright (c) 2014-2020 Indoors Navigation LLC. All rights reserved.
 */

public class INNavigation : MonoBehaviour
{
    public Transform User;


    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        ExecuteAndroidMethod("initialize", "");
#endif

#if UNITY_IOS && !UNITY_EDITOR
        initialize();
#endif
    }

    public void OnNavigationInitialized(string dataString)
    {
        LoadBuilding();
    }

    public void LoadBuilding()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
            ExecuteAndroidMethod("loadBuilding", "");
#endif

#if UNITY_IOS && !UNITY_EDITOR
        loadBuilding();
#endif
    }

    public void OnBuildingLoadingStarted(string dataString)
    {

    }

    public void OnBuildingLoaded(string dataString)
    {
        StartNavigation();
    }

    public void StartNavigation()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        ExecuteAndroidMethod("startNavigation", "");
#endif
#if UNITY_IOS && !UNITY_EDITOR
        startNavigation();
#endif

    }

    public void OnNavigationStarted(string dataString)
    {

    }

    public void StopNavigation()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        ExecuteAndroidMethod("stopNavigation", "");
#endif
#if UNITY_IOS && !UNITY_EDITOR
        stopNavigation();
#endif

    }

    public void OnNavigationStoped(string dataString)
    {

    }

    public void OnUpdateUserPosition(string dataString)
    {
        Debug.Log(dataString); 

        string[] dataArray = dataString.Split(' ');
        float x = float.Parse(dataArray[0], CultureInfo.InvariantCulture);
        float y = float.Parse(dataArray[1], CultureInfo.InvariantCulture);
        int floorId = int.Parse(dataArray[2]);

        User.localPosition = new Vector3(y, 0, x);
    }

    void OnApplicationFocus(bool focused)
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if (focused)
        {  
               List<string> requiredPermissions = new List<string> { Permission.FineLocation };
       
           
               int apiVersion = GetAndroidAPIVersion();
               
               
               if (apiVersion >= 31)
               {
                   requiredPermissions.Add("android.permission.BLUETOOTH_SCAN");
                   requiredPermissions.Add("android.permission.BLUETOOTH_CONNECT");
                   requiredPermissions.Add("android.permission.BLUETOOTH_ADVERTISE");
               }
       
               
               bool allGranted = true;
               foreach (string perm in requiredPermissions)
               {
                   if (!Permission.HasUserAuthorizedPermission(perm))
                   {
                       allGranted = false;
                       break;
                   }
               }
       
               if (allGranted)
               {
                   StartNavigation();
               }
               else
               {
                   Permission.RequestUserPermissions(requiredPermissions.ToArray());
               }

        }
        else
        {
            StopNavigation();
        }
#endif
#if UNITY_IOS && !UNITY_EDITOR
        if (focused)
        {
            StartNavigation();
        }
        else
        {
            StopNavigation();
        }
#endif
    }
#if PLATFORM_ANDROID && !UNITY_EDITOR
    public static void ExecuteAndroidMethod(string methodName, string str)
    {
        using (var unityNavigation = new AndroidJavaObject("com.unity3d.player.INNavigationBridge"))
        {
            unityNavigation.Call(methodName, str);
            Debug.Log(methodName);
        }
    }
#endif
#if UNITY_IOS && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void initialize();

    [DllImport("__Internal")]
    private static extern void loadBuilding();

    [DllImport("__Internal")]
    private static extern void startNavigation();

    [DllImport("__Internal")]
    private static extern void stopNavigation();
#endif

    private void OnGUI()
    {
        //if(GUILayout.Button("Initialize"))
        //{
        //    Initialize();
        //}
        //if (GUILayout.Button("LoadBuilding"))
        //{
        //    LoadBuilding();
        //}
        //if (GUILayout.Button("StartNavigation"))
        //{
        //    StartNavigation();
        //}
        //if (GUILayout.Button("StopNavigation"))
        //{
        //    StopNavigation();
        //}
    }

private int GetAndroidAPIVersion()
{
    int apiVersion = 0;
    if (apiVersion == 0)
    {
        try
        {
            using (AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                apiVersion = version.GetStatic<int>("SDK_INT");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to get API version: " + e.Message);
        }
    }
    
    return apiVersion;
}
}
