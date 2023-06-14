using UnityEngine;

public class LaempchenBlink : MonoBehaviour
{
    private Renderer rend;
    public Material _mat;

    public float value = 0f;

    public bool on = true;

    void Start() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        BulbBlink();
    }
    
    private void BulbBlink() {
        if (Time.fixedTime % 0.5 < .2) {
            _mat.SetFloat("_EmissiveExposureWeight", 0);
            _mat.SetFloat("_EmissiveIntensity", 0);
        }
        else {
            _mat.SetFloat("_EmissiveExposureWeight", 1);
            _mat.SetFloat("_EmissiveIntensity", 10000);
        }
    }
}