using UnityEngine;
using System.Collections;

public class SkyboxSwap : MonoBehaviour
{
    // skybox material extracts from assets
    // if not working on build. use resources.load
    public Material DoomSkybox;         // drag it here
    public float    ChangeDelay = 3f;   // after sec change skybox

    private void OnEnable()
    {
        StartCoroutine(ChangeSkybox());
    }

    IEnumerator ChangeSkybox()
    {
        yield return new WaitForSeconds(ChangeDelay);
        RenderSettings.skybox = DoomSkybox;
    }
}
