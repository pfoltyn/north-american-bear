﻿using UnityEngine;
using System.Collections;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class UnityAdsHelper : MonoBehaviour
{
	private const string adsTimeId = "adverts";

	public string iosGameID = "24300";
	public string androidGameID = "24299";
	
	public bool enableTestMode = true;
	public bool showInfoLogs;
	public bool showDebugLogs;
	public bool showWarningLogs = true;
	public bool showErrorLogs = true;

#if UNITY_IOS || UNITY_ANDROID
	
	public static void ShowAdds(string levelToLoad)
	{
		float adsTime = PlayerPrefs.GetFloat(adsTimeId, 0f);
		if (adsTime > Time.time)
		{
			adsTime = Time.time;
			PlayerPrefs.SetFloat(adsTimeId, adsTime);
		}
		if ((levelToLoad != Utils.lvlSettings) &&
		    (levelToLoad != Utils.lvlMenu) &&
		    (Mathf.Abs(Time.time - adsTime) > 60f))
		{
			PlayerPrefs.SetFloat(adsTimeId, Time.time);
			UnityAdsHelper.ShowAd();
		}
		PlayerPrefs.Save();
	}

	//--- Unity Ads Setup and Initialization

	void Start ()
	{
		Debug.Log("Attempting to initialize Unity Ads now...");

		string gameID = null;
	#if UNITY_IOS
		gameID = iosGameID;
	#elif UNITY_ANDROID
		gameID = androidGameID;
	#endif
		
		if (Advertisement.isInitialized)
		{
			Debug.LogWarning("Unity Ads is already initialized.");
		}
		else if (string.IsNullOrEmpty(gameID))
		{
			Debug.LogError("The game ID value is not set. A valid game ID is required to initialize Unity Ads.");
		}
		else
		{
			Advertisement.debugLevel = Advertisement.DebugLevel.NONE;	
			if (showInfoLogs) Advertisement.debugLevel    |= Advertisement.DebugLevel.INFO;
			if (showDebugLogs) Advertisement.debugLevel   |= Advertisement.DebugLevel.DEBUG;
			if (showWarningLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.WARNING;
			if (showErrorLogs) Advertisement.debugLevel   |= Advertisement.DebugLevel.ERROR;
			
			if (enableTestMode && !Debug.isDebugBuild)
			{
				Debug.LogWarning("Development Build must be enabled in Build Settings to enable test mode for Unity Ads.");
			}
			
			bool isTestModeEnabled = Debug.isDebugBuild && enableTestMode;
			Debug.Log(string.Format("Initializing Unity Ads for game ID {0} with test mode {1}...",
			                        gameID, isTestModeEnabled ? "enabled" : "disabled"));

			Advertisement.allowPrecache = true;
			Advertisement.Initialize(gameID,isTestModeEnabled);
		}
	}
	
	//--- Static Helper Methods ---
	public static bool IsReady () 
	{
		return IsReady(null);
	}

	public static bool IsReady (string zoneID) 
	{ 
		if (string.IsNullOrEmpty(zoneID)) zoneID = null;
		
		return Advertisement.isReady(zoneID);
	}

	public static void ShowAd ()
	{
		ShowAd(null);
	}

	public static void ShowAd (string zoneID)
	{
		if (string.IsNullOrEmpty(zoneID)) zoneID = null;
		
		if (Advertisement.isReady(zoneID))
		{
			ShowOptions options = new ShowOptions();
			options.resultCallback = HandleShowResult;
			options.pause = true;

			Advertisement.Show(zoneID,options);
		}
		else 
		{
			Debug.LogWarning(string.Format("Unable to show ad. The ad placement zone ($0) is not ready.",
			                               zoneID == null ? "default" : zoneID));
		}
	}

	private static void HandleShowResult (ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
#else
	void Start ()
	{
		Debug.LogWarning("Unity Ads is not supported under the current build platform.");
	}

	public static void ShowAd (string zoneID = null)
	{
		Debug.LogError("Failed to show ad. Unity Ads is not supported under the current build platform.");
	}

	public static bool IsReady (string zoneID = null) { return false; }
#endif
}
