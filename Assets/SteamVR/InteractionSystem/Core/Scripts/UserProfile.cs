using System.IO;

public static class UserProfile
{
    private static string   Username = "",
                            Firstname = "",
                            Lastname = "";

    private static void SetUserName(string name) {
        Username = name;
    }
    private static void SetFirstName(string name) {
        Firstname = name;
    }
    private static void SetLastName(string name) {
        Lastname = name;
    }
    public static string GetUserName() {
        return Username;
    }
    public static string GetFirstName() {
        return Firstname;
    }
    public static string GetLastName() {
        return Lastname;
    }

    public static void Init()
    {   // The Path have to be changed here
        //JSONNode jsonNode = JSON.Parse(File.ReadAllText(@"C:\Program Files\BDGL\Data\apps\common\VRhouse" + @"\profile.json"));
        //SetUserName(jsonNode["username"].Value);
        //SetFirstName(jsonNode["firstname"].Value);
        //SetLastName(jsonNode["lastname"].Value);
    }
}