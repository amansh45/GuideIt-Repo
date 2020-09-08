using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentInformation
{
    public static int CurrentChapter { get; set; }

    public static float Left { get; set; }

    public static float Right { get; set; }

    public static bool MarginsSet { get; set; }

    // used only for levelComplete scene
    public static string LevelIdentifier { get; set; }

    // used to set the initial value of the timer while starting the infiniteLevel
    public static float timerForInfinitelevel { get; set; }

}