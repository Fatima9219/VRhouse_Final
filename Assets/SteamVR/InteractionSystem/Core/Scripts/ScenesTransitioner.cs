using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class ScenesTransitioner
{
    private static Dictionary<string, string> parameters;
    public static void Load(string sceneName, Dictionary<string, string> parameters = null)
    {
        ScenesTransitioner.parameters = parameters;
        SceneManager.LoadScene(sceneName);
    }
    public static void Load(string sceneName, string paramKey, string paramValue)
    {
        parameters = new Dictionary<string, string> { { paramKey, paramValue } };
        SceneManager.LoadScene(sceneName);
    }
    public static Dictionary<string, string> getSceneParameters()
    {
        return parameters;
    }
    public static string GetParam(string paramKey)
    {
        if (parameters == null) return "";
        return parameters[paramKey];
    }
    public static void SetParam(string paramKey, string paramValue)
    {
        if (parameters == null)
            parameters = new Dictionary<string, string>();
        parameters.Add(paramKey, paramValue);
    }
}