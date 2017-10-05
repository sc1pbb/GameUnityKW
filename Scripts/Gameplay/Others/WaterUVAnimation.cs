using UnityEngine;
using System.Collections;

public class WaterUVAnimation : MonoBehaviour
{
    Renderer renderer_;
    public int materialIndex_ = 0;
    public Vector2 uvAnimationRate_ = new Vector2 (1.0f, 0.0f);
    public string textureName_ = "_MainTex";

    Vector2 uvOffset = Vector2.zero;

    void Awake ()
    {
        renderer_ = this.GetComponent<Renderer> ();
    }

    void LateUpdate ()
    {
        uvOffset += (uvAnimationRate_ * Time.deltaTime);
        if (renderer_.enabled)
        {
            renderer_.materials[materialIndex_].SetTextureOffset (textureName_, uvOffset);
        }
    }
}
