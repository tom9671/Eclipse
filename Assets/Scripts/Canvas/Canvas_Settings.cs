using UnityEngine;
using TMPro;

public class Canvas_Settings : MonoBehaviour
{
    GameManager gm;

    int minutesTotal;

    public TMP_Text minutesText;
    public TMP_InputField timePasswordField;

    public void Init()
    {
        gameObject.SetActive(true);

        gm = GameManager.gm;

        minutesTotal = gm.totalTimeInMinutes;
        minutesText.text = "" + minutesTotal;
    }

    public void ChangeMinutes(int _difference)
    {
        minutesTotal += _difference;
        minutesTotal = Mathf.Clamp(minutesTotal, 5, 100);
        minutesText.text = "" + minutesTotal;
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetString("TimePassword", timePasswordField.text);
        PlayerPrefs.SetInt("TotalTimeMinutes", minutesTotal);
        gm.totalTimeInMinutes = minutesTotal;
        gm.SetPassword(timePasswordField.text);
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
