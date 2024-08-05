using UnityEngine;
using System;

[Serializable]
public class MarkerResized {
    public Texture2D originalTexture;
    public Texture2D resizedTexture;
    public float scaleOriginal;

    public MarkerResized(Texture2D originalTexture, Vector2 standartSize) {
        this.originalTexture = originalTexture;

        float scale = 2400f / (float) Screen.height;
        standartSize = standartSize * scale;
        scaleOriginal = (standartSize.x / originalTexture.width) / scale;

        resizedTexture = Resize(originalTexture, (int) standartSize.x, (int) standartSize.y);
    }

    Texture2D Resize (Texture2D texture2D, int targetX, int targetY) {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }
}
