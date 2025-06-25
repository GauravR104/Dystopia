using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    public float soundRadius = 10f;
    public float soundCooldown = 0.5f;

    [HideInInspector] public float lastSoundTime;
    [HideInInspector] public Vector3 soundPosition;

    public void EmitSound()
    {
        lastSoundTime = Time.time;
        soundPosition = transform.position;
    }
}
