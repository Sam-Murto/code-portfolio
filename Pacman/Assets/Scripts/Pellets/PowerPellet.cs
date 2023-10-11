using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : BasePellet
{
    public float duration { get; private set; } = 8f;

    void OnEnable()
    {
        gameObject.name = "Super Pellet";
        value = 50;
    }

}
