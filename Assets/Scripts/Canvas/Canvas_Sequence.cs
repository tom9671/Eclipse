using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public enum eDialogueInstance { introduction, replay, conclusion}

public class Canvas_Sequence : MonoBehaviour
{
    GameManager gm;

    [NamedArray(typeof(eDialogueInstance))] public Canvas_Dialogue[] dialogue;
    public Canvas_Dialogue[] review;
    public Canvas_Dialogue[] hints;

    public TMP_InputField answerInput;

    public string[] acceptableAnswers;

    public bool final;

    Canvas_Dialogue instance;
    QRCodeScanner scanner;
    TouchScreenKeyboard keyboard;

    public TMP_Text ScannerCooldownText;
    float scannerCooldown;

    int hintIdx;
    int sequenceIdx;

    GameObject storyMenu;
    // Start is called before the first frame update
    public void Init(int _sequenceIdx)
    {
        sequenceIdx = _sequenceIdx;
        gm = GameManager.gm;

        for(int i = 0; i < acceptableAnswers.Length; i++)
            acceptableAnswers[i] = acceptableAnswers[i].ToLower();

        UpdateWithPlayerNames();
        ShowDialogueSequence(dialogue[(int)eDialogueInstance.introduction]);
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
        ShowDialogueSequence(review[_idx]);
    }

    public void Hint()
    {
        if(hints.Length != 0)
        {
            if (hintIdx >= hints.Length)
                hintIdx = 0;

            if (hints[hintIdx] != null)
            {
                gm.ChangeTime(-5);
                ShowDialogueSequence(hints[hintIdx]);
            }
            hintIdx++;
            gm.UseHint(sequenceIdx);
        }
    }

    public void Next()
    {

    }

    public void ShowDialogueSequence(Canvas_Dialogue _dialogue)
    {
        instance = Instantiate(_dialogue, Vector3.zero, Quaternion.identity);
        instance.Init(this);
    }

    public void SubmitAnswer()
    {
        string answer = answerInput.text.ToLower();

        if (string.Concat(answer.Where(c => !char.IsWhiteSpace(c))) == "")
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
        if (final)
            EndGame();

        ShowDialogueSequence(dialogue[(int)eDialogueInstance.conclusion]);
    }

    public void Failure()
    {
        ScannerCooldownText.gameObject.SetActive(true);
        scannerCooldown = 5.5f;
        ShowDialogueSequence(gm.tryAgainPrompt);
        gm.IncorrectAnswer(sequenceIdx);
    }

    void EmptyInput()
    {
        ShowDialogueSequence(gm.nullInputPrompt);
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
        gm.SetTeamName(answerInput.text);
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
        ShowDialogueSequence(gm.resultScreen);
    }
}
