namespace Witchcraft.Modules;

public class CustomRendAnimator : CustomAnimator
{
    public SpriteRenderer? Renderer { get; set; }

    public override void SetAnim(List<Sprite>? anim, float duration)
    {
        base.SetAnim(anim, duration);

        if (Anim is not { Count: > 0 })
            return;

        Renderer = gameObject.EnsureComponent<SpriteRenderer>();
        Renderer!.sprite = Anim[0];
    }

    public void Update()
    {
        if (!Renderer)
            return;

        FrameTime += Time.deltaTime;

        if (FrameTime < FrameDuration)
            return;

        FrameTime = 0f;
        FrameIndex = NumberUtils.CycleInt(Anim!.Count - 1, 0, FrameIndex, true);
        Renderer!.sprite = Anim[FrameIndex];
    }
}