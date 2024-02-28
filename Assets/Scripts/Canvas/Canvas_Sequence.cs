using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Linq;

public enum eDialogueInstance { introduction, replay, conclusion}

public class Canvas_Sequence : MonoBehaviour
{
    GameManager gm;

    [NamedArray(typeof(eDialogueInstance))] public Canvas_Dialogue[] dialogue;
    public Canvas_Dialogue[] review;
    public Canvas_Dialogue[] hints;

    public TMP_InputField[] answerInput;

    public string[] acceptableAnswers;

    public bool final;

    Canvas_Dialogue instance;
    QRCodeScanner scanner;
    TouchScreenKeyboard keyboard;

    public TMP_Text ScannerCooldownText;
    float scannerCooldown;

    int hintIdx;
    int sequenceIdx;
    int startTime;

    GameObject storyMenu;

    public UnityEvent onSuccessfulAnswer;
    // Start is called before the first frame update
    public void Init(int _sequenceIdx)
    {
        sequenceIdx = _sequenceIdx;
        gm = GameManager.gm;

        startTime = gm.timerReal.GetTimeInSeconds();

        for (int i = 0; i < acceptableAnswers.Length; i++)
        {
            acceptableAnswers[i] = acceptableAnswers[i].ToLower();
            acceptableAnswers[i] = string.Concat(acceptableAnswers[i].Where(c => !char.IsWhiteSpace(c)));
        }

        UpdateWithPlayerNames();
        ShowDialogueSequence(dialogue[(int)eDialogueInstance.introduction], true);
    }

    void FixedUpdate()
    {
        if(scannerCooldown > 0.5f)
        {
            scannerCooldown -= Time.fixedDeltaTime;
            ScannerCooldownText.text = scannerCooldown.ToString("F0");

            if (scannerCooldown <= 0.5f)
                ScannerCooldownText.gameObject.SetActive(false);
        }
    }

    void UpdateWithPlayerNames()
    {
        TMP_Text[] textObjects = GetComponentsInChildren<TMP_Text>();

        for(int i = 0; i < textObjects.Length; i++)
        {
            string tempString = textObjects[i].text;
            textObjects[i].text = "";
            for (int j = 0; j < tempString.Length; j++)
            {
                string character = tempString.Substring(j, 1);
                if (character == "@")
                    character = gm.GetTeamName();
                textObjects[i].text += character;
            }
        }
    }

    public void OpenCamera()
    {
        if (scannerCooldown > 0.5f)
            return;

        if(scanner == null)
        {
            scanner = Instantiate(gm.scannerObject, Vector3.zero, Quaternion.identity);
            scanner.Init(acceptableAnswers, this);
        }
        else
        {
            scanner.gameObject.SetActive(true);
            //scanner.Init(acceptableAnswers, this);
        }
    }

    public void Review(int _idx)
    {
        gm.timerReal.GetComponent<Canvas>().sortingOrder = 0;
        ShowDialogueSequence(review[_idx], false);
    }

    public void Hint()
    {
        if(hints.Length != 0)
        {
            gm.timerReal.GetComponent<Canvas>().sortingOrder = 0;
            if (hintIdx >= hints.Length)
                hintIdx = 0;

            if (hints[hintIdx] != null)
            {
                gm.ChangeTime(-5);
                ShowDialogueSequence(hints[hintIdx], true);
            }
            hintIdx++;
            gm.UseHint(sequenceIdx);
        }
    }

    public void Next()
    {

    }

    public void ShowDialogueSequence(Canvas_Dialogue _dialogue, bool autoPlay)
    {
        instance = Instantiate(_dialogue, Vector3.zero, Quaternion.identity);
        instance.Init(this);
        if (!autoPlay)
            instance.TogglePlay();
    }

    public void SubmitAnswer()
    {
        string answer = "";
        for (int i = 0; i < answerInput.Length; i++)
            answer += answerInput[i].text.ToLower();
        answer = string.Concat(answer.Where(c => !char.IsWhiteSpace(c)));

        if (answer == "")
        {
            EmptyInput();
        }
        else
        {
            if (acceptableAnswers.Length > 0)
            {
                bool correct = false;
                for (int i = 0; i < acceptableAnswers.Length; i++)
                {
                    if (answer.Contains(acceptableAnswers[i]))
                        correct = true;
                }

                if (correct)
                    Success();
                else
                    Failure();
            }
            else
            {
                Success();
            }
        }
    }

    public void Success()
    {
        onSuccessfulAnswer.Invoke();

        int timeTaken = gm.timerReal.GetTimeInSeconds() - startTime;
        gm.RecordTime(sequenceIdx, timeTaken);

        if (final)
            EndGame();

        ShowDialogueSequence(dialogue[(int)eDialogueInstance.conclusion], true);
    }

    public void Failure()
    {
        ScannerCooldownText.gameObject.SetActive(true);
        scannerCooldown = 5.5f;
        ShowDialogueSequence(gm.tryAgainPrompt, true);
        gm.IncorrectAnswer(sequenceIdx);
    }

    void EmptyInput()
    {
        ShowDialogueSequence(gm.nullInputPrompt, true);
        gm.IncorrectAnswer(sequenceIdx);
    }

    public void ChangeSequence()
    {
        gm.StartChallenge(sequenceIdx + 1);
        if (scanner != null)
            Destroy(scanner.gameObject);
        Destroy(gameObject);
    }

    public void OpenStoryMenu()
    {
        if (storyMenu == null)
        {
            storyMenu = Instantiate(gm.storyMenu.gameObject, Vector3.zero, Quaternion.identity);
            storyMenu.SendMessage("Init", this);
        }
        else
            storyMenu.SetActive(true);
    }

    public void UpdateTeamName()
    {
        string answer = "";
        for (int i = 0; i < answerInput.Length; i++)
            answer += answerInput[i].text;
            gm.SetTeamName(answer);
    }

    public void OpenMobileKeyboard(bool _justNumbers)
    {
        Debug.Log("Opening Keyboard");

        if (_justNumbers)
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumberPad);
        else
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    void EndGame()
    {
        gm.EndGame();
    }

    public void OpenResults()
    {
        ShowDialogueSequence(gm.resultScreen, true);
    }
}
