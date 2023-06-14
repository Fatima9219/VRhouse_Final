/*
    In this Class we defines the whole logic that have to be achieved to solve the given tasks
 */

using System;
using UnityEngine;
using UnityEngine.UI;

public class Level1 : MonoBehaviour
{
    private GameObject PickerCartTechnics, Bunit, Tunit, Punit, Dunit;

    private Text SkillometerTime;

    void Start()
    {
        PickerCartTechnics = GameObject.Find("PickerCart").transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        PickerCartTechnics.SetActive(false);

        SkillometerTime = GameObject.Find("Skillometer").transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        Bunit = GameObject.Find("Skillometer").transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        Tunit = GameObject.Find("Skillometer").transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        Punit = GameObject.Find("Skillometer").transform.GetChild(0).gameObject.transform.GetChild(3).gameObject;
        Dunit = GameObject.Find("Skillometer").transform.GetChild(0).gameObject.transform.GetChild(4).gameObject;
    }

    private void Update()
    {
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


        if (Input.GetKeyDown("c"))
        {
            Debug.Log("-----------------------------------------");
            Debug.Log("basisTime "   + TimeSpan.FromSeconds(CommissioningTime.GetBasetime()).ToString("mm\\:ss\\.fff"));
            Debug.Log("travelTime "  + TimeSpan.FromSeconds(CommissioningTime.GetTraveltime()).ToString("mm\\:ss\\.fff"));
            Debug.Log("pickingTime " + TimeSpan.FromSeconds(CommissioningTime.GetPickingtime()).ToString("mm\\:ss\\.fff"));
            Debug.Log("deadTime "    + TimeSpan.FromSeconds(CommissioningTime.GetDeadtime()).ToString("mm\\:ss\\.fff"));        
            Debug.Log("Kommissionszeit gesamt" + CommissioningTime.GetCommissioningTime().ToString("mm\\:ss\\.fff"));
        }
    }
}

// Spielablauf:
// Der Spieler bewegt sich zur dem Terminal. Auf dem Monitor findet er die Aufschrift: "F�r die �bernahme des Auftrags loggen Sie sich bitte mit dem Barcode-Scanner an".
// Die Session startet sobald der Spieler sich mit dem Barcode-Scanner einloggt und damit die �bernahme des Auftrags bek�ndet
// -> sessionIsStarted = true! (Gesamtzeit)  basiszeit startet  Der Kommissionierwagen bootet  Die 6 L�mpchen leuchten auf.
// Der Spieler f�hrt eine Initialisierung mit dem Kommissionierwagen aus. 
// -> Auftrag erscheint (Der Spieler ist noch am lesen). Sobald er sich den Griff des Wagens schnappt, endet die Basiszeit.
// -> Wenn der Spieler sich bewegt, l�uft die Wegzeit, ansonsten die deadtime
// Der Spieler geht zum Regal und muss das ben�tigte Item einscannen.
// Sobald der Spieler sich den BC-Scanner gegriffen hat beginnt die picking time. Sobald die Elemente allesamt in die K�rbe des Wagens gelegt wurden, muss der Spieler dies nur nur quitieren.
// -> Nach der Quitierung stoppt die picking time. N�chster Auftrag wird angezeigt. deadtime l�uft. Sobald die Halterung des Wagens wieder gegriffen wurde und der Spieler sich bewegt, l�uft die wegzeit.
// Nachdem alle items in die K�rbe gelegt wurden, steht auf dem Bildschirm des Wagens: "Erledigt"! (Bitte zur�ck zum Terminal)
// Sobald der Spieler wieder am Terminal angekommen ist endet die session womit die Spielrunde als beendet gilt
// Der Spieler wird automatisch in das Menu gef�hrt wo er sich die Ergebnisse anschauen kann.

//note: PickingTime startet, wenn der Spieler sich den BCS schnappt und endet wenn dieses wieder in der halterung liegt.
//note: PickingTime startet, wenn der Spieler sich das objekt schnappt und endet, wenn dieses in den Korb gelegt wird.