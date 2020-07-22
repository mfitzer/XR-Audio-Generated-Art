﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class LineBandVisualizer : AudioBandVisualizer
{
    [SerializeField, HideInInspector]
    private TrailRenderer trailRenderer;

    [Header("Width")]

    [SerializeField]
    private float minWidth = 0.5f;

    [SerializeField]
    private float maxWidth = 1f;

    [Header("Color")]

    [SerializeField, Range(0, 1)]
    private float colorChangeThreshold = 0.5f;

    [SerializeField]
    private float colorChangeSpeed = 1f;

    [SerializeField, Range(0, 1)]
    private float hueMin = 0f;

    [SerializeField, Range(0, 1)]
    private float hueMax = 1f;

    [SerializeField]
    private float emissionIntensity = 1f;

    [SerializeField, ShaderProperty(ShaderPropertyType.Color)]
    private ShaderPropertyField shaderColorField = new ShaderPropertyField
    {
        fieldName = "_EmissionColor"
    };

    private Color targetColor;

    private void OnValidate()
    {
        if (!trailRenderer)
            trailRenderer = GetComponent<TrailRenderer>();

        if (trailRenderer)
        {
            if (trailRenderer.sharedMaterial)
            {
                shaderColorField.shader = trailRenderer.sharedMaterial.shader;
            }
            else
            {
                shaderColorField.shader = null;
            }
        }
    }

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    protected override void visualizeData(VisualizationData data)
    {
        setWidth(data.audioBands[band]);
        setColor(data.audioBands[band]);
    }

    private void setWidth(float bandAmplitude)
    {
        float widthInterpolation = (maxWidth - minWidth) * visualizerScaleFactor * bandAmplitude;
        trailRenderer.widthMultiplier = minWidth + widthInterpolation;
    }

    //Randomly set emission color based on audio band amplitude.
    private void setColor(float bandAmplitude)
    {
        // TIMER to determine when to change
        if (bandAmplitude > colorChangeThreshold)
            targetColor = Random.ColorHSV(hueMin, hueMax, 1f, 1f, 0, 1, 1, 1) * emissionIntensity;

        //Lerp to target color
        Color currentColor = trailRenderer.material.GetColor(shaderColorField.fieldName);
        Color color = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorChangeSpeed);

        trailRenderer.material.SetColor(shaderColorField.fieldName, color);
    }
}