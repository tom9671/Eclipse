using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;

public class Canvas_Timer : MonoBehaviour
{
    GameManager gm;

    public TMP_Text timerText;
    public int warningPeriod = 120;
    int timeLimit;
    int timeLeft;

    public GameObject extraTimePanel;
    public TMP_InputField passwordField;
    public TMP_InputField extraTimeField;

    public string password = "education24";

    bool end;
    // Start is called before the first frame update
    public void Init(int _timeLimit)
    {
        gm = GameManager.gm;

        password = string.Concat(password.Where(c => !char.IsWhiteSpace(c)));
        password = password.ToLower();

        timerText.text = (_timeLimit / 60) + ":00";

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
            if (timeLeft <= 0)
                timeLeft = 0;

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
        if (timeLeft <= 0)
            timeLeft = 0;
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

        if(timeLeft <= warningPeriod)
        {
            if(timeLeft % 2 == 1)
                timerText.color = Color.black;
            else
                timerText.color = Color.red;
        }
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

    public void OpenPanel()
    {
        password = gm.GetPassword();

        extraTimePanel.SetActive(true);
    }

    public void AddTime()
    {
        string enteredPassword = string.Concat(passwordField.text.Where(c => !char.IsWhiteSpace(c)));
        enteredPassword = enteredPassword.ToLower();
        if (enteredPassword == password)
        {
            int change = System.Int32.Parse(extraTimeField.text);
            timeLeft += (change * 60);
            ShowTimeLeft();
        }

        passwordField.text = "";
        extraTimeField.text = "";

        extraTimePanel.SetActive(false);
    }
}
