using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public Camera rendCam;
    public RenderTexture rendTex;
    //public GameObject menu;

    private int resX = 1280;
    private int resY = 720;

    private SoundManager snd;

    // Start is called before the first frame update
    void Start()
    {
        snd = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        snd.Play("Music");

        Screen.SetResolution(resX, resY, Screen.fullScreen);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            CamCapture();
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("Game");
    }

    public void Options()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CentreMenu()
    {
        cam.transform.position = new Vector3(0f, 0f, -10f);
    }

    public void OpenMusicLink()
    {
        Application.OpenURL("https://zitronsound.bandcamp.com/track/wild-west");
    }
    public void OpenMusicLink2()
    {
        Application.OpenURL("https://zitronsound.bandcamp.com/track/lonely-cowboy");
    }

    public void OpenLogoLink()
    {
        Application.OpenURL("https://imgur.com/gallery/aLMKYA0");
    }

    public void OpenSFXLink()
    {
        Application.OpenURL("https://www.fesliyanstudios.com");
    }

    public void Credits()
    {
        cam.transform.position = new Vector3(0f, 15f, -10f);
    }

    public void Set1280()
    {
        resX = 1280;
        resY = 720;
        Screen.SetResolution(resX, resY, false);
    }
    public void Set1920()
    {
        resX = 1920;
        resY = 1080;
        Screen.SetResolution(resX, resY, false);
    }

    void CamCapture()
    {
        Camera Cam = rendCam;

        RenderTexture currentRT = rendTex;
        RenderTexture.active = Cam.targetTexture;

        Cam.Render();

        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        File.WriteAllBytes(Application.dataPath + "/Background.png", Bytes);
    }

    public void Tutorial()
    {
        cam.transform.position = new Vector3(0f, -15f, -10f);
    }

    public void Tutorial2()
    {
        cam.transform.position = new Vector3(0f, -30f, -10f);
    }

    public void Tutorial3()
    {
        cam.transform.position = new Vector3(0f, -45f, -10f);
    }

    public void Controls()
    {
        cam.transform.position = new Vector3(0f, -60f, -10f);
    }
}
