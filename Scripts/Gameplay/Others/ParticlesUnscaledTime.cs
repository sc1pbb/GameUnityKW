using UnityEngine;
using System.Collections;

public class ParticlesUnscaledTime : MonoBehaviour
{ 
    // Update is called once per frame
    void Update ()
    {
        if (Time.timeScale < 0.01f)
        {
            GetComponent<ParticleSystem>().Simulate (Time.unscaledDeltaTime, true, false);
        }
    }
}
