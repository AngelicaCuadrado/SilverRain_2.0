using System;
using UnityEngine;

public class EnemyEdgeGlow : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private int buffLevel = 0;

    [Header("Rim Settings")]
    [SerializeField] private float rimPower = 4f;
    [SerializeField] private float rimIntensity = 1f;

    private MaterialPropertyBlock mpb;

    private static readonly int RimColorID = Shader.PropertyToID("_RimColor");
    private static readonly int RimPowerID = Shader.PropertyToID("_RimPower");
    private static readonly int RimIntensityID = Shader.PropertyToID("_RimIntensity");

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        ApplyBuffVisual();
    }

    private void OnEnable()
    {
        buffLevel = StageManager.Instance.CurrentStage;
    }

    public void ApplyBuffVisual()
    {
        Color rimColor = StageManager.Instance.GetStageEmissionColor();
        rimIntensity = 0f;

        switch (buffLevel)
        {
            case 1:
                rimIntensity = 0f;
                break;
            case 2:
                rimIntensity = 1.0f;
                break;
            case 3:
                rimIntensity = 1.2f;
                break;
            case 4:
                rimIntensity = 1.0f;
                break;
            case 5:
                rimIntensity = 1.2f;
                break;
            default:
                rimIntensity = 0f;
                break;
        }
        
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(mpb);
            mpb.SetColor(RimColorID, rimColor);
            mpb.SetFloat(RimPowerID, rimPower);
            mpb.SetFloat(RimIntensityID, rimIntensity);
            r.SetPropertyBlock(mpb);
        }
    }
}