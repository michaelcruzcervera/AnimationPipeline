using UnityEngine;
using System.IO;

public class DepthCapture
{
    public static Texture2D capture(Rect pRect)
    {
        Camera lCamera = Camera.main;
        Texture2D image;
        var lPreClearFlags = lCamera.clearFlags;
        var lPreBackgroundColor = lCamera.backgroundColor;
        {
            lCamera.clearFlags = CameraClearFlags.Color;
            
            //Capture Normals
            lCamera.depthTextureMode = DepthTextureMode.DepthNormals;
            lCamera.Render();
            var temp = captureView(pRect);
            image = temp;
        }
        return image;
    }

    /// <summary>
    /// Capture a screenshot(not include GUI)
    /// </summary>
    /// <returns></returns>
    public static Texture2D captureScreenshot()
    {
        return capture(new Rect(0f, 0f, Screen.width, Screen.height));
    }

    /// <summary>
    /// Capture a screenshot(not include GUI) at path filename as a PNG file
    /// eg. zzTransparencyCapture.captureScreenshot("Screenshot.png")
    /// </summary>
    /// <param name="pFileName"></param>
    /// <returns></returns>
    public static void captureScreenshot(string pFileName)
    {
        var lScreenshot = captureScreenshot();
        try
        {
            using (var lFile = new FileStream(pFileName, FileMode.Create))
            {
                BinaryWriter lWriter = new BinaryWriter(lFile);
                lWriter.Write(lScreenshot.EncodeToPNG());
            }
        }
        finally
        {
            Object.DestroyImmediate(lScreenshot);
        }
    }
    static Texture2D captureView(Rect pRect)
    {
        Texture2D image = new Texture2D((int)pRect.width, (int)pRect.height, TextureFormat.ARGB32, false);
        image.ReadPixels(pRect, 0, 0, false);
        return image;
    }

}
