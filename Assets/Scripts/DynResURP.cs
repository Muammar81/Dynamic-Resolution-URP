using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DynResURP : MonoBehaviour
{
    private UniversalRenderPipelineAsset _urpAsset;
    [SerializeField] [Range(0.1f, 1)] private float res=0.1f;
    

    void Awake()
    {
        _urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;

    }

    private void Update()
    {
        if (_urpAsset != null)
            _urpAsset.renderScale = res;
    }
}