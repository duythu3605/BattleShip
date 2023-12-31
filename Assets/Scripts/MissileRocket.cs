using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileRocket : MonoBehaviour
{

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {       
        gameManager.CheckHit(collision.gameObject);
        Destroy(gameObject);
    }
}
