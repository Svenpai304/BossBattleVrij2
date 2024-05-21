using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    public static List<GameObject> particles = new();
    public List<GameObject> localParticles = new();


    private void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        particles = localParticles;
    }

    public static void SpawnParticles(int id, Vector2 position, Vector2 scale, Quaternion rotation, Transform parent)
    {
        GameObject obj = Instantiate(particles[id], position, rotation, parent);
        obj.transform.localScale = scale;
    }

    public static void SpawnParticles(int id, Vector2 position, Vector2 scale, Quaternion rotation)
    {
        GameObject obj = Instantiate(particles[id], position, rotation);
        obj.transform.localScale = scale;
    }
}
