using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericObjectKeeper : MonoBehaviour
{
    public static GenericObjectKeeper Instance;

    public GameObject healParticles;


    private void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
