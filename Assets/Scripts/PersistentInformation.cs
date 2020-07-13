using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentInformation
{
    public static int CurrentChapter { get; set; }

    public static float Left { get; set; }

    public static float Right { get; set; }

    public static bool MarginsSet { get; set; }

    public static string LevelIdentifier { get; set; }

}