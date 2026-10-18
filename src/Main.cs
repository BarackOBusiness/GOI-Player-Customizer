using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerCustomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin {
    public enum MaterialType {
        Original,
        Matte,
        Metallic
    }

    private ConfigEntry<float> PotGoldness;

    private List<ConfigEntry<MaterialType>> Mat = new List<ConfigEntry<MaterialType>>();
    private List<ConfigEntry<string>> Paths = new List<ConfigEntry<string>>();
    private List<ConfigEntry<bool>> EnabledStatus = new List<ConfigEntry<bool>>();

    private List<Material> backupMat = new List<Material>();
    private List<Shader> backupShader = new List<Shader>();
    private List<Texture> backupTex = new List<Texture>();

    private void Awake() {
        try {
            if (!Directory.Exists(string.Format("{0}\\Mods\\Player customizer", Directory.GetCurrentDirectory()))) {
                Directory.CreateDirectory(string.Format("{0}\\Mods\\Player customizer", Directory.GetCurrentDirectory()));
            }

            // For future iterations, combine the path and enablement config entries
            EnabledStatus.Add(Config.Bind("", "Pot texture", false, new ConfigDescription("Whether the pot custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 4 })));
            EnabledStatus.Add(Config.Bind("", "Diogenes texture", false, new ConfigDescription("Whether the diogenes custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 3 })));
            EnabledStatus.Add(Config.Bind("", "Handle texture", false, new ConfigDescription("Whether the handle custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 2 })));
            EnabledStatus.Add(Config.Bind("", "Hammer tip texture", false, new ConfigDescription("Whether the tip custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 1 })));

            Paths.Add(Config.Bind("Textures", "Pot image path", string.Format("{0}\\Mods\\Player customizer\\pot.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for your pot", null, new ConfigurationManagerAttributes { Order = 4 })));
            Paths.Add(Config.Bind("Textures", "Dio image path", string.Format("{0}\\Mods\\Player customizer\\dio.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for diogenes", null, new ConfigurationManagerAttributes { Order = 3 })));
            Paths.Add(Config.Bind("Textures", "Handle image path", string.Format("{0}\\Mods\\Player customizer\\handle.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for your handle", null, new ConfigurationManagerAttributes { Order = 2 })));
            Paths.Add(Config.Bind("Textures", "Tip image path", string.Format("{0}\\Mods\\Player customizer\\tip.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for your hammer tip", null, new ConfigurationManagerAttributes { Order = 1 })));

            PotGoldness = Config.Bind("Materials", "Pot Goldness", 0.0f, new ConfigDescription("Set the goldness of the pot", new AcceptableValueRange<float>(0.0f, 1.0f)));
            Mat.Add(Config.Bind("Materials", "Pot material", MaterialType.Original, new ConfigDescription("Set the material of the pot", null, new ConfigurationManagerAttributes { Order = 3 })));
            Mat.Add(Config.Bind("Materials", "Diogenes material", MaterialType.Original, new ConfigDescription("Set the material of Diogenes", null, new ConfigurationManagerAttributes { Order = 2 })));
            Mat.Add(Config.Bind("Materials", "Handle material", MaterialType.Original, new ConfigDescription("Set the material of the handle", null, new ConfigurationManagerAttributes { Order = 1 })));
            Mat.Add(Config.Bind("Materials", "Hammer tip material", MaterialType.Original, new ConfigDescription("Set the material of the hammer tip", null, new ConfigurationManagerAttributes { Order = 0 })));

            backupMat.Clear(); backupShader.Clear(); backupTex.Clear();
        }
        catch (Exception ex) { Debug.LogException(ex); }
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        try {
            if (scene.name == "Mian") {
                Transform player = GameObject.Find("Player").transform;

                // I believe any of these conditions should only be met if all
                // of them would be met, so potential for cleanup here
                if (backupMat.Count == 0 || backupShader.Count == 0 || backupTex.Count == 0) {
                    backupMat.AddMany(player.Find("Pot/Mesh").GetComponent<Renderer>().material, player.Find("dude/Body").GetComponent<Renderer>().material, player.Find("handle/Mesh").GetComponent<Renderer>().material, player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").GetComponent<Renderer>().material);
                    backupShader.AddMany(player.Find("Pot/Mesh").GetComponent<Renderer>().material.shader, player.Find("dude/Body").GetComponent<Renderer>().material.shader, player.Find("handle/Mesh").GetComponent<Renderer>().material.shader, player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").GetComponent<Renderer>().material.shader);
                    backupTex.AddMany(player.Find("Pot/Mesh").GetComponent<Renderer>().material.mainTexture, player.Find("dude/Body").GetComponent<Renderer>().material.mainTexture, player.Find("handle/Mesh").GetComponent<Renderer>().material.mainTexture, player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").GetComponent<Renderer>().material.mainTexture);
                }

                // Disgusting delegation of responsibilities here,
                // texture existence checking should occur elsewhere and block
                // the flow to texture assignment for the corresponding parts
                MaterialLoader(player.Find("Pot/Mesh").gameObject, 0);
                MaterialLoader(player.Find("dude/Body").gameObject, 1);
                MaterialLoader(player.Find("handle/Mesh").gameObject, 2);
                MaterialLoader(player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").gameObject, 3);
                EyeLoader();
            }
        }
        catch (Exception ex) { Debug.LogException(ex); }
    }

    void MaterialLoader(GameObject gameObj, int materialType) {
        Renderer renderer = gameObj.GetComponent<Renderer>();
        bool useCustomTexture = EnabledStatus[materialType].Value && File.Exists(Paths[materialType].Value);

        switch (Mat[materialType].Value) {
            // The Matte case is commented for future reference
            case MaterialType.Original:
                renderer.material = backupMat[materialType];
                renderer.material.shader = backupShader[materialType];
                renderer.material.mainTexture = backupTex[materialType];
                if (useCustomTexture) {
                    // Use a configured pot goldness instead of just setting it to 0
                    if (materialType == 0) { renderer.material.SetFloat("_Goldness", PotGoldness.Value); }
                    renderer.material.mainTexture = LoadPNG(Paths[materialType].Value);
                }
                break;
            case MaterialType.Matte:
                // First reset the item to its original material and shader
                renderer.material = backupMat[materialType];
                renderer.material.shader = backupShader[materialType];
                // Generate a new material
                renderer.material = new Material(Shader.Find("Standard"));
                // Assign its original texture, in case the one selected is not found
                renderer.material.mainTexture = backupTex[materialType];
                if (useCustomTexture) {
                    // Assign the selected texture to the object's new material
                    renderer.material.mainTexture = LoadPNG(Paths[materialType].Value);
                }
                break;
            case MaterialType.Metallic:
                renderer.material = backupMat[materialType];
                renderer.material.shader = backupShader[materialType];
                renderer.material.mainTexture = backupTex[materialType];
                renderer.material.shader = Shader.Find("Standard");
                if (useCustomTexture) {
                    renderer.material.mainTexture = LoadPNG(Paths[materialType].Value);
                }
                break;
        }
        // Higher quality texture filtering than default
        renderer.material.mainTexture.filterMode = FilterMode.Trilinear;
        // This doesn't take effect unless globally anisotropic filtering
        // is enabled, so depends on quality setting
        renderer.material.mainTexture.anisoLevel = 8;
    }

    void EyeLoader() {
        Renderer Eye = GameObject.Find("Player/dude/Eyes").GetComponent<Renderer>();

        Eye.material.mainTexture = backupTex[1];
        if (EnabledStatus[1].Value && File.Exists(Paths[1].Value)) {
            Eye.material.mainTexture = LoadPNG(Paths[1].Value);
        }
    }

    public static Texture2D LoadPNG(string filePath) {
        Texture2D texture2D = null;
        if (File.Exists(filePath)) {
            byte[] data = File.ReadAllBytes(filePath);
            texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(data);
        }
        return texture2D;
    }
}

public static class ListExtensions
{
    public static void AddMany<T>(this List<T> list, params T[] elements) {
        list.AddRange(elements);
    }
}
