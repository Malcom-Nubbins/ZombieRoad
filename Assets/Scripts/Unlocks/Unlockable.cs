using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : MonoBehaviour
{
    public string unlockableID;
    public UnlockableType type;
    public bool unlocked = true;
    public int Price = 0;
}
