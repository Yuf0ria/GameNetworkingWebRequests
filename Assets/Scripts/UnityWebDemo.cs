using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class UnityWebDemo : MonoBehaviour
{
    public const string BASE_URL = "https://api.restful-api.dev/";

    public IEnumerator Get<T>(string route, Action<T> OnSuccess, Action<string> OnError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BASE_URL + route))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.downloadHandler.text);

                try
                {
                    var objData = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                    OnSuccess?.Invoke(objData);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else
            {
                OnError?.Invoke(webRequest.error);
            }
        }
    }

    public IEnumerator Post<T>(string route, string jsonBody, Action<T> OnSuccess, Action<string> OnError)
    {

        using (UnityWebRequest webRequest = new UnityWebRequest(BASE_URL + route, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.downloadHandler.text);

                try
                {
                    //var objData = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                    //OnSuccess?.Invoke(objData);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else
            {
                OnError?.Invoke(webRequest.error);
            }
        }
    }
    
    

    [ContextMenu("Get Objects")]
    public void GetObjects()
    {
        StartCoroutine(Get<ObjectData[]>("objects",
            (obj) =>
            {
                Debug.Log("Success");
            }, (error) =>
            {
                Debug.LogError(error);
            }));
    }
    
    [ContextMenu("Post Object")]
    public void PostObject()
    {
        var objdata = new PostObjectData()
        {
            name = "I Fon Puro Makusu",
            data = new DataContent()
            {
                CpuModel = "em Faive",
                HardDiskSize = "256GB",
                price = 999f,
                year = 1990
            }
        };

        var serializedContent = JsonConvert.SerializeObject(objdata);
        
        StartCoroutine(Post<PostObjectDataResponse>(
            "objects", 
            serializedContent,  
            (obj) =>
            {
                Debug.Log("Success");
            }, (error) =>
            {
                Debug.LogError(error);
            }));
    }
}

[System.Serializable]
public struct ObjectDatal
{
    public string id;
    public string name;
    //public string data;
}

[System.Serializable]
public struct PostObjectData
{
    public string name;
    public DataContent data;
}

[System.Serializable]
public struct PostObjectDataResponse
{
    public string name;
    public DataContent data;
    public string createdAt;
}

[System.Serializable]
public struct DataContent
{
    public int year;
    public float price;
    [JsonProperty("CPU model")]
    public string CpuModel;
    [JsonProperty("Hard disk size")]
    public string HardDiskSize;
}
