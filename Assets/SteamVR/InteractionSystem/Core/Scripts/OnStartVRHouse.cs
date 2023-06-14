using System.Collections;
using UnityEngine;

// Here we start the initialisation
public class OnStartVRHouse : MonoBehaviour
{
    OnStartVRHouse() {
        UserProfile.Init();
    }

    private void Awake() {
        transform.GetChild(int.Parse(ScenesTransitioner.GetParam("Level").ToString()) - 1).gameObject.SetActive(true);  // if starting from menu
        //transform.GetChild(3).gameObject.SetActive(true);  // if not starting from menu
        StartCoroutine(FadeIn());
    }

    private void Start()
    {
        //GetComponent<AudioSource>().Play();
    }

    private IEnumerator FadeIn() {
        SceneFade.View(Color.clear, 5f);
        yield return new WaitForSeconds(5f);
    }
}