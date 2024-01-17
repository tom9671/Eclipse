using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{

    static int intCurrentSlide = 1;

    public Sprite sprBackgroundImageRight, sprBackgroundImageWrong,
        sprBackgroundImageHint, sprBackgroundImageGeneral, sprBackgroundImageInfo, 
        sprBackgroundImageName, sprBackgroundImagePopup, sprBackgroundImageStart;

    public GameObject goBackground, goConentImage1;
    Image imgCurrentBack, imgCurrentContent1;
    

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBackgroundNext()
    {
        imgCurrentBack = goBackground.GetComponent<Image>();
        imgCurrentContent1 = goBackground.GetComponent<Image>();
        //imgCurrentImage.sprite = sprBackgroundImageGeneral;

        if (intCurrentSlide == 1 || intCurrentSlide == 2)
        {
            imgCurrentBack.sprite = sprBackgroundImageInfo;
            intCurrentSlide++;
        }
        else
        {
            imgCurrentBack.sprite = sprBackgroundImageGeneral;
        }
    }

    public void ChangeBackgroundPopUp()
    {

    }
}
