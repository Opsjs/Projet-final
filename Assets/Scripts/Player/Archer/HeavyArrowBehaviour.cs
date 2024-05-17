using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyArrowBehaviour : MonoBehaviour
{
    [SerializeField] float damage = 15f;
    [SerializeField] Transform shotPoint;

    private void Update()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, shotPoint.position, 0.5f);
    }






    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>().health -= damage;
        }
        if (!collision.collider.CompareTag("Arrow") && !collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
