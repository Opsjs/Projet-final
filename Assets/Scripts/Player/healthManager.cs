using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthManager : MonoBehaviour
{
    [SerializeField] GameObject heroEntity;
    public float health;
    private void Update()
    {
        CheckHealth();

    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            Debug.Log("U ded bro");
            Destroy(heroEntity);
        }
    }
}
