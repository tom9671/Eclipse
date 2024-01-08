using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Widget_Video : MonoBehaviour
{
    VideoPlayer player;
    bool paused;

    Canvas_Dialogue dialogue;

    public Slider progressSlider;

    public void Init(VideoClip _clip, Canvas_Dialogue _dialogue)
    {
        player = GetComponent<VideoPlayer>();
        player.clip = _clip;
        player.Stop();
        player.Play();

        dialogue = _dialogue;

        player.loopPointReached += EndVideo;

        progressSlider.maxValue = (float)_clip.length;
        progressSlider.value = 0;
    }

    void FixedUpdate()
    {
        progressSlider.value = (float)player.time;
    }

    public void PauseToggle()
    {
        paused = !paused;
        if (paused)
            player.Pause();
        else
            player.Play();
    }

    public void SkipRewind(int _difference)
    {
        float newTime = (float)player.time + _difference;
        newTime = Mathf.Clamp(newTime, 0, (float)player.clip.length);
        player.time = newTime;
    }

    void EndVideo(VideoPlayer _player)
    {
        dialogue.EndVideo();
    }
}
