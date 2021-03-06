﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularLight : PoweredObject
{

    public bool onInitially;

    // Start is called before the first frame update
    void Start() { Reset(); }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PowerOnAction(){ LightsOn(); }
    public override void PowerOffAction(){ LightsOff(); }

    public void LightsOn(){  
        foreach (UnityEngine.Experimental.Rendering.Universal.Light2D light in GetComponentsInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D> ())
            light.enabled = true;
    }

    public void LightsOff(){
        foreach (UnityEngine.Experimental.Rendering.Universal.Light2D light in GetComponentsInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D> ())
            light.enabled = false;
    }

    public void Reset(){ SetPower(onInitially); }

}
