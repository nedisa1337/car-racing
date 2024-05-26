using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{

    public ParticleSystem[] smoke;
    private CarController controller;
    private bool smokeFlag = false;

    private void Start()
    {
        controller = GetComponent<CarController>();
    }

    private void FixedUpdate()
    {
        if (controller.playPauseSmoke)
            startSmoke();
        else
            stopSmoke();

        if (smokeFlag) return;
        for(int i = 0; i < smoke.Length; i++)
        {
            var emission = smoke[i].emission;
            emission.rateOverTime = ((int)controller.KPH * 10 <= 2000) ? (int)controller.KPH * 10 : 2000;
        }
    }

    public void startSmoke()
    {
        if(smokeFlag) return;
        for(int i = 0; i < smoke.Length; i++)
        {
            var emission = smoke[i].emission;
            emission.rateOverTime = ((int)controller.KPH * 10 <= 2000) ? (int)controller.KPH * 10 : 2000;
            smoke[i].Play();
        }
        smokeFlag = true;
    }

    public void stopSmoke()
    {
        if (!smokeFlag) return;
        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Stop();
        }
        smokeFlag = false;
    }
}
