using UnityEngine;
using Valve.VR;

public class SceneFade : MonoBehaviour
{
    private Color currentColor = new Color(0, 0, 0, 0); // default starting color: black and fully transparent
    private Color targetColor = new Color(0, 0, 0, 0);  // default target color: black and fully transparent
    private Color deltaColor = new Color(0, 0, 0, 0);   // the delta-color is basically the "speed / second" at which the current color should change
    private bool fadeOverlay = false;

    static public void Start(Color newColor, float duration, bool fadeOverlay = false)
    {
        SteamVR_Events.Fade.Send(newColor, duration, fadeOverlay);
    }

    static public void View(Color newColor, float duration)
    {
        var compositor = OpenVR.Compositor;
        if (compositor != null) {
            /// ClearLastSubmittedFrame verbessert das Problem, löst dieses jedoch nicht
            compositor.ClearLastSubmittedFrame();
            compositor.FadeToColor(duration, newColor.r, newColor.g, newColor.b, newColor.a, false);
        }
    }

    public void OnStartFade(Color newColor, float duration, bool fadeOverlay)
    {
        if (duration > 0.0f)
        {
            targetColor = newColor;
            deltaColor = (targetColor - currentColor) / duration;
        }
        else
        {
            currentColor = newColor;
        }
    }

    static Material fadeMaterial = null;
    static int fadeMaterialColorID = -1;

    void OnEnable()
    {
        if (fadeMaterial == null)
        {
            fadeMaterial = new Material(Shader.Find("Custom/SteamVR_Fade"));
            fadeMaterialColorID = Shader.PropertyToID("fadeColor");
        }

        SteamVR_Events.Fade.Listen(OnStartFade);
        SteamVR_Events.FadeReady.Send();
    }

    void OnDisable()
    {
        SteamVR_Events.Fade.Remove(OnStartFade);
    }

    void OnPostRender()
    {
        if (currentColor != targetColor)
        {
            // if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
            if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.deltaTime)
            {
                currentColor = targetColor;
                deltaColor = new Color(0, 0, 0, 0);
            }
            else
            {
                currentColor += deltaColor * Time.deltaTime;
            }

            if (fadeOverlay)
            {
                var overlay = SteamVR_Overlay.instance;
                if (overlay != null)
                {
                    overlay.alpha = 1.0f - currentColor.a;
                }
            }
        }

        if (currentColor.a > 0 && fadeMaterial)
        {
            fadeMaterial.SetColor(fadeMaterialColorID, currentColor);
            fadeMaterial.SetPass(0);
            GL.Begin(GL.QUADS);

            GL.Vertex3(-1, -1, 0);
            GL.Vertex3(1, -1, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(-1, 1, 0);
            GL.End();
        }
    }
}