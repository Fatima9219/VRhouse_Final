using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//merge to single file as BarcodeScanner
public class ScanColliderPBVi : MonoBehaviour //ScanTrigger // ScanModule
{
    public SteamVR_Action_Vibration hapticAction;
    private Interactable interactable;

    void Start()
    {
        interactable = transform.parent.gameObject.transform.parent.gameObject.GetComponent<Interactable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        LogicStatusRegister.scannedNumber = true;

        if (other.gameObject.CompareTag("BarcodePBVi")) {
            if (other.gameObject.name.Equals("ZP.A.02.3.08")) {
                LogicStatusRegister.correctNumberScanned = true;
            }
            else
                LogicStatusRegister.correctNumberScanned = false;

            GetComponent<AudioSource>().Play();
        }
        //warte 1sek.
        //LogicStatusRegister.scannedNumber = false;
    }


    private void Vibration(float duration, float frequency, float amplitude, SteamVR_Input_Sources source) {
        hapticAction.Execute(0, duration, frequency, amplitude, source);
    }
}