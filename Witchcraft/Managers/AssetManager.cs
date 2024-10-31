using System.Globalization;
using Witchcraft.Gifs;

namespace Witchcraft.Managers;

public class AssetManager
{
    public Dictionary<string, AssetBundle> Bundles { get; set; } = [];
    public Dictionary<string, string> ObjectToBundle { get; set; } = [];
    public Dictionary<string, List<UObject>> UnityLoadedObjects { get; set; } = [];
    public Dictionary<string, List<object>> SystemLoadedObjects { get; set; } = [];
    public Dictionary<string, string> Xmls { get; set; } = [];

    public string Name { get; }
    private Assembly Core { get; }
    private Action PostLoad { get; }
    private string[] BundleNames { get; }
    public LogManager AssetLogs { get; }

    public static List<AssetManager> Managers { get; set; } = [];

    public AssetManager(string name, Action postLoad, Assembly core, string[] bundles, LogManager logs)
    {
        Name = name;
        Core = core;
        PostLoad = postLoad;
        BundleNames = bundles;
        AssetLogs = logs;
        Managers.Add(this);
    }

    public void BeginLoading()
    {
        foreach (var resourceName in Core.GetManifestResourceNames())
        {
            var name1 = resourceName.SanitisePath();

            if (resourceName.EndsWithAny([".png", ".jpg"]))
                AddUnityAsset(name1, LoadSpriteFromResources(resourceName));
            else if (resourceName.EndsWithAny([".mp3", ".wav", ".ogg", ".raw"]))
                AddUnityAsset(name1, LoadAudioFromResources(resourceName));
            else if (resourceName.EndsWithAny([".txt", ".log"]))
                AddSystemAsset(name1, LoadTextFromResources(resourceName));
            else if (resourceName.EndsWithAny([".gif"]))
                AddSystemAsset(name1, LoadGifFromResources(resourceName));
            else if (resourceName.EndsWithAny([".xml"]))
                Xmls[name1] = LoadTextFromResources(resourceName);
            else if (!Bundles.ContainsKey(name1) && BundleNames.Contains(name1))
                Bundles[name1] = LoadBundleFromResources(resourceName);
        }

        PostLoad();
        AssetLogs.Message("Assets loaded!");
    }

    public T? UnityGet<T>(string name, bool fetchPlaceholder = false) where T : UObject
    {
        var tType = typeof(T);

        if (UnityLoadedObjects.TryGetValue(name, out var objList) && objList.TryFinding(x => x is T, out var result))
            return result as T;

        if (ObjectToBundle.TryGetValue(name.ToLower(CultureInfo.CurrentCulture), out var bundle))
            return LoadAsset<T>(Bundles[bundle], name);

        if (name != "Placeholder" && fetchPlaceholder)
        {
            AssetLogs.Debug($"Could not find {name} for type {tType.Name}, attempting to find placeholder");
            return UnityGet<T>("Placeholder", true);
        }

        if (fetchPlaceholder)
            AssetLogs.Debug($"No placholder for type {tType.Name}");

        return null;
    }

    public T? SystemGet<T>(string name, bool fetchPlaceholder = false)
    {
        var tType = typeof(T);

        if (SystemLoadedObjects.TryGetValue(name, out var objList) && objList.TryFinding(x => x is T, out var result))
            return (T)result!;

        if (name != "Placeholder" && fetchPlaceholder)
        {
            AssetLogs.Debug($"Could not find {name} for type {tType.Name}, attempting to find placeholder");
            return SystemGet<T>("Placeholder", true);
        }

        if (fetchPlaceholder)
            AssetLogs.Debug($"No placholder for type {tType.Name}");

        return default;
    }

    public List<T> UnityGetAll<T>() where T : UObject
    {
        var result = new List<T>();

        foreach (var (_, objList) in UnityLoadedObjects)
        {
            foreach (var obj in objList)
            {
                if (obj is T t)
                    result.Add(t);
            }
        }

        return result;
    }

    public List<T> SystemGetAll<T>()
    {
        var result = new List<T>();

        foreach (var (_, objList) in SystemLoadedObjects)
        {
            foreach (var obj in objList)
            {
                if (obj is T t)
                    result.Add(t);
            }
        }

        return result;
    }

    public T? LoadAsset<T>(AssetBundle assetBundle, string name) where T : UObject
    {
        var asset = assetBundle.LoadAsset<T>(name)?.DontDestroyOrUnload();
        AddUnityAsset(name, asset);
        ObjectToBundle.Remove(name);

        if (!Bundles.Keys.Any(ObjectToBundle.Values.Contains))
            Bundles.Remove(assetBundle.name);

        return asset;
    }

    public void AddUnityAsset(string name, UObject? obj)
    {
        if (obj is null)
            return;

        if (!UnityLoadedObjects.TryGetValue(name, out var value))
            UnityLoadedObjects[name] = [ obj ];
        else if (!value.Contains(obj))
            value.Add(obj);
    }

    public void AddSystemAsset(string name, object? obj)
    {
        if (obj is null)
            return;

        if (!SystemLoadedObjects.TryGetValue(name, out var value))
            SystemLoadedObjects[name] = [ obj ];
        else if (!value.Contains(obj))
            value.Add(obj);
    }

    public static Texture2D LoadTextureFromDisk(string path) => LoadTexture(File.OpenRead(path), path.SanitisePath());

    public static Texture2D LoadTextureFromResources(string path, Assembly core) => LoadTexture(core.GetManifestResourceStream(path), path.SanitisePath());

    public static Texture2D LoadTextureFromResourcesStatic(string path) => LoadTextureFromResources(path, Assembly.GetCallingAssembly());

    public Texture2D LoadTextureFromResources(string path) => LoadTextureFromResources(path, Core);

    public static Texture2D EmptyTexture() => new(2, 2, TextureFormat.ARGB32, true);

    public static Texture2D LoadTexture(Stream stream, string name)
    {
        var data = stream.ReadFully();
        var texture = EmptyTexture();
        texture.LoadImage(data, false);
        texture.name = name;
        return texture.DontDestroyOrUnload();
    }

    public static Sprite CreateSprite(Texture2D tex, float ppu = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        var sprite = Sprite.Create(tex, new(0, 0, tex.width, tex.height), new(0.5f, 0.5f), ppu, 0, spriteType);
        sprite.name = tex.name;
        return sprite.DontDestroyOrUnload();
    }

    public static Sprite LoadSpriteFromDisk(string path, float ppu = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight) => CreateSprite(LoadTextureFromDisk(path), ppu, spriteType);

    public static Sprite LoadSpriteFromResources(string path, Assembly core, float ppu = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight) => CreateSprite(LoadTextureFromResources(path,
        core), ppu, spriteType);

    public static Sprite LoadSpriteFromResourcesStatic(string path, float ppu = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight) => LoadSpriteFromResources(path,
        Assembly.GetCallingAssembly(), ppu, spriteType);

    public Sprite LoadSpriteFromResources(string path, float ppu = 100f, SpriteMeshType spriteType = SpriteMeshType.Tight) => LoadSpriteFromResources(path, Core, ppu, spriteType);

    public static AudioClip LoadAudioFromDisk(string path) => CreateAudio(path.SanitisePath(), File.ReadAllBytes(path));

    public static AudioClip LoadAudioFromResources(string path, Assembly core) => CreateAudio(path.SanitisePath(), core.GetManifestResourceStream(path).ReadFully());

    public static AudioClip LoadAudioFromResourcesStatic(string path) => LoadAudioFromResources(path, Assembly.GetCallingAssembly());

    public AudioClip LoadAudioFromResources(string path) => LoadAudioFromResources(path, Core);

    public static AudioClip CreateAudio(string name, byte[] data)
    {
        var samples = new float[data.Length / 4];

        for (var i = 0; i < samples.Length; i++)
            samples[i] = (float)BitConverter.ToInt32(data, i * 4) / int.MaxValue;

        var audioClip = AudioClip.Create(name, samples.Length / 2, 2, 48000, false);
        audioClip.SetData(samples, 0);
        return audioClip.DontDestroyOrUnload();
    }

    public AssetBundle LoadBundleFromResources(string path)
    {
        var bundle = LoadBundle(path, StreamType.Resources, Core);
        bundle.GetAllAssetNames().ForEach(x => ObjectToBundle[ConvertToBaseName(x)] = bundle.name);
        return bundle;
    }

    public static AssetBundle LoadBundleFromResources(string path, Assembly core) => LoadBundle(path, StreamType.Resources, core);

    public static AssetBundle LoadBundleFromResourcesStatic(string path) => LoadBundleFromResources(path, Assembly.GetCallingAssembly());

    public static AssetBundle LoadBundleFromDisk(string path) => LoadBundle(path, StreamType.Disk);

    public static AssetBundle LoadBundle(string path, StreamType type, Assembly core = null!)
    {
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            path += "_mac";
        else if (path.EndsWith("_mac", StringComparison.OrdinalIgnoreCase))
            path = path.Replace("_mac", string.Empty);

        var bundle = AssetBundle.LoadFromMemory(GetStream(path, type, core).ReadFully());
        bundle.name = ConvertToBaseName(path.SanitisePath());
        return bundle.DontDestroyOrUnload();
    }

    public string LoadTextFromResources(string path) => LoadTextFromResources(path, Core);

    public static string LoadTextFromResourcesStatic(string path) => LoadTextFromResources(path, Assembly.GetCallingAssembly());

    public static string LoadTextFromResources(string path, Assembly core) => new StreamReader(core.GetManifestResourceStream(path)).ReadToEnd() ?? "Missing text";

    /// <remarks>https://stackoverflow.com/questions/51315918/how-to-encodetopng-compressed-textures-in-unity courtesy of pat from SalemModLoader.</remarks>
    public static Texture2D Decompress(Texture2D source)
    {
        var renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(source, renderTex);
        var previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        var readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        readableText.name = source.name;
        return readableText.DontDestroyOrUnload();
    }

    public static string ConvertToBaseName(string name) => name.Split('/')[^1].Split('.')[0];

    public string? GetString(string path, bool fetchPlaceholder = false) => SystemGet<string>(path, fetchPlaceholder);

    public Sprite? GetSprite(string path, bool fetchPlaceholder = false) => UnityGet<Sprite>(path, fetchPlaceholder);

    public AudioClip? GetAudio(string path, bool fetchPlaceholder = false) => UnityGet<AudioClip>(path, fetchPlaceholder);

    public Gif? GetGif(string path, bool fetchPlaceholder = false) => SystemGet<Gif>(path, fetchPlaceholder);

    public Material? GetMaterial(string path, bool fetchPlaceholder = false) => UnityGet<Material>(path, fetchPlaceholder);

    public GameObject? GetGameObject(string path, bool fetchPlaceholder = false) => UnityGet<GameObject>(path, fetchPlaceholder);

    public static Stream GetStream(string path, StreamType type, Assembly core = null!) => type switch
    {
        StreamType.Disk => File.OpenRead(path) ?? throw new FileNotFoundException(path),
        _ => core?.GetManifestResourceStream(path) ?? throw new FileNotFoundException(path)
    };

    public static Gif LoadGifFromResources(string path, Assembly assembly) => LoadGif(assembly.GetManifestResourceStream(path).ReadFully());

    public static Gif LoadGifFromResourcesStatic(string path) => LoadGifFromResources(path, Assembly.GetCallingAssembly());

    public Gif LoadGifFromResources(string path) => LoadGifFromResources(path, Core);

    public static Gif LoadGifFromDisk(string path) => LoadGif(File.ReadAllBytes(path));

    public static Gif LoadGif(byte[] data)
    {
        var result = new List<Sprite>();
        var gifData = GifLoader.SetGifData(data);

        if (gifData == null || gifData.ImageBlockList == null || gifData.ImageBlockList.Count < 1)
            return null!;

        var width = gifData.ScreenWidth;
        var height = gifData.ScreenHeight;
        var imgIndex = 0;
        var rawTextures = new List<Color32[]>();

        foreach (var img in gifData.ImageBlockList)
        {
            var decodedData = GifLoader.GetDecodedData(img);
            var graphicCtrlEx = GifLoader.GetGraphicCtrlExt(gifData, imgIndex);
            var transparentIndex = GifLoader.GetTransparentIndex(graphicCtrlEx);

            var colorTable = GifLoader.GetColorTableAndSetBgColor(gifData, img, transparentIndex, out var bgColor);
            rawTextures.Add(GifLoader.GetTextureData(decodedData, bgColor, colorTable, transparentIndex, img));
            imgIndex++;
        }

        for (var i = 0; i < rawTextures.Count; i++)
        {
            var gifFrame = new Texture2D(width, height, TextureFormat.ARGB32, false, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            };
            gifFrame.SetPixels32(rawTextures[i], 0);
            gifFrame.Apply();

            result.Add(Sprite.Create(gifFrame, new(0, 0, width, height), new(0.5f, 0.5f), 100));
        }

        return new(result);
    }
}

public enum StreamType
{
    Disk,
    Resources
}