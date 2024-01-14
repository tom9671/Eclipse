using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Widget_VoiceOver : MonoBehaviour
{
    AudioSource source;

    float progress;
    bool paused;

    Canvas_Dialogue dialogue;

    public Slider progressSlider;

    public GameObject[] pauseGraphics = new GameObject[2];

    public void Init(AudioClip _clip, Canvas_Dialogue _dialogue)
    {
        transform.localScale = Vector3.one;

        source = GetComponent<AudioSource>();
        source.clip = _clip;

        paused = false;
        source.time = 0;
        progressSlider.maxValue = _clip.length;
        progressSlider.value = 0;
        progress = 0;
        UpdateGraphics();

        source.Stop();
        source.Play();

        dialogue = _dialogue;
    }

    void FixedUpdate()
    {
        progressSlider.value = source.time;
    }

    void Update()
    {
        if (!paused)
        {
            progress += Time.deltaTime;
            if(progress >= source.clip.length)
            {
                paused = true;
                EndAudio();
                UpdateGraphics();
            }
        }
    }

    public void PauseToggle()
    {
        if(progress >= (source.clip.length - 0.1f) || source.time == 0)
        {
            source.time = 0;
            progress = 0;
            paused = true;
        }

        paused = !paused;
        if (paused)
            source.Pause();
        else
            source.Play();

        UpdateGraphics();
    }

    public void SkipRewind(int _difference)
    {
        float newTime = source.time + _difference;
        newTime = Mathf.Clamp(newTime, 0, source.clip.length - 0.1f);
        if (newTime == source.clip.length)
            EndAudio();
        progress = newTime;
        source.time = newTime;
    }

    void EndAudio()
    {
        dialogue.EndAudio();
    }

    public void StopAudio()
    {
        source.Stop();
    }

    void UpdateGraphics()
    {
        pauseGraphics[0].SetActive(!paused);
        pauseGraphics[1].SetActive(paused);
    }
}
