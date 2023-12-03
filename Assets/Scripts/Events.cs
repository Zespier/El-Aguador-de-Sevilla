using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events : MonoBehaviour {

    public static Action OnPickUp;
    public static Action<int> OnClientServed;
    public static Action OnStartLevel;
    public static Action OnFullWaterRetrieved;
    public static Action OnLevelCompleted;

}
