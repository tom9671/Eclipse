using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Video;
using TMPro;

public enum eContentType { none, image, video, Object }
public enum eMaxSize { width, height }
public enum eAlignment { right, left, center }
public enum eWaitFor { none, text, video, audio }

[System.Serializable]
public class ImageParams
{
    public Sprite image;
    public eAlignment alignContent;
    public float yOffset;
}

[System.Serializable]
public class DialogueParams
{
    public string dialogue;
    public eAlignment alignContent;
}

[System.Serializable]
public class DialogueEntry
{
    public string subtitle;
    public DialogueParams[] dialogue;
    public AudioClip dialogueClip;
    public eContentType content;

    public ImageParams[] displayImage;
    public eMaxSize limitSizeOf;
    public float dimensionLimit = 300;

    public VideoClip video;

    public GameObject Object;
    public eWaitFor waitFor;
}

public class Canvas_Dialogue : MonoBehaviour
{
    [Tooltip("Every piece of dialogue intended to appear in the same instance of this canvas")]
    public DialogueEntry[] dialogueSequence;
    [Tooltip("Reference to the text object used to display the message")]
    public TMP_Text subtitleText;
    public TMP_Text[] displayText;
    public Image[] displayImageUI;

    public Transform audioHolder;
    public Transform videoHolder;

    [Range(1, 10)]
    [Tooltip("The speed at which characters appear in the output, expressed as '(1.1 - (x/10)) / 16'")]
    public float textSpeed = 5;

    public Button nextButton;
    Vector3 nextPos;

    public UnityEvent onEndDialogue;

    public Transform graphicContent;
    Vector3 graphicPos;
    Vector3[] dialoguePos;
    Vector2[] dialogueSize;

    int messageIdx;
    int messageSubIdx;
    int characterIdx;

    bool writing;

    Canvas_Sequence sequence;
    Widget_Video videoPlayer;
    Widget_VoiceOver audioPlayer;

    GameManager gm;
    // Start is called before the first frame update
    public void Init(Canvas_Sequence _sequence)
    {
        sequence = _sequence;
        gm = GameManager.gm;

        dialoguePos = new Vector3[2];
        dialogueSize = new Vector2[2];
        graphicPos = graphicContent.localPosition;
        for (int i = 0; i < 2; i++)
        {
            dialoguePos[i] = displayText[i].transform.localPosition;
            dialogueSize[i] = displayText[i].GetComponent<RectTransform>().sizeDelta;
        }

        UpdateWithPlayerNames();
        StartWriting();
    }

    void UpdateWithPlayerNames()
    {
        TMP_Text[] textObjects = GetComponentsInChildren<TMP_Text>();

        for (int i = 0; i < textObjects.Length; i++)
        {
            string tempString = textObjects[i].text;
            textObjects[i].text = "";
            for (int j = 0; j < tempString.Length; j++)
            {
                string character = tempString.Substring(j, 1);
                if (character == "@")
                    character = gm.GetTeamName();
                else if (character == "$")
                    character = gm.GetTimeLeft();
                textObjects[i].text += character;
            }
        }

        for (int i = 0; i < dialogueSequence.Length; i++)
        {
            for (int j = 0; j < dialogueSequence[i].dialogue.Length; j++)
            {
                string tempString = dialogueSequence[i].dialogue[j].dialogue;
                dialogueSequence[i].dialogue[j].dialogue = "";
                for (int k = 0; k < tempString.Length; k++)
                {
                    string character = tempString.Substring(k, 1);
                    if (character == "@")
                        character = gm.GetTeamName();
                    dialogueSequence[i].dialogue[j].dialogue += character;
                }
            }

            string tempString1 = dialogueSequence[i].subtitle;
            dialogueSequence[i].subtitle = "";
            for (int k = 0; k < tempString1.Length; k++)
            {
                string character = tempString1.Substring(k, 1);
                if (character == "@")
                    character = gm.GetTeamName();
                dialogueSequence[i].subtitle += character;
            }
        }
    }

    void StartWriting()
    {
        subtitleText.text = dialogueSequence[messageIdx].subtitle;
        if (dialogueSequence[messageIdx].waitFor != eWaitFor.none)
            nextButton.gameObject.SetActive(false);

        if (dialogueSequence[messageIdx].dialogueClip != null)
        {
            /*
            for (int i = 0; i < 2; i++)
                displayText[i].enabled = false;
            */
            if (audioPlayer == null)
            {
                audioPlayer = Instantiate(gm.audioPlayer, audioHolder.position, audioHolder.rotation);
                audioPlayer.transform.parent = audioHolder;
            }

            audioPlayer.Init(dialogueSequence[messageIdx].dialogueClip, this);
        }
        /*
        else
        {*/
            for (int i = 0; i < 2; i++)
                displayText[i].enabled = (i < dialogueSequence[messageIdx].dialogue.Length && dialogueSequence[messageIdx].dialogue[i].dialogue != "");
            DisplayContent();
            for (int i = 0; i < 2; i++)
                displayText[i].text = "";
            writing = true;
            characterIdx = 0;
            messageSubIdx = 0;
            StartCoroutine(WriteText());
        //}
    }

    IEnumerator WriteText()
    {
        float interval = (1.1f - (textSpeed / 10)) / 16;

        bool characterIdxParam = characterIdx < dialogueSequence[messageIdx].dialogue[messageSubIdx].dialogue.Length;
        bool canWrite = (dialogueSequence[messageIdx].dialogue[messageSubIdx].dialogue != "" && writing && characterIdxParam);
        //Debug.Log(canWrite + " " + messageIdx + " " + messageSubIdx);
        if (canWrite)
        {
            string character = dialogueSequence[messageIdx].dialogue[messageSubIdx].dialogue.Substring(characterIdx, 1);

            displayText[messageSubIdx].text += character;
            if (character == " ")
                interval = 0;
        }

        yield return new WaitForSeconds(interval);

        characterIdx++;
        if (characterIdx >= dialogueSequence[messageIdx].dialogue[messageSubIdx].dialogue.Length)
        {
            if ((messageSubIdx + 1) >= dialogueSequence[messageIdx].dialogue.Length)
                StopWriting();
            else
            {
                messageSubIdx++;
                characterIdx = 0;
                StartCoroutine(WriteText());
            }
        }
        else
        {
            StartCoroutine(WriteText());
        }
    }

    public void Continue()
    {
        if (writing)
            StopWriting();
        else
        {
            messageIdx++;
            if (messageIdx >= dialogueSequence.Length)
            {
                EndDialogue();
            }
            else
            {
                StartWriting();
            }
        }
    }

    void DisplayContent()
    {
        //Reset graphic content before setup
        for (int i = 0; i < 2; i++)
        {
            displayImageUI[i].GetComponent<RectTransform>().sizeDelta = Vector2.zero;

            displayText[i].GetComponent<RectTransform>().sizeDelta = dialogueSize[i];
            if (i < dialogueSequence[messageIdx].dialogue.Length)
            {
                if (dialogueSequence[messageIdx].dialogue[i].alignContent == eAlignment.left)
                    displayText[i].horizontalAlignment = HorizontalAlignmentOptions.Left;
                else if (dialogueSequence[messageIdx].dialogue[i].alignContent == eAlignment.center)
                {
                    displayText[i].horizontalAlignment = HorizontalAlignmentOptions.Center;
                    displayText[i].GetComponent<RectTransform>().sizeDelta = new Vector2(dialogueSize[i].x * 2, dialogueSize[i].y);
                }
                else
                    displayText[i].horizontalAlignment = HorizontalAlignmentOptions.Right;
            }
        }
        if (videoPlayer != null)
            videoPlayer.gameObject.SetActive(false);

        //Setup content based on dialogueSequence
        switch (dialogueSequence[messageIdx].content)
        {
            case eContentType.image:
                for (int i = 0; i < 2; i++)
                {
                    if (i < dialogueSequence[messageIdx].displayImage.Length && dialogueSequence[messageIdx].displayImage[i].image != null)
                    {
                        Vector2 imageDimensions = Vector2.zero;
                        float width = dialogueSequence[messageIdx].displayImage[i].image.bounds.size.x;
                        float height = dialogueSequence[messageIdx].displayImage[i].image.bounds.size.y;
                        if (dialogueSequence[messageIdx].limitSizeOf == eMaxSize.width)
                            imageDimensions = new Vector2(dialogueSequence[messageIdx].dimensionLimit, (height / width) * dialogueSequence[messageIdx].dimensionLimit);
                        else
                            imageDimensions = new Vector2((width / height) * dialogueSequence[messageIdx].dimensionLimit, dialogueSequence[messageIdx].dimensionLimit);

                        displayImageUI[i].GetComponent<RectTransform>().sizeDelta = imageDimensions;
                        displayImageUI[i].sprite = dialogueSequence[messageIdx].displayImage[i].image;
                    }
                }
                break;
            case eContentType.video:
                if (dialogueSequence[messageIdx].video != null)
                {
                    if (videoPlayer == null)
                    {
                        videoPlayer = Instantiate(gm.videoPlayer, videoHolder.position, videoHolder.rotation);
                        videoPlayer.transform.parent = videoHolder;
                        videoPlayer.transform.localScale = Vector3.one * 1.05f;
                    }
                    else
                        videoPlayer.gameObject.SetActive(true);

                    videoPlayer.Init(dialogueSequence[messageIdx].video, this);
                }
                break;
        }

        //Change orientation of content depending on dialogueSequence
        for (int i = 0; i < 2; i++)
        {
            if (i < dialogueSequence[messageIdx].displayImage.Length && dialogueSequence[messageIdx].displayImage != null)
            {
                switch (dialogueSequence[messageIdx].displayImage[i].alignContent)
                {
                    case eAlignment.right:
                        displayImageUI[i].transform.localPosition = Vector3.up * dialogueSequence[messageIdx].displayImage[i].yOffset;
                        break;
                    case eAlignment.left:
                        displayImageUI[i].transform.localPosition = new Vector2(-graphicPos.x * 2, 0) + Vector2.up * dialogueSequence[messageIdx].displayImage[i].yOffset;
                        break;
                    case eAlignment.center:
                        displayImageUI[i].transform.localPosition = new Vector2(-graphicPos.x, 0) + Vector2.up * dialogueSequence[messageIdx].displayImage[i].yOffset;
                        break;
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            if (i < dialogueSequence[messageIdx].dialogue.Length)
            {
                switch (dialogueSequence[messageIdx].dialogue[i].alignContent)
                {
                    case eAlignment.right:
                        displayText[i].transform.localPosition = new Vector2(-dialoguePos[i].x, dialoguePos[i].y);
                        break;
                    case eAlignment.left:
                        displayText[i].transform.localPosition = dialoguePos[i];
                        break;
                    case eAlignment.center:
                        displayText[i].transform.localPosition = new Vector2(0, dialoguePos[i].y);
                        break;
                }
            }
        }
    }

    public void StopWriting()
    {
        writing = false;
        for (int i = 0; i < 2; i++)
        {
            if(i < dialogueSequence[messageIdx].dialogue.Length)
                displayText[i].text = dialogueSequence[messageIdx].dialogue[i].dialogue;
        }
        characterIdx = dialogueSequence[messageIdx].dialogue[messageSubIdx].dialogue.Length;
        if (dialogueSequence[messageIdx].waitFor == eWaitFor.text)
            EnableNext();
    }

    void EndDialogue()
    {
        onEndDialogue.Invoke();
        Dissappear();
    }

    public void EndVideo()
    {
        if (dialogueSequence[messageIdx].waitFor == eWaitFor.video)
            EnableNext();
    }

    public void EndAudio()
    {
        if (dialogueSequence[messageIdx].waitFor == eWaitFor.audio)
            EnableNext();
    }

    public void Dissappear()
    {
        Destroy(gameObject);
    }

    public void ChangeSequence()
    {
        sequence.ChangeSequence();
    }

    public void StartTimer()
    {
        gm.StartTimer();
    }

    public void OpenStoryMenu()
    {
        sequence.OpenStoryMenu();
    }

    public void OpenResults()
    {
        sequence.OpenResults();
    }

    public void ResetGame()
    {
        gm.ResetGame();
    }

    public void EnableNext()
    {
        nextPos = nextButton.transform.localPosition;
        nextButton.gameObject.SetActive(true);
        Transform nextParent = nextButton.transform.parent;
        nextButton.transform.parent = null;
        nextButton.transform.parent = nextParent;
        nextButton.transform.localPosition = nextPos;
    }
}
