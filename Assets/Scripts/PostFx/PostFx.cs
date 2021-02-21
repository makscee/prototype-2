// using System;
// using UnityEngine;
// using UnityEngine.Rendering.PostProcessing;
//  
// [Serializable]
// [PostProcess(typeof(PostFxRenderer), PostProcessEvent.AfterStack, "Custom/PostFx")]
// public sealed class PostFx : PostProcessEffectSettings
// {
//     [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
//     public FloatParameter blend = new FloatParameter { value = 0.5f };
//     
//     public override bool IsEnabledAndSupported(PostProcessRenderContext context)
//     {
//         return true;
//         return enabled.value
//                && blend.value > 0f;
//     }
// }
//  
// public sealed class PostFxRenderer : PostProcessEffectRenderer<PostFx>
// {
//     static readonly int ColorPrimary = Shader.PropertyToID("_ColorPrimary");
//     static readonly int Noise = Shader.PropertyToID("_Noise");
//     static readonly int Mask = Shader.PropertyToID("_Mask");
//     static readonly int Blend = Shader.PropertyToID("_Blend");
//     static readonly int Offset = Shader.PropertyToID("_Offset");
//     static readonly int ColorSecondary = Shader.PropertyToID("_ColorSecondary");
//
//     public override void Render(PostProcessRenderContext context)
//     {
//         Debug.Log($"render");
//         // return;
//         var sheet = context.propertySheets.Get(Shader.Find("Shader Graphs/stripes_custom"));
//         // sheet.properties.SetFloat(Blend, settings.blend);
//         // if (ShadowCamera.Instance != null)
//         // {
//         //     sheet.properties.SetTexture(Mask, ShadowCamera.Instance.renderTexture);
//         //     sheet.properties.SetTexture(Noise, NoiseGenerator.GetTexture());
//         //     // var primary = BlockEditor.ClusterPulseBlock.palette.colors[2];
//         //     // var secondary = BlockEditor.ClusterPulseBlock.palette.colors[1];
//         //     // sheet.properties.SetColor(ColorPrimary, primary);
//         //     // sheet.properties.SetColor(ColorSecondary, secondary);
//         //
//         // }
//
//         // sheet.properties.SetFloat(Offset, Time.time / 20);
//         context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
//     }
// }