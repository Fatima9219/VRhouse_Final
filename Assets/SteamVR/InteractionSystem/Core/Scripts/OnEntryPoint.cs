using UnityEngine;


// Hier ueberpruefen wir ob die erhaltenen Argumente von BDGL valide sind.
// ob die Profile Datei und Profile Picture im Data Ordner sind und wir auf diese zugreifen können

public class OnEntryPoint : MonoBehaviour
{
    private string[] args;
    private static string path;
    private static string system_default_locale;

    string[] parameter;

    void Start()
    {
        args = System.Environment.GetCommandLineArgs();

        parameter = args[1].ToString().Split(';');

        //string[] parameter = args[1].ToString().Split(';');

        //Debug.Log(parameter[0]); //username
        //Debug.Log(parameter[1]); //accesskey

        //Debug.Log("Got Parameter: " + args[1] + "!");

        //string s = "You win some. You lose some.";

        //string[] subs = s.Split(' ', '.');

        LoadSystemConfig();
    }

    private void LoadSystemConfig() {
       
    }
}