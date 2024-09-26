using UnityEngine.UI;

namespace Witchcraft.Gifs;

public class CustomAnimator : MonoBehaviour
{
    private List<Sprite>? Anim { get; set; }
    private Image? Renderer { get; set; }
    private int FrameIndex { get; set; }
    private float FrameDuration { get; set; }
    private float TotalDuration { get; set; }
    private float Time { get; set; }

    public void SetAnim(List<Sprite> anim, float duration)
    {
        if (anim != null && anim.Count > 0)
        {
            Anim = anim;
            Renderer = gameObject.GetComponent<Image>();
            FrameIndex = 0;
            Renderer.sprite = Anim[0];
            TotalDuration = duration;
            FrameDuration = duration / Anim.Count;
        }
    }

    public void Update()
    {
        if (!Renderer)
            return;

        Time += UnityEngine.Time.deltaTime;

        if (Time < FrameDuration)
            return;

        Time -= FrameDuration;

        if (FrameIndex < Anim!.Count - 1)
            FrameIndex++;
        else
            FrameIndex = 0;

        Renderer!.sprite = Anim[FrameIndex];
    }
}