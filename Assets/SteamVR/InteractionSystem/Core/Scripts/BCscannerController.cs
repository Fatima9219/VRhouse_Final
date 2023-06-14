using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class BCscannerController : MonoBehaviour
{
    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Action_Boolean scanAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("BCscanner", "Scan");

    private Interactable interactable;
    private GameObject BCscannerLight;

    void Start()
    {
        interactable = GetComponent<Interactable>();
        BCscannerLight = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (interactable.attachedToHand)
            if (scanAction[interactable.attachedToHand.handType].state) 
                BCscannerLight.SetActive(true);            
            else 
                BCscannerLight.SetActive(false);
    }
}