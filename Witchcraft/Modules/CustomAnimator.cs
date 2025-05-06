namespace Witchcraft.Modules;

public abstract class CustomAnimator : MonoBehaviour
{
    public List<Sprite>? Anim { get; set; }
    public int FrameIndex { get; set; }
    public float FrameDuration { get; set; }
    public float FrameTime { get; set; }

    public bool IsStatic => Anim is { Count: 1 };

    public virtual void SetAnim(List<Sprite>? anim, float duration)
    {
        Anim = anim;
        FrameIndex = 0;
        SetDuration(duration);
    }

    public void SetDuration(float duration)
    {
        if (Anim is { Count: > 0 })
            FrameDuration = duration / Anim.Count;
    }
}