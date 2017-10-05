using UnityEngine;
using System.Collections;

public class DestroyInTime : MonoBehaviour
{
    [SerializeField]
    private float duration_ = 4f;

    void Start ()
    {
        Destroy (this.gameObject, duration_);
    }
}
