using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        Sunlight = Random.Range(0f, 1f);
        if(Energy > Sunlight)
        {
            Energy -= 0.01f;
        }
        if(Energy < Sunlight)
        {
            Energy = Sunlight;
        }
        HealthSlider.value = Energy;
        Nutrients++;
        NutrientText.text = Nutrients.ToString();
    }

    public void Reset()
    {
        Sunlight = 1;
        Energy = 1;
        Nutrients = 10;
    }
}
