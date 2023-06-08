using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnResult : MonoBehaviour
{
    //============================================
    private Image soundImage;
    private bool sound;
    private Sprite onSprite;
    private Sprite offSprite;    
    //============================================
    private void Awake()
    {
        soundImage = this.GetComponent<Image>();
        sound = true;
        onSprite = soundImage.sprite;
        offSprite = Resources.Load<Sprite>("Stage/Sound_Off");
    }
    //============================================
    public void Play()
    {
        UIObjects.Instance.optionPanel.gameObject.SetActive(false);
    }    
    public void Sound()
    {
        sound = !sound;

        if (sound)
            soundImage.sprite = onSprite;
        else
            soundImage.sprite = offSprite;

        //> sound on / off 설정.
    }

    public void Lobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void Restart()
    {
        SceneManager.LoadScene("Stage");
    }

    public void Quit()
    {
        Application.Quit();
    }
    //============================================
}
