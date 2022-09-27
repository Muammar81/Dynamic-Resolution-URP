using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Mathf;
using static UnityEngine.Screen;

public class DynamicResolutionController : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 1f)] private float wScale, hScale = 0.1f;
    private uint sampleSize = 5;

    private StringBuilder _sb = new StringBuilder();
    private string _screenText = String.Empty;
    private string _targetScale = String.Empty;
    private DynamicResolutionHandler _dynResHandler;
    private UniversalRenderPipelineAsset _urpAsset;

    private void Awake()
    {
        Camera.main.allowDynamicResolution = true;
        var settings = GlobalDynamicResolutionSettings.NewDefault();
        settings.enabled = true;
        //settings.dynResType = DynamicResolutionType.Hardware;
        //settings.upsampleFilter = DynamicResUpscaleFilter.CatmullRom;
        //settings.forceResolution = true;
        DynamicResolutionHandler.instance.Update(settings, null);

        _dynResHandler = DynamicResolutionHandler.instance;
         
        DynamicResolutionHandler.SetDynamicResScaler(null, DynamicResScalePolicyType.ReturnsPercentage);
        
        // _urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        // _urpAsset.renderScale = 0.1f;

        //SetRenderTextures();
    }

    private void SetRenderTextures()
    {
        var rendTex = FindObjectsOfType<RenderTexture>();
        rendTex?.ToList().ForEach(r => r.useDynamicScale = true);

        if (rendTex != null)
        {
            int texEnabled = rendTex.Count(r => r.useDynamicScale == true);
            print($"Textures in scene with DynRes enabled: {texEnabled}");
        }
    }

    void Update()
    {
        FrameTiming[] frameTiming = new FrameTiming[sampleSize];
        FrameTimingManager.CaptureFrameTimings();
        uint numbReturned = FrameTimingManager.GetLatestTimings(sampleSize, frameTiming);
        ScalableBufferManager.ResizeBuffers(wScale, hScale);

        _targetScale = $"Target Scale: {wScale}x{hScale}\n";

        _sb.Clear();
        _sb.Append($"\nScale: {ScalableBufferManager.widthScaleFactor}x{ScalableBufferManager.heightScaleFactor}");
        _sb.Append($"\nResolution: {currentResolution.width}x{currentResolution.height}");
        _sb.Append($"\nDynRes Enabled: {_dynResHandler.DynamicResolutionEnabled()}");
        _sb.Append($"\nHardware Enabled: {_dynResHandler.HardwareDynamicResIsEnabled()}");
        _sb.Append($"\nSoftware Enabled: {_dynResHandler.SoftwareDynamicResIsEnabled()}");
        _sb.Append($"\nCurrent Scale: {_dynResHandler.GetCurrentScale().ToString()}");
        _screenText = _sb.ToString();

        ChangeRes();
    }

    void ChangeRes()
    {
        // One finger lowers the resolution
        if (Input.GetButtonDown("Fire1"))
        {
            wScale = Max(0.1f, wScale - 0.1f);
            hScale = Max(0.1f, hScale - 0.1f);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            wScale = Min(1, wScale + 0.1f);
            hScale = Min(1, hScale + 0.1f);
        }
    }

    void OnGUI()
    {
        Rect rect = new Rect(50, 50, width - 100, height - 100);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 40;
        style.normal.textColor = Color.red;
        GUI.Label(rect, _targetScale, style);
        style.normal.textColor = Color.cyan;
        GUI.Label(rect, _screenText, style);
    }
}