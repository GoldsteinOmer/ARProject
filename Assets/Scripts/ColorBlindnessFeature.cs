using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorBlindnessFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent passEvent = RenderPassEvent.AfterRendering;
    }

    public Settings settings = new Settings();
    private ColorBlindnessPass pass;

    public override void Create()
    {
        pass = new ColorBlindnessPass(settings.passEvent, settings.material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;

        pass.SetMaterial(settings.material);
        pass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(pass);
    }

    class ColorBlindnessPass : ScriptableRenderPass
    {
        private Material mat;
        private RTHandle source;

        public ColorBlindnessPass(RenderPassEvent evt, Material material)
        {
            renderPassEvent = evt;
            mat = material;
        }

        public void Setup(RTHandle cameraColorTarget)
        {
            source = cameraColorTarget;
        }

        public void SetMaterial(Material material)
        {
            mat = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (mat == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("ColorBlindnessFullScreen");
            Blitter.BlitCameraTexture(cmd, source, source, mat, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}