using UnityEngine;
using HarmonyLib;
using BepInEx;
using UnityEngine.SceneManagement;
using System;
using System.Timers;

[ContentWarningPlugin("com.cowari.myplugin", "1.0.0", vanillaCompatible: true)]
[BepInPlugin("com.cowari.myplugin", "My Plugin", "1.0.0")]
//[BepInProcess("Content Warning.exe")]
public class MyPlugin : BaseUnityPlugin
{
    public void Awake()
    {
        Logger.LogInfo("МОЙ ПЛАГИН ЗАГРУЖЕН!!!");
        SceneManager.sceneLoaded += OnSceneLoaded; // Подписка на событие загрузки сцены
    }

    public void Update(){
        CheatUI.ToggleUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SurfaceScene") // Замените на имя вашей сцены
        {
            var harmony = new Harmony("com.cowari.myplugin");
            harmony.PatchAll(); // Применение патчей после загрузки сцены
            CheatUI.Initialize();
        }
    }

    // Патч для метода LoadFaceFromPlayerPrefs
    [HarmonyPatch(typeof(PlayerVisor), "LoadFaceFromPlayerPrefs")]
    class LoadFaceFromPlayerPrefsPatch
    {
        [HarmonyPrefix]
        static void Prefix(ref string __state)
        {
            __state = "лол"; // Замените на нужное вам значение
            //Debug.Log("Изменяем значение на: " + __state);
        }

        [HarmonyPostfix]
        static void Postfix(ref string __state)
        {
            Debug.Log("Изменили значение на: " + __state);

            PlayerVisor playerVisorInstance = FindObjectOfType<PlayerVisor>();
            if (playerVisorInstance != null)
            {
                if (playerVisorInstance.visorFaceText != null)
                {
                    playerVisorInstance.visorFaceText.text = __state; // Update the text
                }
                else
                {
                    Debug.LogError("visorFaceText в PlayerVisor не найден!");
                }
            }
            else
            {
                Debug.LogError("PlayerVisor instance not found!");
            }
        }
    }
}