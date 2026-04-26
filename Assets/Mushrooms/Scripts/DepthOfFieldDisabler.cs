using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DepthOfFieldDisabler : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnLoad()
    {
        Apply();
        var go = new GameObject("DepthOfFieldDisabler_Watchdog");
        DontDestroyOnLoad(go);
        go.AddComponent<DepthOfFieldDisabler>();
        SceneManager.sceneLoaded += (_, _2) => Apply();
    }

    private void LateUpdate() => Apply();

    private static void Apply()
    {
        foreach (var v in Object.FindObjectsByType<Volume>(FindObjectsSortMode.None))
        {
            if (v.profile == null) continue;
            if (v.profile.TryGet<DepthOfField>(out var dof) == true) dof.active = false;
        }

        foreach (var cam in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            var data = cam.GetUniversalAdditionalCameraData();
            if (data == null) continue;
            data.renderPostProcessing = false;
        }
    }
}
