using System.Globalization;
using Newtonsoft.Json;

namespace Witchcraft.Managers;

public class AssetManager : BaseManager
{
    private Dictionary<string, AssetBundle> Bundles { get; } = [];
    private Dictionary<string, string> ObjectToBundle { get; } = [];
    private Dictionary<string, HashSet<UObject>> LoadedObjects { get; } = [];
    private Dictionary<string, HashSet<string>> UnloadedObjects { get; } = [];
    public Dictionary<string, string> Xmls { get; } = [];

    private Assembly Core { get; }
    private Action PostLoad { get; }
    private Action PostAllLoad { get; }
    private string[] BundleNames { get; }

    public static List<AssetManager> Managers { get; } = [];
    public static event Action? PostAllLoaded;

    public AssetManager(string name, WitchcraftMod mod, Action postLoad, Action postAllLoad, Assembly core, string[] bundles) : base(name, mod)
    {
        Core = core;
        PostLoad = postLoad;
        PostAllLoad = postAllLoad;
        BundleNames = bundles;
        PostAllLoaded += PostAllLoad;
        StoreAssetNames();
        Managers.Add(this);
    }

    private void StoreAssetNames()
    {
        foreach (var resourceName in Core.GetManifestResourceNames())
        {
            if (resourceName.EndsWithAny(".png", ".jpg", ".mp3", ".ogg", ".raw", ".wav", ".txt", ".log", ".json", ".gif"))
                AddPath(resourceName.SanitisePath(), resourceName);
        }
    }

    private void BeginLoading()
    {
        foreach (var resourceName in Core.GetManifestResourceNames())
        {
            var name1 = resourceName.SanitisePath();

            if (resourceName.EndsWithAny(".xml"))
                Xmls[name1] = LoadTextFromResources(resourceName).text;
            else if (!Bundles.ContainsKey(name1) && BundleNames.Contains(name1))
                RegisterBundle(LoadBundleFromResources(resourceName));
        }

        PostLoad();
    }

    public static void LoadAllAssets()
    {
        Managers.ForEach(x => x.BeginLoading());
        PostAllLoaded?.Invoke();
    }

    public void RegisterBundle(AssetBundle bundle)
    {
        Bundles[bundle.name] = bundle;
        bundle.GetAllAssetNames().ForEach(x => ObjectToBundle[ConvertToBaseName(x)] = bundle.name);
    }

    public T? Get<T>(string name, bool fetchPlaceholder = false) where T : UObject
    {
        if (LoadedObjects.TryGetValue(name, out var objList) && objList.TryFinding(x => x is T, out var result))
            return result as T;

        if (ObjectToBundle.TryGetValue(name.ToLower(CultureInfo.CurrentCulture), out var bundle))
            return LoadAsset<T>(Bundles[bundle], name);

        if (!UnloadedObjects.TryGetValue(name, out var strings))
            return null;

        var tType = typeof(T);

        if (tType == typeof(Sprite) && strings.TryFinding(x => x.EndsWithAny(".png", ".jpg"), out var path))
            result = AddAsset(name, LoadSpriteFromResources(path!));
        else if (tType == typeof(AudioClip) && strings.TryFinding(x => x.EndsWithAny(".mp3", ".ogg", ".raw"), out path))
            result = AddAsset(name, LoadAudioFromResources(path!));
        else if (tType == typeof(AudioClip) && strings.TryFinding(x => x.EndsWithAny(".wav"), out path))
            result = AddAsset(name, LoadWavAudioFromResources(path!));
        else if (tType == typeof(Texture2D) && strings.TryFinding(x => x.EndsWithAny(".png", ".jpg"), out path))
            result = AddAsset(name, LoadTextureFromResources(path!));
        else if (tType == typeof(TextAsset) && strings.TryFinding(x => x.EndsWithAny(".txt", ".log", ".json"), out path))
            result = AddAsset(name, LoadTextFromResources(path!));
        else if (tType == typeof(Gif) && strings.TryFinding(x => x.EndsWithAny(".gif"), out path))
            result = AddAsset(name, LoadGifFromResources(path!));
        else
        {
            if (name != "Placeholder" && fetchPlaceholder)
            {
                Mod.Debug($"Could not find {name} for type {tType.Name}, attempting to find placeholder");
                return Get<T>("Placeholder", true);
            }

            if (fetchPlaceholder)
                Mod.Debug($"No placeholder for type {tType.Name}");

            return null;
        }

        strings.Remove(path!);

        if (strings.Count == 0)
            UnloadedObjects.Remove(name);

        return result as T;
    }

    public IEnumerable<T> GetAll<T>() where T : UObject => LoadedObjects.Values.GetAll().OfType<T>();

    public T? LoadAsset<T>(AssetBundle assetBundle, string name) where T : UObject
    {
        var asset = assetBundle.LoadAsset<T>(name)?.DontDestroyOrUnload();
        AddAsset(name, asset);
        ObjectToBundle.Remove(name);

        if (!Bundles.Keys.Any(ObjectToBundle.Values.Contains))
        {
            Bundles.Remove(assetBundle.name);
            assetBundle.Unload(false);
        }

        return asset;
    }

    public T AddAsset<T>(string name, T? obj) where T : UObject
    {
        if (obj is null)
            return null!;

        if (!LoadedObjects.TryGetValue(name, out var value))
            LoadedObjects[name] = [ obj ];
        else
            value.Add(obj);

        return obj;
    }

    public void AddPath(string name, string? path)
    {
        if (path is null)
            return;

        if (!UnloadedObjects.TryGetValue(name, out var value))
            UnloadedObjects[name] = [ path ];
        else
            value.Add(path);
    }

    public static Texture2D LoadTextureFromDisk(string path) => LoadTexture(File.OpenRead(path), path.SanitisePath());

    public static Texture2D LoadTextureFromResources(string path, Assembly core) => LoadTexture(core.GetManifestResourceStream(path), path.SanitisePath());

    public static Texture2D LoadTextureFromResourcesStatic(string path) => LoadTextureFromResources(path, Assembly.GetCallingAssembly());

    public Texture2D LoadTextureFromResources(string path) => LoadTextureFromResources(path, Core);

    public static Texture2D EmptyTexture() => new(2, 2, TextureFormat.ARGB32, true)
    {
        filterMode = FilterMode.Bilinear,
        wrapMode = TextureWrapMode.Clamp
    };

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

    public AssetBundle LoadBundleFromResources(string path) => LoadBundle(path, StreamType.Resources, Core);

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

    public TextAsset LoadTextFromResources(string path) => LoadTextFromResources(path, Core);

    public static TextAsset LoadTextFromResourcesStatic(string path) => LoadTextFromResources(path, Assembly.GetCallingAssembly());

    public static TextAsset LoadTextFromResources(string path, Assembly core) => new(new StreamReader(core.GetManifestResourceStream(path)).ReadToEnd() ?? "Missing text");

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

    public string? GetString(string path, bool fetchPlaceholder = false) => Get<TextAsset>(path, fetchPlaceholder)?.text;

    public Sprite? GetSprite(string path, bool fetchPlaceholder = false) => Get<Sprite>(path, fetchPlaceholder);

    public AudioClip? GetAudio(string path, bool fetchPlaceholder = false) => Get<AudioClip>(path, fetchPlaceholder);

    public Gif? GetGif(string path, bool fetchPlaceholder = false) => Get<Gif>(path, fetchPlaceholder);

    public Material? GetMaterial(string path, bool fetchPlaceholder = false) => Get<Material>(path, fetchPlaceholder);

    public GameObject? GetGameObject(string path, bool fetchPlaceholder = false) => Get<GameObject>(path, fetchPlaceholder);

    public static Stream GetStream(string path, StreamType type, Assembly core = null!) => type switch
    {
        StreamType.Disk => File.OpenRead(path) ?? throw new FileNotFoundException(path),
        _ => core?.GetManifestResourceStream(path) ?? throw new FileNotFoundException(path)
    };

    public static Gif LoadGifFromResources(string path, Assembly assembly) => LoadGif(path.SanitisePath(), GetStream(path, StreamType.Resources, assembly));

    public Gif LoadGifFromResources(string path) => LoadGifFromResources(path, Core);

    public static Gif LoadGifFromDisk(string path) => LoadGif(path.SanitisePath(), GetStream(path, StreamType.Disk));

    public static Gif LoadGif(string name, Stream stream)
    {
        var gif = new Gif(name);
        gif.Load(stream);
        return gif;
    }

    public static byte[] GetBytes(string path, StreamType type, Assembly core = null!) => GetStream(path, type, core).ReadFully();

    public static T DeserializeJson<T>(string data) => JsonConvert.DeserializeObject<T>(data)!;

    public static T[] DeserializeArrayJson<T>(string data) => JsonConvert.DeserializeObject<T[]>(data)!;

    public static AssetManager? Assets<T>() => ModSingleton<T>.Instance?.Assets;

    // courtesy of pat, love ya mate
    public static TMP_SpriteAsset BuildGlyphs(IEnumerable<Sprite> sprites, string spriteAssetName, Dictionary<string, string> index)
    {
        var textures = sprites.Select(x => x.texture).ToArray();
        var asset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        var image = new Texture2D(2048, 2048) { name = spriteAssetName };
        var rects = image.PackTextures(textures, 2);

        for (var i = 0; i < rects.Length; i++)
        {
            var rect = rects[i];
            var tex = textures[i];

            var glyph = new TMP_SpriteGlyph()
            {
                glyphRect = new()
                {
                    x = (int)(rect.x * image.width),
                    y = (int)(rect.y * image.height),
                    width = (int)(rect.width * image.width),
                    height = (int)(rect.height * image.height),
                },
                metrics = new()
                {
                    width = tex.width,
                    height = tex.height,
                    horizontalBearingY = tex.width * 0.75f,
                    horizontalBearingX = 0,
                    horizontalAdvance = tex.width
                },
                index = (uint)i,
                sprite = sprites.ElementAtOrDefault(i),
                scale = 1f
            };

            asset.spriteGlyphTable.Add(glyph);
            asset.spriteCharacterTable.Add(new(0, asset, glyph)
            {
                name = index[glyph.sprite.name],
                glyphIndex = (uint)i,
                scale = 1f
            });
        }

        asset.name = spriteAssetName;
        asset.material = new(Shader.Find("TextMeshPro/Sprite"));
        asset.version = "1.1.0";
        asset.spriteSheet = image;
        asset.UpdateLookupTables();
        asset.hashCode = TMP_TextUtilities.GetSimpleHashCode(asset.name);
        asset.material.SetTexture(ShaderUtilities.ID_MainTex, asset.spriteSheet);
        return asset.DontDestroyOrUnload();
    }

    public static AudioClip LoadWavAudioFromResources(string path, Assembly core) => LoadWavAudio(path.SanitisePath(), core.GetManifestResourceStream(path).ReadFully());

    public static AudioClip LoadWavAudioFromResourcesStatic(string path) => LoadWavAudioFromResources(path, Assembly.GetCallingAssembly());

    public AudioClip LoadWavAudioFromResources(string path) => LoadWavAudioFromResources(path, Core);

    public static AudioClip LoadWavAudio(string name, byte[] fileBytes)
    {
        var chunk = BitConverter.ToInt32(fileBytes, 16) + 24;
        var channels = BitConverter.ToUInt16(fileBytes, 22);
        var sampleRate = BitConverter.ToInt32(fileBytes, 24);
        var bitDepth = BitConverter.ToUInt16(fileBytes, 34);
        var wavSize = BitConverter.ToInt32(fileBytes, chunk);
        var data = bitDepth switch
        {
            8 => Convert8BitByteArrayToAudioClipData(fileBytes, wavSize),
            16 => Convert16BitByteArrayToAudioClipData(fileBytes, chunk, wavSize),
            24 => Convert24BitByteArrayToAudioClipData(fileBytes, chunk, wavSize),
            32 => Convert32BitByteArrayToAudioClipData(fileBytes, chunk, wavSize),
            _ => throw new(bitDepth + " bit depth is not supported."),
        };

        var audioClip = AudioClip.Create(name, data.Length, channels, sampleRate, false);
        audioClip.SetData(data, 0);
        audioClip.hideFlags |= HideFlags.DontSaveInEditor;
        return audioClip.DontDestroyOrUnload();
    }

    private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int wavSize)
    {
        var data = new float[wavSize];

        for (var i = 0; i < wavSize; i++)
            data[i] = (float)source[i] / sbyte.MaxValue;

        return data;
    }

    private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int wavSize)
    {
        headerOffset += sizeof(int);
        const int x = sizeof(short);
        var convertedSize = wavSize / x;
        var data = new float[convertedSize];

        for (var i = 0; i < convertedSize; i++)
            data[i] = (float)BitConverter.ToInt16(source, (i * x) + headerOffset) / short.MaxValue;

        return data;
    }

    private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int wavSize)
    {
        const int intSize = sizeof(int);
        headerOffset += intSize;
        var convertedSize = wavSize / 3;
        var data = new float[convertedSize];
        var block = new byte[intSize]; // Using a 4-byte block for copying 3 bytes, then copy bytes with 1 offset

        for (var i = 0; i < convertedSize; i++)
        {
            Buffer.BlockCopy(source, (i * 3) + headerOffset, block, 1, 3);
            data[i] = (float)BitConverter.ToInt32(block, 0) / int.MaxValue;
        }

        return data;
    }

    private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int wavSize)
    {
        headerOffset += sizeof(int);
        var convertedSize = wavSize / 4;
        var data = new float[convertedSize];

        for (var i = 0; i < convertedSize; i++)
            data[i] = (float)BitConverter.ToInt32(source, (i * 4) + headerOffset) / int.MaxValue;

        return data;
    }
}

public enum StreamType
{
    Disk,
    Resources
}

[AttributeUsage(AttributeTargets.Method)]
public class UponAssetsLoadedAttribute : Attribute;

[AttributeUsage(AttributeTargets.Method)]
public class UponAllAssetsLoadedAttribute : Attribute;