using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    public float Speed = 1.2f;
    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * Speed);
    }
}
