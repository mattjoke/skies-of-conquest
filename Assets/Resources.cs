using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resources : MonoBehaviour
{
    public float Sunlight;
    public float Energy;
    public int Nutrients;
    public Slider HealthSlider;
    public TMP_Text NutrientText;
    public GameObject NumberPopup;
    public Camera MapCamera;
    public List<GameObject> NutrientDeposits;

    public float LeftOpponentSunlight;
    public float RighOpponenttSunlight;
    public int LeftOpponentNutrients;
    public int RightOpponentNutrients;

    public float ExtractionTimerLong = 0;
    float ExtractionTimerLongMax = 2;
    bool Extracting = false;
    int CurrentExtractionTarget = 0;

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        Sunlight = 1;
        Energy = 1;
        Nutrients = 1000;
    }

    void Update()
    {
        TickEnergySunlight();
        TickNutrientExtraction();
        UpdateHUD();
    }

    void UpdateHUD()
    {
        HealthSlider.value = Energy;
        NutrientText.text = Nutrients.ToString();
    }

    void TickEnergySunlight()
    {
        //Sunlight = UnityEngine.Random.Range(0f, 1f);
        if (Energy > Sunlight)
        {
            Energy -= 0.01f;
        }
        if (Energy < Sunlight)
        {
            Energy = Sunlight;
        }
    }

    void TickNutrientExtraction()
    {
        ExtractionTimerLong += Time.deltaTime;
        if(ExtractionTimerLong >= ExtractionTimerLongMax)
        {
            Extracting = true;
            ExtractionTimerLong -= ExtractionTimerLongMax;
        }
        if(Extracting)
        {
            if(CurrentExtractionTarget == NutrientDeposits.Count)
            {
                Extracting = false;
                CurrentExtractionTarget = 0;
            }
            else
            {
                Nutrient NutrientScript = NutrientDeposits[CurrentExtractionTarget].GetComponent<Nutrient>();
                if (!NutrientScript.Empty && NutrientScript.IsTapped)
                {
                    if (NutrientScript.PlayerTaps != 0)
                    {
                        DisplayResourceGain(
                            NutrientScript.DrainSpeed * NutrientScript.PlayerTaps,
                            NutrientScript.transform.position
                        );
                    }
                    Nutrients += NutrientScript.DrainSpeed * NutrientScript.PlayerTaps;
                    LeftOpponentNutrients += NutrientScript.DrainSpeed * NutrientScript.LeftOpponentTaps;
                    RightOpponentNutrients += NutrientScript.DrainSpeed * NutrientScript.RightOpponentTaps;
                    Debug.Log("Left nutrients: " + LeftOpponentNutrients.ToString() + "     Right nutrients: " + RightOpponentNutrients.ToString());
                    if (!NutrientScript.Unlimited)
                    {
                        NutrientScript.NutrientsLeft -= NutrientScript.DrainSpeed *
                            (NutrientScript.PlayerTaps + NutrientScript.LeftOpponentTaps + NutrientScript.RightOpponentTaps);
                        if(NutrientScript.NutrientsLeft <= 0)
                        {
                            NutrientScript.Empty = true;
                        }
                    }
                }
                CurrentExtractionTarget++;
            }
        }
    }

    void DisplayResourceGain(int amount,Vector3 position)
    {
        GameObject PopupInstance = Instantiate(NumberPopup,position,Quaternion.identity);
        PopupInstance.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = "+" + amount.ToString();
    }
}
