namespace Witchcraft.Modules;

public abstract class CustomAnimator : MonoBehaviour
{
    public List<Sprite>? Anim { get; set; }
    public int FrameIndex { get; set; }
    public float FrameDuration { get; set; }
    public float FrameTime { get; set; }

    public bool IsStatic => Anim is null or { Count: 1 };

    protected abstract Sprite Sprite { set; }
    protected abstract bool RendExists { get; }

    public void SetAnim(List<Sprite>? anim, float duration)
    {
        Anim = anim;
        FrameIndex = 0;
        SetDuration(duration);

        if (Anim is not { Count: > 0 })
            return;

        SetRenderer();
        Sprite = Anim[0];
    }

    protected abstract void SetRenderer();

    public void SetDuration(float duration)
    {
        if (Anim is { Count: > 0 })
            FrameDuration = duration / Anim.Count;
    }

    public void Update()
    {
        if (IsStatic || !RendExists)
            return;

        FrameTime += Time.deltaTime;

        if (FrameTime < FrameDuration)
            return;

        FrameTime = 0f;
        FrameIndex = NumberUtils.CycleInt(Anim!.Count - 1, 0, FrameIndex, true);
        Sprite = Anim[FrameIndex];
    }
}