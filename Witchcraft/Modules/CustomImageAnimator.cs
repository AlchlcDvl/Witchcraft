using UnityEngine.UI;

namespace Witchcraft.Modules;

public class CustomImageAnimator : CustomAnimator
{
    public Image? Renderer { get; set; }

    public override void SetAnim(List<Sprite>? anim, float duration)
    {
        base.SetAnim(anim, duration);

        if (Anim is not { Count: > 0 })
            return;

        Renderer = gameObject.EnsureComponent<Image>();
        Renderer!.sprite = Anim[0];
    }

    public void Update()
    {
        if (!Renderer || IsStatic)
            return;

        FrameTime += Time.deltaTime;

        if (FrameTime < FrameDuration)
            return;

        FrameTime = 0f;
        FrameIndex = NumberUtils.CycleInt(Anim!.Count - 1, 0, FrameIndex, true);
        Renderer!.sprite = Anim[FrameIndex];
    }
}