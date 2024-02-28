using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public static Camera mainCamera;

    string teamName;

    public Canvas_Sequence[] sequence = new Canvas_Sequence[4];

    public Vector2 aspectRatio = new Vector2(4, 3);

    public QRCodeScanner scannerObject;
    public Canvas_Stories storyMenu;
    public int totalTimeInMinutes = 30;
    public Canvas_Timer timer;
    public Canvas_Timer timerReal;
    public Canvas_Dialogue timeUpPrompt;
    public Canvas_Dialogue tryAgainPrompt;
    public Canvas_Dialogue nullInputPrompt;
    public Canvas_Dialogue resultScreen;

    public Widget_Video videoPlayer;
    public Widget_VoiceOver audioPlayer;

    //Variables to track
    int[] numbHintsUsed;
    int[] incorrectAnswers;
    int[] secondsTaken;
    string[] timeTaken;

    string timePassword;

    void Awake()
    {
        //Makes a GameManager singleton
        if (gm != this && gm != null)
            Destroy(gameObject);
        else if (gm != this)
            gm = this;

        float maxHeight = Screen.resolutions[Screen.resolutions.Length - 1].height;
        float maxWidth = (maxHeight / aspectRatio.x) * aspectRatio.y;

        //Set Resolution based off hierarchy specifications
        Screen.SetResolution((int)maxHeight, (int)maxWidth, false);

        mainCamera = FindObjectOfType<Camera>();

        numbHintsUsed = new int[sequence.Length];
        incorrectAnswers = new int[sequence.Length];
        secondsTaken = new int[sequence.Length];
        timeTaken = new string[sequence.Length];

        if (PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetString("TimePassword", "Education24");
            PlayerPrefs.SetInt("TotalTimeMinutes", 30);
            PlayerPrefs.SetInt("FirstTime", 1);
        }

        timePassword = PlayerPrefs.GetString("TimePassword");
        totalTimeInMinutes = PlayerPrefs.GetInt("TotalTimeMinutes");

        if (timePassword == "")
        {
            PlayerPrefs.SetString("TimePassword", "Education24");
            timePassword = PlayerPrefs.GetString("TimePassword");
        }
    }

    public void StartChallenge(int _challengeIdx)
    {
        Canvas_Sequence newSequence = Instantiate(sequence[_challengeIdx], Vector3.zero, Quaternion.identity);
        newSequence.Init(_challengeIdx);
        newSequence.gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        if (timerReal == null)
        {
            timerReal = Instantiate(timer, Vector3.zero, Quaternion.identity);
            timerReal.Init(totalTimeInMinutes * 60);
        }
    }

    public void ChangeTime(int _difference)
    {
        if (timerReal != null)
            timerReal.ChangeTime(_difference);
    }

    public void TimeUp()
    {
        DataCollection dc = FindObjectOfType<DataCollection>();
        if (dc != null)
            dc.GrabData();

        Canvas_Dialogue instance = Instantiate(timeUpPrompt, Vector3.zero, Quaternion.identity);
        instance.Init(null);
    }

    public void OpenResults()
    {
        Canvas_Dialogue instance = Instantiate(resultScreen, Vector3.zero, Quaternion.identity);
        instance.Init(null);
    }

    public string GetTeamName()
    {
        return teamName;
    }

    public void SetTeamName(string _name)
    {
        teamName = _name;
    }

    public string GetTimeLeft()
    {
        if (timerReal != null)
            return timerReal.GetTotalTime();
        else
            return "69:420";
    }

    public void EndGame()
    {
        DataCollection dc = FindObjectOfType<DataCollection>();
        if (dc != null)
            dc.GrabData();

        if (timerReal != null)
        {
            timerReal.EndGame();
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void UseHint(int _idx)
    {
        numbHintsUsed[_idx]++;
    }

    public void IncorrectAnswer(int _idx)
    {
        incorrectAnswers[_idx]++;
    }

    public void RecordTime(int _idx, int _seconds)
    {
        secondsTaken[_idx] = _seconds;
        timeTaken[_idx] = timerReal.SecToDig(_seconds);
    }

    public int[] GetHintsUsed()
    {/*
        int hintsUsedTotal = 0;
        for (int i = 0; i < numbHintsUsed.Length; i++)
        {
            hintsUsedTotal += numbHintsUsed[i];
        }
        */
        return numbHintsUsed;
    }

    public int[] GetIncorrectAnswers()
    {
        /*
        int incorrectAnswersTotal = 0;
        for (int i = 0; i < incorrectAnswers.Length; i++)
        {
            incorrectAnswersTotal += incorrectAnswers[i];
        }*/

        return incorrectAnswers;
    }

    public int GetTimeInSeconds()
    {
        return timerReal.GetTimeInSeconds();
    }

    public string GetTimeString()
    {
        return timerReal.GetTotalTime();
    }

    public int[] GetChallengeSeconds()
    {
        return secondsTaken;
    }

    public string[] GetChallengeStrings()
    {
        return timeTaken;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("" + GetIncorrectAnswers() + " " + GetTimeInSeconds() + " " + GetTimeString() + " " + GetHintsUsed());
        }
    }

    public string GetPassword()
    {
        return timePassword;
    }

    public void SetPassword(string _password)
    {
        timePassword = _password;
    }
}
