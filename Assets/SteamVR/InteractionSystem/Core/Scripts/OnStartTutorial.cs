using System.Collections;
using UnityEngine;

public class OnStartTutorial : MonoBehaviour
{
    private void Awake() {
        StartCoroutine(FadeIn());
    }

    private void Start()
    {}

    private IEnumerator FadeIn() {
        SceneFade.View(Color.clear, 5f);
        yield return new WaitForSeconds(5f);
    }
}