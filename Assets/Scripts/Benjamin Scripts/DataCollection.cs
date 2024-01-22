using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DataCollection : MonoBehaviour
{
    private string intHintsUsed = "test";
    private string intTimesIncorrect = "test";
    private string intTotalTime = "test";

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfe79NkAVfNv339oSyJTofrB4jsqAswbeFUhZBb8fnEdKGd_Q/formResponse";
    public void Start()
    {
        WWWForm form = new WWWForm();

        StartCoroutine(Post(intHintsUsed, intTimesIncorrect, intTotalTime));
    }

    IEnumerator Post(string intHintUsed, string intTimesIncorrect, string intTotalTime)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.741020485", intHintUsed);
        form.AddField("entry.1388233873", intTimesIncorrect);
        form.AddField("entry.2012214965", intTotalTime);

        using (UnityWebRequest www = UnityWebRequest.Post(BASE_URL, form))
        {
            

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("no connection aquired");
            }
            else
            {
                Debug.Log("Connected");

                yield return www.SendWebRequest();
            }
        }
    }

    public void SendData()
    {
        StartCoroutine(Post(intHintsUsed, intTimesIncorrect, intTotalTime));
    }
}
