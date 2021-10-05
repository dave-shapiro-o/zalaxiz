using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourPulse : MonoBehaviour
{
    private float FadeDuration = 1f;

    [SerializeField] private Color startColour;
    [SerializeField] private Color endColour; 

    private float lastColorChangeTime;

    private new Light light; 

    void Start()
    {
        light = GetComponent<Light>();
        //startColor = Color1;
        //endColor = Color2;
    }

    void Update()
    {
        var ratio = (Time.time - lastColorChangeTime) / FadeDuration;
        ratio = Mathf.Clamp01(ratio);
        //material.color = Color.Lerp(startColour, endColour, ratio);
        light.color = Color.Lerp(startColour, endColour, Mathf.Sqrt(ratio)); // A cool effect
        //material.color = Color.Lerp(startColour, endColour, ratio * ratio); // Another cool effect

        if (ratio == 1f)
        {
            lastColorChangeTime = Time.time;

            // Switch colors
            var temp = startColour;
            startColour = endColour;
            endColour = temp;
        }
    }
}
