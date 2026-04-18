using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Rendering;

namespace Sackrany.SerializableData
{
    internal static class ThumbnailCapture
    {
        const int LongSideTarget = 200;
        const int JpegQuality = 65;

        internal static async UniTask<byte[]> Capture()
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            var source = ScreenCapture.CaptureScreenshotAsTexture();
            var bytes = Resize(source);
            Object.Destroy(source);

            return bytes;
        }

        static byte[] Resize(Texture2D source)
        {
            int w = source.width;
            int h = source.height;

            float scale = w >= h
                ? (float)LongSideTarget / w
                : (float)LongSideTarget / h;

            int tw = Mathf.RoundToInt(w * scale);
            int th = Mathf.RoundToInt(h * scale);

            var resized = new Texture2D(tw, th, TextureFormat.RGB24, false);

            for (int y = 0; y < th; y++)
            {
                float v = (th == 1) ? 0f : (float)y / (th - 1);
                for (int x = 0; x < tw; x++)
                {
                    float u = (tw == 1) ? 0f : (float)x / (tw - 1);
                    Color c = source.GetPixelBilinear(u, v);
                    resized.SetPixel(x, y, c);
                }
            }

            resized.Apply();

            byte[] bytes = resized.EncodeToJPG(JpegQuality);
            Object.Destroy(resized);

            return bytes;
        }
    }
}
