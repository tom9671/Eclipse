using UnityEngine;
using TMPro;

public class Canvas_Settings : MonoBehaviour
{
    GameManager gm;

    int minutesTotal;

    public TMP_Text minutesText;

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
        PlayerPrefs.SetInt("TotalTimeMinutes", minutesTotal);
        gm.totalTimeInMinutes = minutesTotal;
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
