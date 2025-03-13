using UnityEngine.UI;

namespace Witchcraft.Modules;

public class CustomAnimator : MonoBehaviour
{
    public List<Sprite>? Anim { get; set; }
    public Image? Renderer { get; set; }
    public int FrameIndex { get; set; }
    public float FrameDuration { get; set; }
    public float Time { get; set; }

    public void SetAnim(List<Sprite>? anim, float duration)
    {
        if (anim is not { Count: > 0 })
            return;

        Anim = anim;
        Renderer = gameObject.EnsureComponent<Image>();
        FrameIndex = 0;
        Renderer!.sprite = Anim[0];
        SetDuration(duration);
    }

    public void SetDuration(float duration)
    {
        if (Anim is { Count: > 0 })
            FrameDuration = duration / Anim.Count;
    }

    public void Update()
    {
        if (!Renderer)
            return;

        Time += UnityEngine.Time.deltaTime;

        if (Time < FrameDuration)
            return;

        Time = 0f;
        FrameIndex = NumberUtils.CycleInt(Anim!.Count - 1, 0, FrameIndex, true);
        Renderer!.sprite = Anim[FrameIndex];
    }
}