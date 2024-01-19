using UnityEngine;
using UnityEngine.UI;

public class Canvas_Stories : MonoBehaviour
{
    public GameObject closeButton;
    public Canvas_Dialogue[] stories;
    bool[] read;

    public Image[] checkGraphics;

    Canvas_Sequence sequence;

    bool closedFirst;
    // Start is called before the first frame update
    public void Init(Canvas_Sequence _sequence)
    {
        sequence = _sequence;

        read = new bool[stories.Length];

        closedFirst = true;
    }

    public void OpenStory(int _storyIdx)
    {
        read[_storyIdx] = true;
        Canvas_Dialogue instance = Instantiate(stories[_storyIdx], Vector3.zero, Quaternion.identity);
        instance.Init(null);

        bool allRead = true;
        for(int i = 0; i < read.Length; i++)
        {
            checkGraphics[i].gameObject.SetActive(read[i]);

            if (!read[i])
                allRead = false;
        }

        closeButton.SetActive(allRead);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if(closedFirst)
        {
            sequence.ShowDialogueSequence(sequence.dialogue[(int)eDialogueInstance.replay], true);
            closedFirst = false;
        }
    }
}
