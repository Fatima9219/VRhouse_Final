using UnityEngine;
using UnityEngine.UI;
using System;

public class DateAndTime : MonoBehaviour
{
    private Text textClock;

    private void Awake() {
        textClock = GetComponent<Text>();
    }
    private void Update() {
        DateTime time = DateTime.Now;
        if(textClock != null)
            textClock.text = LeadingZero(time.Hour) + ":" + LeadingZero(time.Minute); // + ":" + LeadingZero(time.Second);
    }
    private string LeadingZero(int n) {
        return n.ToString().PadLeft(2, '0');
    }

    public string GetDate() {
        return DateTime.Now.ToString("dd.MM.yyyy");
    }
}