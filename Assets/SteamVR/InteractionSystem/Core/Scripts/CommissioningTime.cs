using System;
using UnityEngine;

// Here we calculate the timings
public static class CommissioningTime
{
    public enum Unit {
        basis, travel, picking, dead
    }

    private static float    basisTime = 0,
                            travelTime = 0,
                            pickingTime = 0,
                            deadTime = 0;

    public static bool      sessionIsRunning = false,
                            basisTimeIsRunning = false,
                            travelTimeIsRunning = false,
                            pickingTimeIsRunning = false;

    /// Have to be called inside a Update function
    public static void Counter(Unit timer) {
        switch (timer) {
            case Unit.basis:
                basisTime += Time.deltaTime;
                break;
            case Unit.travel:
                travelTime += Time.deltaTime;
                break;
            case Unit.picking:
                pickingTime += Time.deltaTime;
                break;
            case Unit.dead:
                deadTime += Time.deltaTime;
                break;
            default:
                break;
        }
    }
    public static float GetBasetime() {
        return basisTime;
    }
    public static float GetTraveltime() {
        return travelTime;
    }
    public static float GetPickingtime() {
        return pickingTime;
    }
    public static float GetDeadtime() {
        return deadTime;
    }
    public static TimeSpan GetCommissioningTime() {
        return TimeSpan.FromSeconds(basisTime + (travelTime + pickingTime + deadTime));
    }
    public static float GetCommissioningTimeRAW() {  // eines von den beiden methoden muss weg
        return basisTime + (travelTime + pickingTime + deadTime);
    }
    public static void ResetAllValues() {
        basisTime = travelTime = pickingTime = deadTime = 0;
        sessionIsRunning = basisTimeIsRunning = travelTimeIsRunning = pickingTimeIsRunning = false;
    }
}