using UnityEngine.UI;

namespace Witchcraft.Modules;

public class CustomImageAnimator : CustomAnimator
{
    public Image? Renderer { get; set; }

    protected override bool RendExists => Renderer != null;
    protected override Sprite Sprite { set => Renderer!.sprite = value; }

    protected override void SetRenderer() => Renderer = gameObject.GetComponent<Image>();
}