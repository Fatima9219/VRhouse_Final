using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonTransitioner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    public SteamVR_Action_Vibration hapticAction;

    public AudioClip    onHoverClip,
                        onDownClip;

    private Color32     m_NormalColor = new Color(175 / 255f, 175f / 255f, 175f / 255f),
                        m_HoverColor  = new Color(255f, 255f, 255f);

    private Image       m_Image = null;

    public GameObject   setActivePanel = null, 
                        setInactivePanel = null;

    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<AudioSource>().PlayOneShot(onHoverClip);
        Vibration(0.1f, 10, 75, SteamVR_Input_Sources.RightHand);
        m_Image.color = m_HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Image.color = m_NormalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(PlayAudioClipWithDelay());

        switch (eventData.pointerEnter.transform.name)
        {
            case "LoadMenuBtn":
                LoadScene(1);
                break;

            case "StartGame":
                LoadScene(2);
                break;

            case "QuitGameBtn":
                Application.Quit();
                break;

            case "MenuBtn":
                //ScenesTransitioner.Load("Menu", "Results", "false");
                break;

            case "RestartLevelBtn":
                LoadScene(2);
                break;

            case "ReplayBtn":
                StartCoroutine(FadeOut(5, "2"));
                break;

            case "StartLevel1":
                StartCoroutine(FadeOut(5, "1"));
                break;

            case "StartLevel2":
                StartCoroutine(FadeOut(5, "2"));
                break;

            case "StartLevel3":
                StartCoroutine(FadeOut(5, "3"));
                break;

            case "StartLevel4":
                StartCoroutine(FadeOut(5, "4"));
                break;

            case "TutorialBtn":
                LoadScene(3);
                break;

            case "ContinueBtn":
                break;
        }

        m_Image.color = m_NormalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_Image.color = m_HoverColor;
    }

    private void LoadScene(int lv)
    {
        StartCoroutine(FadeToNextLevel(lv));
    }
    
    private IEnumerator FadeToNextLevel(int lv)
    {
        SceneFade.View(Color.black, 2f);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(lv);
    }

    private IEnumerator FadeOut(float time, string level)
    {
        SceneFade.View(Color.black, time);
        yield return new WaitForSeconds(time);
        SceneFade.View(Color.white, .0001f);
        ScenesTransitioner.Load("VRhouse", "Level", level);
    }

    private IEnumerator PlayAudioClipWithDelay()
    {
        GetComponent<AudioSource>().PlayOneShot(onDownClip);
        yield return new WaitForSeconds(.2f);
        if (setActivePanel != null && setInactivePanel != null) {
            setActivePanel.SetActive(true);
            setInactivePanel.SetActive(false);
        }
    }

    private void Vibration(float duration, float frequency, float amplitude, SteamVR_Input_Sources source) {
        hapticAction.Execute(0, duration, frequency, amplitude, source);
    }
}