using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ScreenShotter : MonoBehaviour
{
    public Texture2D CaptureScreenShot()
    {
        int width = Screen.width; int height = Screen.height;
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);
        Camera MainCamera = Camera.main;

        if (!MainCamera)
        {
            Debug.LogError(Constants.Camera_Not_Found);
            return null;
        }

        MainCamera.targetTexture = rt;
        RenderTexture.active = rt;
        MainCamera.Render();

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        MainCamera.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        Texture2D resizedScreenshot = ResizeTexture(screenshot, width / 6, height / 6);
        Destroy(screenshot);
        return resizedScreenshot;
    }

    private Texture2D ResizeTexture(Texture2D ORG, int w, int h)
    {
        RenderTexture rt = RenderTexture.GetTemporary(w, h, 24);
        RenderTexture.active = rt;
        Graphics.Blit(ORG, rt);

        Texture2D RSZ = new Texture2D(w, h, TextureFormat.RGB24, false);
        RSZ.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        RSZ.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return RSZ;
    }
}
