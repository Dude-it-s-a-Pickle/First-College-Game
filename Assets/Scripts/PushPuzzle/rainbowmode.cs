using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rainbowmode : MonoBehaviour
{

    private float hue;
    private float sat;
    private float bri;

    public float speed;

    public SpriteRenderer Sprite;

    private void Start()
    {

        Sprite.GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        Color.RGBToHSV(Sprite.material.color, out hue, out sat, out bri);

        hue += speed / 10000;

        if (hue >= 1)
        {
            hue = 0;
        }


        sat = 1;
        bri = 1;

        Sprite.material.color = Color.HSVToRGB(hue, sat, bri);




        

    }
}
