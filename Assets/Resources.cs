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
    public List<GameObject> NutrientDeposits;

    float ExtractionTimerLong = 0;
    float ExtractionTimerLongMax = 2;
    float ExtractionTimerShort = 0;
    float ExtractionTimerShortMax = 0.1f;
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
        Nutrients = 10;
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
        Sunlight = UnityEngine.Random.Range(0f, 1f);
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
                ExtractionTimerShort += Time.deltaTime;
                if (ExtractionTimerShort >= ExtractionTimerShortMax)
                {
                    ExtractionTimerShort -= ExtractionTimerShortMax;
                    Nutrient NutrientScript = NutrientDeposits[CurrentExtractionTarget].GetComponent<Nutrient>();
                    if (!NutrientScript.Empty && NutrientScript.IsTapped)
                    {
                        Nutrients += NutrientScript.DrainSpeed * NutrientScript.PlayerTaps;
                        //add opponent nutrient gains here
                        if(!NutrientScript.Unlimited)
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
    }
}
