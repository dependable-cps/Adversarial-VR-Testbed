using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

public class CustomTunnelingVignetteController : MonoBehaviour
{
    public TunnelingVignetteController vignetteController;

    [Header("Cybersickness Severity Level ( 0=Low, 1=Medium, 2=High)")]
    [Range(0, 3)]
    public int severityLevel = 1;

    // [Header("Manual Control")] 
    
    //public bool enableVignette = true;

    // [Range(0f, 1f)] public float apertureSize = 0.7f;
    //
    // [Range(0f, 1f)] public float featheringEffect = 0.2f;
    //
    // [Range(-0.2f, 0.2f)] public float verticalOffset = 0f;
    //
    // public Color vignetteColor = Color.black;
    // public Color vignetteColorBlend = Color.black;

    private MaterialPropertyBlock block;

    private static readonly int _ApertureSize = Shader.PropertyToID("_ApertureSize");
    private static readonly int _FeatheringEffect = Shader.PropertyToID("_FeatheringEffect");
    // private static readonly int _VignetteColor = Shader.PropertyToID("_VignetteColor");
    // private static readonly int _VignetteColorBlend = Shader.PropertyToID("_VignetteColorBlend");
    
    // [SerializeField]
    // VignetteParameters m_DefaultParameters = new VignetteParameters();
    //
    // /// <summary>
    // /// The default <see cref="VignetteParameters"/> of this <see cref="TunnelingVignetteController"/>.
    // /// </summary>
    // public VignetteParameters defaultParameters
    // {
    //     get => m_DefaultParameters;
    //     set => m_DefaultParameters = value;
    // }

    void Start()
    {
        // Disable automatic locomotion updates
        if (vignetteController != null)
            vignetteController.locomotionVignetteProviders.Clear();

        if (block == null)
            block = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (vignetteController == null)
            return;

        if (block == null)
            block = new MaterialPropertyBlock();

        var meshRenderer = vignetteController.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            return;

        //float finalAperture = enableVignette ? apertureSize : 1f;
        float finalAperture = GetApertureFromLevel(severityLevel);

        meshRenderer.GetPropertyBlock(block);
        block.SetFloat(_ApertureSize, finalAperture);
        //block.SetFloat(_FeatheringEffect, featheringEffect);
        block.SetFloat(_FeatheringEffect, GetFeatheringFromLevel(severityLevel));
        // block.SetColor(_VignetteColor, vignetteColor);
        // block.SetColor(_VignetteColorBlend, vignetteColorBlend);
        meshRenderer.SetPropertyBlock(block);

        // Apply vertical position offset
        var pos = vignetteController.transform.localPosition;
        // pos.y = verticalOffset;
        vignetteController.transform.localPosition = pos;
    }

    
    private float GetApertureFromLevel(int level)
    {
        switch (level)
        {
            case 0: return 1.0f;   // None → Full FOV
            case 1: return 0.75f;  // Low → ~120°–155° visible
            case 2: return 0.72f;  // Medium → ~58°–110° visible
            case 3: return 0.6f;  // High → ~36°–80° visible
            default: return 1.0f;
        }
    }

    private float GetFeatheringFromLevel(int level)
    {
        switch (level)
        {
            case 0: return 0f;    // None
            case 1: return 0.28f;  // Low: soft OFOV
            case 2: return 0.2f;  // Medium
            case 3: return 0.1f; // High: sharp OFOV
            default: return 0.2f;
        }
    }


}