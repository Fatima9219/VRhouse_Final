/*
  Hier speichern wir alle Zustände.
 */

public static class LogicStatusRegister
{
    //public static string level = "Assets/Scenes/Assets/Level/PickByLight.json";  //set from level2.cs
    public static string level = @"C:\Program Files\BDGL\Data\apps\common\VRhouse\VRhouse_Data\PickByLight.json";

    public static bool userIsLoggedInAsPicker = false;
    public static bool userIsLoggedInAsTransporter = false;
    public static bool userIsLoggedInAsManager = false;
    public static bool pickerCartIsBooted = false;
    public static bool pickerCartInitialized = false;
    public static bool pickingSequenzeRunning = false;
    public static bool pickerCartGrabbed = false;
    public static bool userOnMove = false;
    public static bool pickingIsDone = false;

    public static string lastScanUnit = "";
    public static string lastPTLQuotationUnit = "";

    public static bool[] PTLIsQuoted = new bool[6];
    public static bool[] PickingCartWheels = new bool[4];

    public static bool BarcodeCaptured = false;
    public static int  errorCounter = 0;

    //PBVi (QND)
    public static bool scannedNumber = false;
    public static bool correctNumberScanned = false;
    public static bool bootingPBVicompleted = false;

    public static bool AllWheelsOnTerminanBase() {
        return AllTrue(PickingCartWheels);
    }

    public static bool AllTrue(bool[] array) {
        foreach (bool b in array) if (!b) return false;
        return true;
    }

    public static void ResetAllValues() {
        userIsLoggedInAsPicker = false;
        userIsLoggedInAsTransporter = false;
        userIsLoggedInAsManager = false;
        pickerCartIsBooted = false;
        pickerCartInitialized = false;
        pickingSequenzeRunning = false;
        pickerCartGrabbed = false;
        userOnMove = false;
        pickingIsDone = false;
        
        lastScanUnit = "";
        lastPTLQuotationUnit = "";

        errorCounter = 0;
    }
}