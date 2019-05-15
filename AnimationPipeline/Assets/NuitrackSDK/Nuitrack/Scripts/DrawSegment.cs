using UnityEngine;
using nuitrack;

public class DrawSegment : MonoBehaviour
{
    [SerializeField]
    Color32[] colorsList;

    Rect imageRect;

    [SerializeField]
    UnityEngine.UI.Image segmentOut;
    int renderStep = 2;
    Texture2D segmentTexture;
    Sprite segmentSprite;
    byte[] outSegment;


    int cols = 0;
    int rows = 0;

    void Start()
    {
        NuitrackManager.onUserTrackerUpdate += ColorizeUser;

        nuitrack.OutputMode mode = NuitrackManager.DepthSensor.GetOutputMode();
        cols = mode.XRes / renderStep;
        rows = mode.YRes/ renderStep;

        Debug.Log(cols);
        imageRect = new Rect(0, 0, cols, rows);

        segmentTexture = new Texture2D(cols, rows, TextureFormat.ARGB32, false);

        outSegment = new byte[cols * rows * 4];
    }

    void OnDestroy()
    {
        NuitrackManager.onUserTrackerUpdate -= ColorizeUser;
    }

    int userId;

    void ColorizeUser(nuitrack.UserFrame frame)
    {

        userId = CurrentUserTracker.CurrentUser;

        int pixelid = 0;
        int pointer = 0;

        for (int i = 0; i < (frame.Cols * frame.Rows); i+= renderStep)
        {
            Color32 currentColor = new Color32(0, 0, 0, 0);
            
            if (frame[i] == userId)
                currentColor = colorsList[frame[i]];

            int ptr = pixelid * 4;
            outSegment[ptr] = currentColor.a;
            outSegment[ptr + 1] = currentColor.r;
            outSegment[ptr + 2] = currentColor.g;
            outSegment[ptr + 3] = currentColor.b;
            pixelid++;
            pointer++;

            if (pointer == frame.Cols / renderStep)
            {
                i += frame.Cols;
                pointer = 0;
            }
        }

        segmentTexture.LoadRawTextureData(outSegment);
        segmentTexture.Apply();

        segmentSprite = Sprite.Create(segmentTexture, imageRect, UnityEngine.Vector3.one * 0.5f, 100f, 0, SpriteMeshType.FullRect);

        segmentOut.sprite = segmentSprite;
    }
}