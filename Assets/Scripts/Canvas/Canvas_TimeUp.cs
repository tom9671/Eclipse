using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas_TimeUp : MonoBehaviour
{
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.gm;
    }

    public void Restart()
    {
        gm.ResetGame();
    }
}
