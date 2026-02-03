namespace Witchcraft.Modules;

public class CustomRendAnimator : CustomAnimator
{
    public SpriteRenderer? Renderer { get; set; }

    protected override bool RendExists => Renderer != null;
    protected override Sprite Sprite { set => Renderer!.sprite = value; }

    protected override void SetRenderer() => Renderer = gameObject.GetComponent<SpriteRenderer>();
}