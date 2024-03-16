using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DataCollection : MonoBehaviour
{
    private string[] strData;
    private List<string> strDataList = new List<string>();

    private bool boolHaveInternet = false;
    private bool boolFirstTime = true;

    GameObject goGameManager;

    [SerializeField]
    private string BASE_URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfe79NkAVfNv339oSyJTofrB4jsqAswbeFUhZBb8fnEdKGd_Q/formResponse";
    private string TEST_URL = "https://google.com/";
    public void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "DataCollection.txt"))
        {
            File.CreateText(Application.persistentDataPath + "DataCollection.txt");
            Invoke("Reset", 1);
            return;
        }

        strData = File.ReadAllLines(Application.persistentDataPath + "DataCollection.txt");
        Debug.Log(Application.persistentDataPath);

        foreach (string data in strData)
        {
            strDataList.Add(data);
        }

        if (boolFirstTime == true)
        {
            StartCoroutine(InternetCheck());
        }        

    }

    IEnumerator InternetCheck() 
    {
        UnityWebRequest www = new UnityWebRequest(TEST_URL);
        yield return www.SendWebRequest();

        if (www.error != null)
        {
            boolFirstTime = false;
            boolHaveInternet = false;
            Debug.Log(www.result);

            Debug.Log(strDataList.Count);
        }
        else
        {
            boolFirstTime = false;
            boolHaveInternet = true;
            Debug.Log(www.result);

            if (strDataList.Count != 0)
            {
                do
                {
                    if (boolHaveInternet == true)
                    {
                        StartCoroutine(Post(strDataList));
                        for (int i = 0; i < 5; i++)
                        {
                            strDataList.RemoveAt(0);
                        }
                    }

                } while (strDataList.Count != 0);

                using (StreamWriter writer = new StreamWriter(Application.persistentDataPath + "DataCollection.txt", false))
                {
                    writer.Write("");
                }

            }
        }
    }

    IEnumerator Post(List<string> strDataList)
    {
        WWWForm form = new WWWForm();

        form.AddField("entry.741020485", strDataList[0]);
        form.AddField("entry.1388233873", strDataList[1]);
        form.AddField("entry.2012214965", strDataList[2]);
        form.AddField("entry.931242414", strDataList[3]);
        form.AddField("entry.1318815076", strDataList[4]);

        using (UnityWebRequest www = UnityWebRequest.Post(BASE_URL, form))
        {
            

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Cannot connect to data Aquzition");
            }
            else
            {
                Debug.Log("Connected");


                yield return www.SendWebRequest();
            }

            Debug.Log(www.result);
        }
    }

    public void GrabData()
    {
        foreach (string data in strData)
        {
            strDataList.Add(data);
        }

        Debug.Log(strDataList.Count);

        using (StreamWriter writer = new StreamWriter(Application.persistentDataPath + "DataCollection.txt", append: true))
        {
            int intGetTime;
            int[] intHintsUsed, intIncorrect;
            string[] strEachChallengeTime;
            bool boolIsFirstHint = false, boolIsFirstIncorrect = false;

            if (strDataList.Count > 0)
            {
                //writer.WriteLine("");
            }

            intHintsUsed = gameObject.GetComponent<GameManager>().GetHintsUsed();
            intIncorrect = gameObject.GetComponent<GameManager>().GetIncorrectAnswers();
            intGetTime = gameObject.GetComponent<GameManager>().GetTimeInSeconds();
            strEachChallengeTime = gameObject.GetComponent<GameManager>().GetChallengeStrings();

            //writer.WriteLine(intHintsUsed);
            //writer.WriteLine(intIncorrect);

            foreach (int item in intHintsUsed)
            {
                if (boolIsFirstHint)
                {
                    writer.Write(item + " , ");
                }
                else
                {
                    boolIsFirstHint = true;
                }              
            }

            writer.Write("\n");

            foreach (int item in intIncorrect)
            {
                if (boolIsFirstIncorrect)
                {
                    writer.Write(item + " , ");
                }
                else
                {
                    boolIsFirstIncorrect = true;
                }               
            }

            writer.Write("\n");

            writer.WriteLine(intGetTime);
            writer.WriteLine(System.DateTime.Now.ToString());

            foreach (string item in strEachChallengeTime)
            {
                if (boolIsFirstIncorrect)
                {
                    writer.Write(item + " , ");
                }
                else
                {
                    boolIsFirstIncorrect = true;
                }
            }

            writer.Write("\n");

        }

         Debug.Log("Done");
    }

    void Reset()
    {
        SceneManager.LoadScene("Main");
    }
}
