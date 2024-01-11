using UnityEngine;
using System.Collections;
using TMPro;

public class Canvas_Timer : MonoBehaviour
{
    GameManager gm;

    public TMP_Text timerText;
    int timeLimit;
    int timeLeft;

    bool end;
    // Start is called before the first frame update
    public void Init(int _timeLimit)
    {
        gm = GameManager.gm;

        timeLimit = _timeLimit;
        timeLeft = timeLimit;
        StartCoroutine("PassSecond");
    }

    IEnumerator PassSecond()
    {
        if (!end)
        {
            yield return new WaitForSeconds(1);

            timeLeft--;
            timeLeft = Mathf.Clamp(timeLeft, 0, timeLimit);

            ShowTimeLeft();

            if (timeLeft <= 0)
                gm.TimeUp();
            else
                StartCoroutine("PassSecond");
        }
    }

    public void ChangeTime(int _difference)
    {
        timeLeft += _difference;
        timeLeft = Mathf.Clamp(timeLeft, 0, timeLimit);
        ShowTimeLeft();
    }

    void ShowTimeLeft()
    {
        int minutes = 0;
        int seconds = timeLeft;
        while (seconds > 59)
        {
            minutes++;
            seconds -= 60;
        }

        if (seconds > 9)
            timerText.text = minutes + ":" + seconds;
        else
            timerText.text = minutes + ":0" + seconds;
    }

    public void EndGame()
    {
        end = true;
        timerText.gameObject.SetActive(false);
        StopCoroutine("PassSecond");
    }

    public string GetTotalTime()
    {
        int minutes = 0;
        int seconds = timeLimit - timeLeft;
        while (seconds > 59)
        {
            minutes++;
            seconds -= 60;
        }

        if (seconds > 9)
            return (minutes + ":" + seconds);
        else
            return (minutes + ":0" + seconds);
    }

    public int GetTimeInSeconds()
    {
        return timeLimit - timeLeft;
    }
}
