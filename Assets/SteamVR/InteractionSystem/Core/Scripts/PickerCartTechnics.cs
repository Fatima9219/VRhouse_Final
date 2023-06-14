using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//NOTE. Vielleicht statt ResetValues. einfach am Ende die Werte allesamt auf false setzen. ohne dies später in der Init tun zu müssen?
// Access to all Technic Elements
public class PickerCartTechnics : MonoBehaviour
{
    private GameObject      _PickerCartTechnics,
                            _PickerCartTablet,
                            _PickerCartTabletDisplay_Boot,
                            _PickerCartTabletDisplay_Orders,
                            _PickerCartTabletDisplay_Completed,
                            _UserName,
                            _ActualDate;

    private GameObject[]    _PTL_BulbLED,
                            _PTL_Btn_Quotation;

    private Text            _Position,
                            _ArticleNr,
                            _Quantity,
                            _Unit,
                            _Description,
                            _Annotation;

    private Text[]          _PTL_DisplayText;
                            
    private Renderer        _renderer;

    private Material        _tablet_LED_material,
                            _tablet_material;

    private bool            _systemInitSeqIsRunning = false,
                            _pickingPhase = false;

    private int[]           _basket = new int[6];
    private int             ListUnitCount;
    private bool            actualUnitWasPicked = true;  //  true wenn alle einheiten in körben und bestätigt
    private int             pickUnitNr;

    public static bool[]    PTLIsRegistered = new bool[6];

    //public static ScanCollider _ScanCollider;

    private void Awake()
    {
        _PickerCartTechnics = GameObject.Find("Technics");
        _PickerCartTablet = GameObject.Find("Tablet");

        if (_PickerCartTechnics != null) {
            _renderer = _PickerCartTablet.GetComponent<Renderer>();

            foreach (Material matt in _renderer.materials)
                if (matt.name.Equals("TabletLED (Instance)"))
                    _tablet_LED_material = matt;


            foreach (Material matt in _renderer.materials)
                if (matt.name.Equals("TabletDisplay (Instance)"))
                {
                    _tablet_material = matt;
                }

            //_tablet_material = _PickerCartTablet.GetComponent<Renderer>().material;

            _PickerCartTabletDisplay_Boot = _PickerCartTablet.transform.GetChild(0).gameObject;
            _PickerCartTabletDisplay_Orders = _PickerCartTablet.transform.GetChild(1).gameObject;
            _PickerCartTabletDisplay_Completed = _PickerCartTablet.transform.GetChild(2).gameObject;

            _UserName = _PickerCartTabletDisplay_Orders.transform.GetChild(1).gameObject;
            _ActualDate = _PickerCartTabletDisplay_Orders.transform.GetChild(2).gameObject;
            _Position = _PickerCartTabletDisplay_Orders.transform.GetChild(4).gameObject.GetComponent<Text>();
            _ArticleNr = _PickerCartTabletDisplay_Orders.transform.GetChild(6).gameObject.GetComponent<Text>();
            _Quantity = _PickerCartTabletDisplay_Orders.transform.GetChild(10).gameObject.GetComponent<Text>();
            _Unit = _PickerCartTabletDisplay_Orders.transform.GetChild(12).gameObject.GetComponent<Text>();
            _Description = _PickerCartTabletDisplay_Orders.transform.GetChild(8).gameObject.GetComponent<Text>();
            _Annotation = _PickerCartTabletDisplay_Orders.transform.GetChild(14).gameObject.GetComponent<Text>();

            _PTL_BulbLED = new GameObject[6];
            _PTL_DisplayText = new Text[6];
            _PTL_Btn_Quotation = new GameObject[6];
            for (int i = 0; i != 6; i++)
            {
                _PTL_BulbLED[i] = _PickerCartTechnics.transform.GetChild(3 + i).gameObject.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject;
                _PTL_DisplayText[i] = _PickerCartTechnics.transform.GetChild(9 + i).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
                //ADD buttons if needed
            }
        }  
    }

    public void BootSystem()
    {
        StartCoroutine(BootSequence());
    }

    private void SystemInitSeq() {
        //prevent from lighting after register
        foreach (GameObject bulb in _PTL_BulbLED)
            bulb.SetActive(false);

        //blink if not registered
        for (int i=0; i!=6; i++) {
            if (!PTLIsRegistered[i])
                BulbBlink(i);
        }

        //QND way   Combine Unit Basket + PTL  / Reihenfolge wird jedoch nicht beachtet!
        if (LogicStatusRegister.lastScanUnit.Equals("BarcodeBasket1") && LogicStatusRegister.lastPTLQuotationUnit.Equals("PTL_1"))
            PTLIsRegistered[0] = true;

        if (LogicStatusRegister.lastScanUnit.Equals("BarcodeBasket2") && LogicStatusRegister.lastPTLQuotationUnit.Equals("PTL_2"))
            PTLIsRegistered[1] = true;

        if (LogicStatusRegister.lastScanUnit.Equals("BarcodeBasket3") && LogicStatusRegister.lastPTLQuotationUnit.Equals("PTL_3"))
            PTLIsRegistered[2] = true;

        if (LogicStatusRegister.lastScanUnit.Equals("BarcodeBasket4") && LogicStatusRegister.lastPTLQuotationUnit.Equals("PTL_4"))
            PTLIsRegistered[3] = true;

        if (LogicStatusRegister.lastScanUnit.Equals("BarcodeBasket5") && LogicStatusRegister.lastPTLQuotationUnit.Equals("PTL_5"))
            PTLIsRegistered[4] = true;

        if (LogicStatusRegister.lastScanUnit.Equals("BarcodeBasket6") && LogicStatusRegister.lastPTLQuotationUnit.Equals("PTL_6"))
            PTLIsRegistered[5] = true;

        if (AllTrue(PTLIsRegistered)) {
            //prevent from lighting after register last element
            foreach (GameObject bulb in _PTL_BulbLED)   
                bulb.SetActive(false);

            LogicStatusRegister.pickerCartInitialized = true;

            for (int i = 0; i != 6; i++) {
                LogicStatusRegister.PTLIsQuoted[i] = false;
                PTLIsRegistered[i] = false;
            }                

            LoadOrderFileAndStartSeq();
        }
    }

    private bool actuallyOnPicking = false;

    private void PickingSeq() { // in update loop   // oder statt update loop in eine while verschachteln?

        if (pickUnitNr != ListUnitCount)   //solange nicht erfüllt, läuft die picking sequence   (+1?) sind wir noch nicht durch
        {
            if (actualUnitWasPicked) { // wenn ja, lade nächste
                StartCoroutine(LoadNextUnitToPick(pickUnitNr));   // nächste einheit zum picken ist geladen

                LogicStatusRegister.lastScanUnit = "";
                actualUnitWasPicked = false;
            }

            if (_PositionToScan.Equals(LogicStatusRegister.lastScanUnit) && _PositionToScan !="") //
            {     //sobald das GESUCHTE (d.h. mit der position nr vergleichen!!) Objekt eingescannt wurde, zeigen wir die baskets und die anzahl
                  //ShowBasketsToFill(basket[0]!=0, basket[1]!=0, basket[2]!=0, basket[3]!=0, basket[4]!=0, basket[5]!=0); // get true or false
                  //prevent from lighting after register. Reset auf false

                actuallyOnPicking = true;
                CommissioningTime.pickingTimeIsRunning = true;

                foreach (GameObject bulb in _PTL_BulbLED)
                    bulb.SetActive(false);

                // Blinke die PTLs wo Anzahl nicht 0 ist und welche noch nicht bestätigt wurden. Wenn bestätigt, leuchte nicht und lösche die Anzeige
                for (int i = 0; i != 6; i++)
                {
                    if ((_basket[i] != 0) && !LogicStatusRegister.PTLIsQuoted[i])
                    {
                        _PTL_DisplayText[i].text = _basket[i].ToString(); //show count
                        BulbBlink(i);  // and blink
                    }
                    else
                        _PTL_DisplayText[i].text = "";   // else not blink and delete text
                }
            }
            else if (!_PositionToScan.Equals(LogicStatusRegister.lastScanUnit) && LogicStatusRegister.lastScanUnit != "" && LogicStatusRegister.BarcodeCaptured) { // auch für den oberen IF anwenden!
                LogicStatusRegister.BarcodeCaptured = false;
                LogicStatusRegister.lastScanUnit = "";
                LogicStatusRegister.errorCounter++;
            }

            if (AllTrue(LogicStatusRegister.PTLIsQuoted)) {// proof if any PTL is on true -> picking is done.
                pickUnitNr++;

                for (int i=0; i!=6; i++) // reset all to false
                    LogicStatusRegister.PTLIsQuoted[i] = false;

                CommissioningTime.pickingTimeIsRunning = false;
                actuallyOnPicking = false;   // tut der user jetzt gerade.
                actualUnitWasPicked = true;  // höhere Ebene. Das Ziel muss noch erreicht werden.
            }
        }

        else { //PickingList completed
            StartCoroutine(ShutDownSystemSequence());
            _pickingPhase = false;
            LogicStatusRegister.pickingIsDone = true;
        }
    }

    public static bool AllTrue(bool[] array) {
        foreach (bool b in array) if (!b) return false;
        return true;
    }

    private void LoadOrderFileAndStartSeq() {
        LevelProfile.Init(0);           // get List Number from Register?
        ListUnitCount = LevelProfile.GetListElementCount();
        _pickingPhase = true;            // und starten damit die PickingPhase
    }

    private void BulbBlink(int n) {
        _PTL_BulbLED[n].SetActive((Time.fixedTime % 0.5 < .2) ? true : false);
    }

    private void Update()
    {
        if (!LogicStatusRegister.pickerCartInitialized && _systemInitSeqIsRunning)
            SystemInitSeq();

        if (!LogicStatusRegister.pickingSequenzeRunning && _pickingPhase)
            PickingSeq();
    }

    private IEnumerator BootSequence() // exactly 10sec
    {
        yield return new WaitForSeconds(2f);
        //PowerLED on
        ///_tablet_LED_material.SetColor("_EmissiveColor", Color.green);  /// next gen.
        _tablet_LED_material.SetColor("_Color", Color.green);  /// standard version
        _tablet_LED_material.SetColor("_EmissionColor", Color.green);  /// standard version

        yield return new WaitForSeconds(3f);
        //Loading Boot Screen
        _PickerCartTabletDisplay_Boot.SetActive(true);
        yield return new WaitForSeconds(3f);
        _PickerCartTabletDisplay_Boot.SetActive(false);

        yield return new WaitForSeconds(1f);
        //Loading Orders Screen
        ///_tablet_material.SetColor("_EmissiveColor", Color.white);   /// next gen.
        ///_tablet_material.SetFloat("_EmissiveIntensity", 20f);   /// next gen.
        ///_tablet_material.SetFloat("_EmissiveExposureWeight", 0.5f);   /// next gen.
        _tablet_material.SetColor("_Color", Color.white);  /// standard version
        yield return new WaitForSeconds(1f);
        _PickerCartTabletDisplay_Orders.SetActive(true);

        _UserName.GetComponent<Text>().text = UserProfile.GetFirstName()+" "+UserProfile.GetLastName();
        _ActualDate.GetComponent<Text>().text = gameObject.AddComponent<DateAndTime>().GetDate();
        
        _systemInitSeqIsRunning = true;
    }

    private IEnumerator ShutDownSystemSequence() {
        yield return new WaitForSeconds(1f);
        _PickerCartTabletDisplay_Orders.SetActive(false);

        yield return new WaitForSeconds(1f);
        _PickerCartTabletDisplay_Completed.SetActive(true);
    }


    private string _PositionToScan = "";

    private IEnumerator LoadNextUnitToPick(int unitNr) {
        // clean screen of the last one for one second
        yield return new WaitForSeconds(1f);
        _Position.text = "";
        _ArticleNr.text = "";
        _Quantity.text = "";
        _Unit.text = "";
        _Description.text = "";
        _Annotation.text = "";

        // load and show next unit to pick
        yield return new WaitForSeconds(1f);
        LevelProfile.LoadUnit(unitNr);
        _Position.text      = LevelProfile.GetPosition();
        _ArticleNr.text     = LevelProfile.GetArticleNr();
        _Quantity.text      = LevelProfile.GetQuantity();
        _Unit.text          = LevelProfile.GetUnit();
        _Description.text   = LevelProfile.GetDescription();
        _Annotation.text    = LevelProfile.GetAnnotation();

        _PositionToScan = _Position.text;

        // load needed count of items for each basket
        _basket = LevelProfile.SetCountToBaskets();

        // TODO setze jene auf true dessen anzahl 0 ist und demnach nicht leuchten sollen. Setze auf TRUE entweder hier oder in dem IEnumerator
        for (int i=0; i!=6; i++)
            if (_basket[i] == 0)
                LogicStatusRegister.PTLIsQuoted[i] = true;
    }
}