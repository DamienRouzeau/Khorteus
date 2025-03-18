using UnityEngine;

public class NightVisionCamera : MonoBehaviour
{
    public Shader nightVisionShader;

    void Start()
    {
        if (nightVisionShader != null)
        {
            GetComponent<Camera>().SetReplacementShader(nightVisionShader, "");
        }
    }
}
