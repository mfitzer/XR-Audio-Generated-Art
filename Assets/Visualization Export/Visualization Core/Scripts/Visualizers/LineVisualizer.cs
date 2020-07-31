﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineVisualizer
{
    [HideInInspector]
    public TrailRenderer trailRenderer;

    [Header("Width")]
    
    public bool controlWidth = true;

    public VisualizationTriggerAuthoring widthChangeTrigger =
        new VisualizationTriggerAuthoring(new ContinuousTrigger());

    public float minWidth = 0.5f;    
    public float maxWidth = 1f;

    [Header("Color")]
    
    public bool controlColor = true;

    public VisualizationTriggerAuthoring colorChangeTrigger =
        new VisualizationTriggerAuthoring(new TargetAmplitudeTrigger(0.5f, 1));
    
    public float colorChangeSpeed = 1f;

    [Range(0, 1)]
    public float hueMin = 0f;

    [Range(0, 1)]
    public float hueMax = 1f;
    
    public float emissionIntensity = 1f;

    [ShaderProperty(ShaderPropertyType.Color)]
    public ShaderPropertyField shaderColorField = new ShaderPropertyField
    {
        fieldName = "_EmissionColor"
    };

    private Color targetColor;

    [Header("Lifetime")]

    public bool controlLifetime = true;

    public VisualizationTriggerAuthoring lifetimeChangeTrigger =
        new VisualizationTriggerAuthoring(new ContinuousTrigger());
    
    [Tooltip("Minimum value of the trail renderer time field.")]
    public float minLifetime = 1f;

    [Tooltip("Maximum value of the trail renderer time field.")]
    public float maxLifetime = 5f;

    public void visualize(float amplitude, float visualizerScaleFactor)
    {
        if (controlWidth)
            setWidth(amplitude, visualizerScaleFactor);

        if (controlColor)
            setColor(amplitude);

        if (controlLifetime)
            setLifetime(amplitude);
    }

    public void setWidth(float amplitude, float visualizerScaleFactor)
    {
        if (widthChangeTrigger.trigger.checkTrigger(amplitude))
        {
            float widthInterpolation = (maxWidth - minWidth) * visualizerScaleFactor * amplitude;
            trailRenderer.widthMultiplier = minWidth + widthInterpolation;
        }        
    }

    //Randomly set emission color based on audio amplitude.
    public void setColor(float amplitude)
    {
        if (colorChangeTrigger.trigger.checkTrigger(amplitude))
            targetColor = Random.ColorHSV(hueMin, hueMax, 1f, 1f, 0, 1, 1, 1) * emissionIntensity;

        //Lerp to target color
        Color currentColor = trailRenderer.material.GetColor(shaderColorField.fieldName);
        Color color = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorChangeSpeed);

        trailRenderer.material.SetColor(shaderColorField.fieldName, color);
    }

    public void setLifetime(float amplitude)
    {
        if (lifetimeChangeTrigger.trigger.checkTrigger(amplitude))
        {
            trailRenderer.time = (maxLifetime - minLifetime) * (1 - amplitude);
        }
    }
}
