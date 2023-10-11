using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer: MonoBehaviour
{
    private Action timedAction;
    private float time = 0f;

    // Update is called once per frame
    void Update()
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;

            if (time <= 0f)
            {
                timedAction();
            }
        }
            
    }

    public void SetTimer(float time, Action action)
    {
        this.time = time;
        timedAction = action;
    }

}
