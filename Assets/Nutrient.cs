using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutrient : MonoBehaviour
{
    public int NutrientsLeft;
    public int DrainSpeed;
    public bool Unlimited;
    public bool Empty = false;
    public bool IsTapped = false;
    public int PlayerTaps = 0;
    public int LeftOpponentTaps = 0;
    public int RightOpponentTaps = 0;
}
