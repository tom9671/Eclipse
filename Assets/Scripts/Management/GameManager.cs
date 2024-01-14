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
    Canvas_Timer timerReal;
    public Canvas_TimeUp timeUpPrompt;
    public Canvas_Dialogue tryAgainPrompt;
    public Canvas_Dialogue nullInputPrompt;
    public Canvas_Dialogue resultScreen;

    public Widget_Video videoPlayer;
    public Widget_VoiceOver audioPlayer;

    //Variables to track
    int[] numbHintsUsed;
    int[] incorrectAnswers;

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

        if(PlayerPrefs.GetInt("FirstTime") == 0)
        {
            PlayerPrefs.SetInt("TotalTimeMinutes", 30);
            PlayerPrefs.SetInt("FirstTime", 1);
        }

        totalTimeInMinutes = PlayerPrefs.GetInt("TotalTimeMinutes");
    }

    public void StartChallenge(int _challengeIdx)
    {
        Canvas_Sequence newSequence = Instantiate(sequence[_challengeIdx], Vector3.zero, Quaternion.identity);
        newSequence.Init(_challengeIdx);
        newSequence.gameObject.SetActive(false);
    }

    public void StartTimer()
    {
        if(timerReal == null)
        {
            timerReal = Instantiate(timer, Vector3.zero, Quaternion.identity);
            timerReal.Init(totalTimeInMinutes * 60);
        }
    }

    public void ChangeTime(int _difference)
    {
        if(timerReal != null)
            timerReal.ChangeTime(_difference);
    }

    public void TimeUp()
    {
        Instantiate(timeUpPrompt, Vector3.zero, Quaternion.identity);
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
        if(timerReal != null)
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

    public int GetHintsUsed()
    {
        int hintsUsedTotal = 0;
        for(int i = 0; i < numbHintsUsed.Length; i++)
        {
            hintsUsedTotal += numbHintsUsed[i];
        }

        return hintsUsedTotal;
    }

    public int GetIncorrectAnswers()
    {
        int incorrectAnswersTotal = 0;
        for (int i = 0; i < incorrectAnswers.Length; i++)
        {
            incorrectAnswersTotal += incorrectAnswers[i];
        }

        return incorrectAnswersTotal;
    }

    public int GetTimeInSeconds()
    {
        return timer.GetTimeInSeconds();
    }

    public string GetTimeString()
    {
        return timer.GetTotalTime();
    }
}
