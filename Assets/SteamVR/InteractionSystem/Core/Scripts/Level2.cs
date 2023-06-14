/*
    Pick-By-Light Logic Level Description
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Level2 : MonoBehaviour
{
    private GameObject Skillometer, Bunit, Tunit, Punit, Dunit;
    private Text SkillometerTime;

    private PickerCartTechnics _PickerCartTechnics;

    private GameObject PickByVision, PickByVoice;

    private void Awake()
    {
        PickByVision = GameObject.Find("PickByVision").transform.gameObject;
        PickByVision.SetActive(false);

        PickByVoice = GameObject.Find("PickByVoice").transform.gameObject;
        PickByVoice.SetActive(false);

        Skillometer = GameObject.Find("Skillometer");
        ResetAllValues();
        LogicStatusRegister.ResetAllValues();
        CommissioningTime.ResetAllValues();
    }

    void Start()
    {
        // Find, activate and init the Skillometer
        
        //Skillometer.SetActive(true);
        SkillometerTime = Skillometer.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        Bunit = Skillometer.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        Tunit = Skillometer.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        Punit = Skillometer.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject;
        Dunit = Skillometer.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject;
       
        _PickerCartTechnics = gameObject.AddComponent<PickerCartTechnics>();
    }

    private void ResetAllValues() {
        partOneIsDone = partTwoIsDone = partThreeIsDone = false;
    }

    private bool    partOneIsDone   = false,   // Finish PickingCart initialization
                    partTwoIsDone   = false,   // Finish PickingSequence
                    partThreeIsDone = false;   // Return to Terminal

    private void Update()
    {   // this have to be rewritten
        if (CommissioningTime.basisTimeIsRunning) { 
            CommissioningTime.Counter(CommissioningTime.Unit.basis);
            Bunit.SetActive(true);
            Tunit.SetActive(false);
            Punit.SetActive(false);
            Dunit.SetActive(false);
            SkillometerTime.text = TimeSpan.FromSeconds(CommissioningTime.GetBasetime()).ToString("mm\\:ss");
        }

        if (CommissioningTime.travelTimeIsRunning) {
            CommissioningTime.Counter(CommissioningTime.Unit.travel);
            Bunit.SetActive(false);
            Tunit.SetActive(true);
            Punit.SetActive(false);
            Dunit.SetActive(false);
            SkillometerTime.text = TimeSpan.FromSeconds(CommissioningTime.GetTraveltime()).ToString("mm\\:ss");
        }

        if (CommissioningTime.pickingTimeIsRunning) {
            CommissioningTime.Counter(CommissioningTime.Unit.picking);
            Bunit.SetActive(false);
            Tunit.SetActive(false);
            Punit.SetActive(true);
            Dunit.SetActive(false);
            SkillometerTime.text = TimeSpan.FromSeconds(CommissioningTime.GetPickingtime()).ToString("mm\\:ss");
        }

        if (!CommissioningTime.basisTimeIsRunning && !CommissioningTime.travelTimeIsRunning && !CommissioningTime.pickingTimeIsRunning && CommissioningTime.sessionIsRunning) {
            CommissioningTime.Counter(CommissioningTime.Unit.dead);
            Bunit.SetActive(false);
            Tunit.SetActive(false);
            Punit.SetActive(false);
            Dunit.SetActive(true);
            SkillometerTime.text = TimeSpan.FromSeconds(CommissioningTime.GetDeadtime()).ToString("mm\\:ss");
        }

        if (!CommissioningTime.sessionIsRunning) {
            Bunit.SetActive(false);
            Tunit.SetActive(false);
            Punit.SetActive(false);
            Dunit.SetActive(false);
        }

        if (!partOneIsDone)
        {
            if (LogicStatusRegister.userIsLoggedInAsPicker)
            {
                if (!CommissioningTime.sessionIsRunning)
                {
                    CommissioningTime.sessionIsRunning = true;
                    CommissioningTime.basisTimeIsRunning = true;
                    _PickerCartTechnics.BootSystem();
                }
                if (LogicStatusRegister.pickerCartInitialized)
                {
                    CommissioningTime.basisTimeIsRunning = false;
                    partOneIsDone = true;
                }
            }
        }

        if (partOneIsDone && !partTwoIsDone)
        { //if part one is done, we are allowed to travel and pick to achieve partTwo as done
            if (LogicStatusRegister.pickerCartGrabbed && LogicStatusRegister.userOnMove)
                CommissioningTime.travelTimeIsRunning = true;
            else
                CommissioningTime.travelTimeIsRunning = false;

            if (LogicStatusRegister.pickingIsDone && LogicStatusRegister.AllWheelsOnTerminanBase())
                partTwoIsDone = true;
        }

        if (partTwoIsDone && !partThreeIsDone) {
            if (LogicStatusRegister.AllWheelsOnTerminanBase()) {    /*&& !LogicStatusRegister.userIsLoggedInAsPicker*/ // => 
                partThreeIsDone = true;
                CommissioningTime.sessionIsRunning = false;
                StartCoroutine(FadeToNextLevel(1));
            } 
        }
    }

    private IEnumerator FadeToNextLevel(int lv) {
        SceneFade.View(Color.black, 2f);
        yield return new WaitForSeconds(2f);
        ScenesTransitioner.Load("Menu", "Results", "false");
    }
}