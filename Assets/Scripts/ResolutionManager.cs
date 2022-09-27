using System.Text;
using TMPro;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    private FrameTiming[] frameTimings;
    private uint numTimings = 3;
    private Camera cam;
    private Resolution baseResolution;
    private StringBuilder sb;

    [SerializeField] [Range(0.1f, 1f)] private float WScale = 1;

    private void Awake()
    {
        Resolution[] resolutions = Screen.resolutions;
        cam = Camera.main;
        cam.allowDynamicResolution = true;
        frameTimings = new FrameTiming[numTimings];

        /*RenderTexture[] rendTex = FindObjectsOfType<RenderTexture>();
        rendTex.ToList().ForEach(r => r.useDynamicScale = true);*/

        baseResolution = Screen.currentResolution;
        /*        GlobalDynamicResolutionSettings g = new GlobalDynamicResolutionSettings();
                g.enabled = true;
                g.dynResType = DynamicResolutionType.Software;
        g.minPercentage = 0.1f;*/
        sb = new StringBuilder();
    }

    private void Update()
    {
        FrameTimingManager.CaptureFrameTimings();
        uint numReturned = FrameTimingManager.GetLatestTimings(numTimings, frameTimings);

        //WScale = 0.1f;
        //if (frameTimings != null && frameTimings.Length < numTimings)
        ScalableBufferManager.ResizeBuffers(WScale, WScale);

        //DynamicRes.currentResScale = 0.1f;


        if (!IsCPUBound())
        {
            //Screen.SetResolution(600, 400, true);
        }
    }

    void OnGUI()
    {
        Rect rect = new Rect(50, 50, Screen.width,Screen.height);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 70;
        style.normal.textColor = Color.white;
        float fps = 1f / Time.unscaledDeltaTime;
        GUI.Label(rect,fps.ToString("N0"),style);
    }

    bool IsCPUBound()
    {
        long presentCalled = (long)frameTimings[0].cpuTimePresentCalled;
        long frameComplete = (long)frameTimings[0].cpuTimeFrameComplete;

        float latencyMs = 1000 *
            (frameComplete - presentCalled) / (float)FrameTimingManager.GetCpuTimerFrequency();

        float frameLatency = latencyMs / (float)frameTimings[0].gpuFrameTime;
        return frameLatency <= 0.5f; //Recommended threshold by XBOX
    }
}
