using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//merge to single file as BarcodeScanner
public class ScanCollider : MonoBehaviour //ScanTrigger // ScanModule
{
    public SteamVR_Action_Vibration hapticAction;
    private Interactable interactable;

    void Start()
    {
        interactable = transform.parent.gameObject.transform.parent.gameObject.GetComponent<Interactable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Barcode")) {
            switch (other.gameObject.name) {
                case "BarcodeUser":
                    LogicStatusRegister.userIsLoggedInAsPicker = true;
                    break;

                case "BarcodeBasket1":
                    LogicStatusRegister.lastScanUnit = other.gameObject.name;
                    break;

                case "BarcodeBasket2":
                    LogicStatusRegister.lastScanUnit = other.gameObject.name;
                    break;

                case "BarcodeBasket3":
                    LogicStatusRegister.lastScanUnit = other.gameObject.name;
                    break;

                case "BarcodeBasket4":
                    LogicStatusRegister.lastScanUnit = other.gameObject.name;
                    break;

                case "BarcodeBasket5":
                    LogicStatusRegister.lastScanUnit = other.gameObject.name;
                    break;

                case "BarcodeBasket6":
                    LogicStatusRegister.lastScanUnit = other.gameObject.name;
                    break;

                case "ProductBarcode":
                    LogicStatusRegister.BarcodeCaptured = true;
                    LogicStatusRegister.lastScanUnit = other.gameObject.transform.parent.name;
                    break;
            }
            GetComponent<AudioSource>().Play();
            BlinkBC_LED();
            Vibration(0.1f, 10, 75, interactable.attachedToHand.handType);
        }
    }

    private void BlinkBC_LED() {
        /// TODO BC display to green!
    }

    private void Vibration(float duration, float frequency, float amplitude, SteamVR_Input_Sources source) {
        hapticAction.Execute(0, duration, frequency, amplitude, source);
    }
}