using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalPellet : BasePellet
{
    // Start is called before the first frame update
    void OnEnable()
    {
        gameObject.name = "Pellet";
        value = 10;
    }

}
