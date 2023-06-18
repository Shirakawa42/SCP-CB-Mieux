using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable, VolumeComponentMenu("Post-processing/Custom/FogEffect")]
public class CustomFogEffect : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter startDistance = new ClampedFloatParameter(35, 0, 100);
    public ClampedFloatParameter endDistance = new ClampedFloatParameter(40, 0, 100);
    public ColorParameter fogColor = new ColorParameter(Color.black, false, false, true);

    Material m_Material;

    public bool IsActive() => m_Material != null;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/DistanceFog") != null)
            m_Material = new Material(Shader.Find("Hidden/Shader/DistanceFog"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_StartDistance", startDistance.value);
        m_Material.SetFloat("_EndDistance", endDistance.value);
        m_Material.SetColor("_Color", fogColor.value);
        HDUtils.DrawFullScreen(cmd, m_Material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}
