using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundlle : MonoBehaviour
{
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("GameOver");
            GameManager.Instance.GameOver();
        }
    }

}
