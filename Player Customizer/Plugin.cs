using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player_Customizer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class PlayerCustomizer : BaseUnityPlugin
    {

        public enum MaterialEnum
        {
            Original,
            Matte,
            Metallic
        }

        private List<ConfigEntry<MaterialEnum>> Mat = new List<ConfigEntry<MaterialEnum>>();
        private List<ConfigEntry<string>> Paths = new List<ConfigEntry<string>>();
        private List<ConfigEntry<bool>> EnabledStatus = new List<ConfigEntry<bool>>();

        private List<Material> backupMat = new List<Material>();
        private List<Shader> backupShader = new List<Shader>();
        private List<Texture> backupTex = new List<Texture>();

        private List<MaterialEnum> lastMat = new List<MaterialEnum>();
        private List<string> lastPaths = new List<string>();
        private List<bool> lastStatus = new List<bool>();

        private void Awake()
        {
            try {
                if (!Directory.Exists(string.Format("{0}\\Mods\\Player customizer", Directory.GetCurrentDirectory())))
                {
                    Directory.CreateDirectory(string.Format("{0}\\Mods\\Player customizer", Directory.GetCurrentDirectory()));
                }

                EnabledStatus.Add(Config.Bind("", "Pot texture", false, new ConfigDescription("Whether the pot custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 4 })));
                EnabledStatus.Add(Config.Bind("", "Diogenes texture", false, new ConfigDescription("Whether the diogenes custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 3 })));
                EnabledStatus.Add(Config.Bind("", "Handle texture", false, new ConfigDescription("Whether the handle custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 2 })));
                EnabledStatus.Add(Config.Bind("", "Hammer tip texture", false, new ConfigDescription("Whether the tip custom texture should be enabled or not", null, new ConfigurationManagerAttributes { Order = 1 })));

                Paths.Add(Config.Bind("Textures", "Pot image path", string.Format("{0}\\Mods\\Player customizer\\pot.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for your pot", null, new ConfigurationManagerAttributes { Order = 4 })));
                Paths.Add(Config.Bind("Textures", "Dio image path", string.Format("{0}\\Mods\\Player customizer\\dio.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for diogenes", null, new ConfigurationManagerAttributes { Order = 3 })));
                Paths.Add(Config.Bind("Textures", "Handle image path", string.Format("{0}\\Mods\\Player customizer\\handle.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for your handle", null, new ConfigurationManagerAttributes { Order = 2 })));
                Paths.Add(Config.Bind("Textures", "Tip image path", string.Format("{0}\\Mods\\Player customizer\\tip.png", Directory.GetCurrentDirectory()), new ConfigDescription("This is the path to the image you will be using for your hammer tip", null, new ConfigurationManagerAttributes { Order = 1 })));

                Mat.Add(Config.Bind("Materials", "Pot material", MaterialEnum.Original, new ConfigDescription("Set the material of the pot", null, new ConfigurationManagerAttributes { Order = 3 })));
                Mat.Add(Config.Bind("Materials", "Diogenes material", MaterialEnum.Original, new ConfigDescription("Set the material of Diogenes", null, new ConfigurationManagerAttributes { Order = 2 })));
                Mat.Add(Config.Bind("Materials", "Handle material", MaterialEnum.Original, new ConfigDescription("Set the material of the handle", null, new ConfigurationManagerAttributes { Order = 1 })));
                Mat.Add(Config.Bind("Materials", "Hammer tip material", MaterialEnum.Original, new ConfigDescription("Set the material of the hammer tip", null, new ConfigurationManagerAttributes { Order = 0 })));

                backupMat.Clear(); backupShader.Clear(); backupTex.Clear();
                lastMat.AddMany(Mat[0].Value, Mat[1].Value, Mat[2].Value, Mat[3].Value);
                lastPaths.AddMany(Paths[0].Value, Paths[1].Value, Paths[2].Value, Paths[3].Value);
                lastStatus.AddMany(EnabledStatus[0].Value, EnabledStatus[1].Value, EnabledStatus[2].Value, EnabledStatus[3].Value);

                SceneManager.sceneLoaded += OnSceneLoaded;
            } catch (Exception ex) {Debug.LogException(ex); }
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        void Update()
        {
            try
            {
                if (GameObject.Find("Player"))
                {
                    Transform player = GameObject.Find("Player").transform;

                    if (EnabledStatus[0].Value != lastStatus[0]) {
                        MaterialLoader(player.Find("Pot/Mesh").gameObject, 0);
                    }
                    if (EnabledStatus[1].Value != lastStatus[1]) {
                        EyeLoader();
                        MaterialLoader(player.Find("dude/Body").gameObject, 1);
                    }
                    if (EnabledStatus[2].Value != lastStatus[2]) {
                        MaterialLoader(player.Find("handle/Mesh").gameObject, 2);
                    }
                    if (EnabledStatus[3].Value != lastStatus[3]) {
                        MaterialLoader(player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").gameObject, 3);
                    }
                    

                    if (Paths[0].Value != lastPaths[0])
                    {
                        MaterialLoader(player.Find("Pot/Mesh").gameObject, 0);
                    }
                    if (Paths[1].Value != lastPaths[1])
                    {
                        EyeLoader();
                        MaterialLoader(player.Find("dude/Body").gameObject, 1);
                    }
                    if (Paths[2].Value != lastPaths[2])
                    {
                        MaterialLoader(player.Find("handle/Mesh").gameObject, 2);
                    }
                    if (Paths[3].Value != lastPaths[3])
                    {
                        MaterialLoader(player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").gameObject, 3);
                    }
                    

                    if (Mat[0].Value != lastMat[0]) {
                        MaterialLoader(player.Find("Pot/Mesh").gameObject, 0);
                    }
                    if (Mat[1].Value != lastMat[1]) {
                        MaterialLoader(player.Find("dude/Body").gameObject, 1);
                    }
                    if (Mat[2].Value != lastMat[2]) {
                        MaterialLoader(player.Find("handle/Mesh").gameObject, 2);
                    }
                    if (Mat[3].Value != lastMat[3]) {
                        MaterialLoader(player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").gameObject, 3);
                    }

                }
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            try {
                if (scene.name == "Mian")
                {   
                    Transform player = GameObject.Find("Player").transform;

                    if (backupMat.Count == 0 || backupShader.Count == 0 || backupTex.Count == 0)
                    {
                        backupMat.AddMany(player.Find("Pot/Mesh").GetComponent<Renderer>().material, player.Find("dude/Body").GetComponent<Renderer>().material, player.Find("handle/Mesh").GetComponent<Renderer>().material, player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").GetComponent<Renderer>().material);
                        backupShader.AddMany(player.Find("Pot/Mesh").GetComponent<Renderer>().material.shader, player.Find("dude/Body").GetComponent<Renderer>().material.shader, player.Find("handle/Mesh").GetComponent<Renderer>().material.shader, player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").GetComponent<Renderer>().material.shader);
                        backupTex.AddMany(player.Find("Pot/Mesh").GetComponent<Renderer>().material.mainTexture, player.Find("dude/Body").GetComponent<Renderer>().material.mainTexture, player.Find("handle/Mesh").GetComponent<Renderer>().material.mainTexture, player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").GetComponent<Renderer>().material.mainTexture);
                    }

                    MaterialLoader(player.Find("Pot/Mesh").gameObject, 0);
                    MaterialLoader(player.Find("dude/Body").gameObject, 1);
                    MaterialLoader(player.Find("handle/Mesh").gameObject, 2);
                    MaterialLoader(player.Find("Hub/Slider/Handle/PoleMiddle/climbinghammer_remap/RetopoGroup1").gameObject, 3);
                    EyeLoader();
                } 
            } catch (Exception ex) { Debug.LogException(ex); }
        }

        void MaterialLoader(GameObject Object, int MaterialType)
        {
            switch (Mat[MaterialType].Value)
            {
                case MaterialEnum.Original:
                    Object.GetComponent<Renderer>().material = backupMat[MaterialType];
                    Object.GetComponent<Renderer>().material.shader = backupShader[MaterialType];
                    Object.GetComponent<Renderer>().material.mainTexture = backupTex[MaterialType];
                    lastMat[MaterialType] = Mat[MaterialType].Value;
                    if (EnabledStatus[MaterialType].Value && File.Exists(Paths[MaterialType].Value))
                    {
                        if (MaterialType == 0) { Object.GetComponent<Renderer>().material.SetFloat("_Goldness", 0f); }
                        Object.GetComponent<Renderer>().material.mainTexture = LoadPNG(Paths[MaterialType].Value);
                    }
                    lastStatus[MaterialType] = EnabledStatus[MaterialType].Value;
                    lastPaths[MaterialType] = Paths[MaterialType].Value;
                    break;

                case MaterialEnum.Matte:
                    Object.GetComponent<Renderer>().material = backupMat[MaterialType];
                    Object.GetComponent<Renderer>().material.shader = backupShader[MaterialType];
                    Object.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
                    Object.GetComponent<Renderer>().material.mainTexture = backupTex[MaterialType];
                    lastMat[MaterialType] = Mat[MaterialType].Value;
                    if (EnabledStatus[MaterialType].Value && File.Exists(Paths[MaterialType].Value))
                    {
                        Object.GetComponent<Renderer>().material.mainTexture = LoadPNG(Paths[MaterialType].Value);
                    }
                    lastStatus[MaterialType] = EnabledStatus[MaterialType].Value;
                    lastPaths[MaterialType] = Paths[MaterialType].Value;
                    break;

                case MaterialEnum.Metallic:
                    Object.GetComponent<Renderer>().material = backupMat[MaterialType];
                    Object.GetComponent<Renderer>().material.shader = backupShader[MaterialType];
                    Object.GetComponent<Renderer>().material.mainTexture = backupTex[MaterialType];
                    Object.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                    lastMat[MaterialType] = Mat[MaterialType].Value;
                    if (EnabledStatus[MaterialType].Value && File.Exists(Paths[MaterialType].Value))
                    {
                        Object.GetComponent<Renderer>().material.mainTexture = LoadPNG(Paths[MaterialType].Value);
                    }
                    lastStatus[MaterialType] = EnabledStatus[MaterialType].Value;
                    lastPaths[MaterialType] = Paths[MaterialType].Value;
                    break;
            }
        }

        void EyeLoader()
        {
            Renderer Eye = GameObject.Find("Player/dude/Eyes").GetComponent<Renderer>();

            Eye.material.mainTexture = backupTex[1];
            if (EnabledStatus[1].Value) {
                Eye.material.mainTexture = LoadPNG(Paths[1].Value);
            }
        }

        public static Texture2D LoadPNG(string filePath)
        {
            Texture2D texture2D = null;
            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);
                texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(data);
            }
            return texture2D;
        }

        void OnDisable()
        {
            try {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            } catch (Exception ex) { Debug.LogException(ex); }
        }
    }


    public static class ListExtensions
    {
        public static void AddMany<T>(this List<T> list, params T[] elements)
        {
            list.AddRange(elements);
        }
    }
}