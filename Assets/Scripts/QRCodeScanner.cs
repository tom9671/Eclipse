using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImageBackground;
    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;
    [SerializeField]
    private RectTransform _scanZone;
    [SerializeField]
    private TMP_Text outputText;

    private bool _isCamAvailable;
    private WebCamTexture _cameraTexture;

    string[] acceptableAnswers;

    Canvas_Sequence sequence;

    public Sprite[] answerSprites;
    public Image displayImage;

    // Start is called before the first frame update
    public void Init(string[] _correctAnswer, Canvas_Sequence _sequence)
    {
        acceptableAnswers = _correctAnswer;
        sequence = _sequence;

        SetUpCamera();
    }

    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRender();
    }

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            _isCamAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false || i == 0)
            {
                _cameraTexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
                Debug.Log(devices[i].name);
            }
        }

        _cameraTexture.Play();
        _rawImageBackground.texture = _cameraTexture;
        _isCamAvailable = true;
    }

    private void UpdateCameraRender()
    {
        if (!_isCamAvailable)
        {
            return;
        }
        float ratio = (float)_cameraTexture.width / (float)_cameraTexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = -_cameraTexture.videoRotationAngle;
        _rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    public void OnClickScan()
    {
        Scan();
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width, _cameraTexture.height);
            if(result != null)
            {
                outputText.text = result.Text;

                int x = 0;
                if (int.TryParse(outputText.text, out x))
                {
                    if (x < answerSprites.Length)
                    {
                        displayImage.gameObject.SetActive(true);
                        displayImage.sprite = answerSprites[x];
                        outputText.text = "Is this what you're looking for?";
                    }
                    else
                    {
                        outputText.text = "Index error, report as bug!";
                    }
                }
                else
                {
                    outputText.text = "Value error, report as bug!";
                }
            }
            else
            {
                outputText.text = "Unable to read QR code";
            }
        }
        catch
        {
            outputText.text = "Camera not found";
        }
    }

    public void EndScan()
    {
        gameObject.SetActive(false);
    }
}
