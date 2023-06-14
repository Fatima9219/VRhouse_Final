using System.IO;
using UnityEngine;

public static class LevelProfile
{
    private static JSONNode jsonNode;
    private static int ListNr;

    private static int ListElementCount;

    private static string   Position,
                            ArticleNr,
                            Quantity,
                            Unit,
                            Description,
                            Annotation;

    private static readonly int[]    Basket = new int[6];

    public static int       ScoreA,
                            ScoreB,
                            ScoreC;

    public static int GetListElementCount() {
        return ListElementCount;
    }

    public static string GetPosition() {
        return Position;
    }

    public static string GetArticleNr() {
        return ArticleNr;
    }

    public static string GetQuantity() {
        return Quantity;
    }

    public static string GetUnit() {
        return Unit;
    }

    public static string GetDescription() {
        return Description;
    }

    public static string GetAnnotation() {
        return Annotation;
    }

    public static void Init(int list_nr) {
        ListNr = list_nr;
        jsonNode = JSON.Parse(File.ReadAllText(LogicStatusRegister.level));

        ListElementCount = jsonNode["PickingList"][list_nr]["PickUnit"].Count;

        ScoreA = jsonNode["PickingList"][list_nr]["Score"]["A"];
        ScoreB = jsonNode["PickingList"][list_nr]["Score"]["B"];
        ScoreC = jsonNode["PickingList"][list_nr]["Score"]["C"];
    }

    public static void LoadUnit(int unitNr) {
        Position    = jsonNode["PickingList"][ListNr]["PickUnit"][unitNr]["position"];
        ArticleNr   = jsonNode["PickingList"][ListNr]["PickUnit"][unitNr]["articleNr"];
        Quantity    = jsonNode["PickingList"][ListNr]["PickUnit"][unitNr]["quantity"];
        Unit        = jsonNode["PickingList"][ListNr]["PickUnit"][unitNr]["unit"];
        Description = jsonNode["PickingList"][ListNr]["PickUnit"][unitNr]["description"];
        Annotation  = jsonNode["PickingList"][ListNr]["PickUnit"][unitNr]["annotation"];

        int i = 0;
        foreach (JSONNode j in jsonNode["PickingList"][ListNr]["PickUnit"][unitNr]["basket"]) {
            Basket[i] = j["count"].AsInt;
            i++;
        }
    }

    public static int[] SetCountToBaskets() {
        return Basket;
    }
}