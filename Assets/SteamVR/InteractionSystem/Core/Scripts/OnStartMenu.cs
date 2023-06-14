using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System;

// Menu and Evaluation of the results
// TODO: change to more generic code
// TODO : Important! -> Transfer the data to BDGL!
public class OnStartMenu : MonoBehaviour
{
    private string[] args;
    private static string path;
    private static string system_default_loc;

    GameObject MainMenu, ResultMenu;

    Text[] resultsTXT;

    private void Awake()
    {
        if (ScenesTransitioner.GetParam("Results").ToString() != "") {
            MainMenu = GameObject.Find("MainMenu");
            ResultMenu = GameObject.Find("MenuCanvas").transform.GetChild(0).gameObject;
            MainMenu.SetActive(false);
            ResultMenu.SetActive(true);
            // Beachatung der Reihenfolge ist Suboptimal... evtl. besser ändern oder komplett auf Obj.Find umstellen
            resultsTXT = new Text[6];
            resultsTXT[0] = ResultMenu.transform.GetChild(0).GetComponent<Text>();
            resultsTXT[1] = ResultMenu.transform.GetChild(1).GetComponent<Text>();
            resultsTXT[2] = ResultMenu.transform.GetChild(2).GetComponent<Text>();
            resultsTXT[3] = ResultMenu.transform.GetChild(3).GetComponent<Text>();
            resultsTXT[4] = ResultMenu.transform.GetChild(4).GetComponent<Text>();
            resultsTXT[5] = ResultMenu.transform.GetChild(5).GetComponent<Text>();

            resultsTXT[0].text = CommissioningTime.GetCommissioningTime().ToString("mm\\:ss\\.fff");
            resultsTXT[1].text = TimeSpan.FromSeconds(CommissioningTime.GetBasetime()).ToString("mm\\:ss");
            resultsTXT[2].text = TimeSpan.FromSeconds(CommissioningTime.GetTraveltime()).ToString("mm\\:ss");
            resultsTXT[3].text = TimeSpan.FromSeconds(CommissioningTime.GetPickingtime()).ToString("mm\\:ss");
            resultsTXT[4].text = TimeSpan.FromSeconds( CommissioningTime.GetDeadtime()).ToString("mm\\:ss");
            resultsTXT[5].text = LogicStatusRegister.errorCounter.ToString();

            if (CommissioningTime.GetCommissioningTimeRAW() <= LevelProfile.ScoreC)
                if (CommissioningTime.GetCommissioningTimeRAW() <= LevelProfile.ScoreB)
                    if (CommissioningTime.GetCommissioningTimeRAW() <= LevelProfile.ScoreA && LogicStatusRegister.errorCounter == 0)
                    {/* A: Do nothing. Keep all stars */}
                    else
                        GameObject.Find("Star3").SetActive(false); //B : deactivate Star3
                else {
                    GameObject.Find("Star2").SetActive(false); //C: deactivate Star2 and 3
                    GameObject.Find("Star3").SetActive(false); 
                }
            else {
                GameObject.Find("Star1").SetActive(false); //C: deactivate Star2 and 3
                GameObject.Find("Star2").SetActive(false);
                GameObject.Find("Star3").SetActive(false);
            }

            // HERE we can save All Results we want/need inside of the profile
        }

        args = Environment.GetCommandLineArgs();

        LoadProfileImage();
        StartCoroutine(FadeIn());
    }

    void LoadProfileImage()
    { // Das geht auch besser, ändern
        try {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(@"C:\Program Files\BDGL\Data\apps\common\VRhouse\VRhouse_Data\uimg"));
            GameObject.Find("ProfileImg").GetComponent<RawImage>().texture = texture;
        }
        catch {
            Debug.Log("Something goes wrong with the profile image");
        }
    }

    private IEnumerator FadeIn()
    {
        SceneFade.View(Color.clear, 5f);
        yield return new WaitForSeconds(5f);
    }
}