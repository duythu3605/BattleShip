using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    private GameManager _gameManager;
    private Enemy _enemy;
    public Vector3 _targetGridLocation;
    private int targetGrid = -1;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _enemy = GameObject.Find("Enemy").GetComponent<Enemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            if (collision.gameObject.name == "Submarine") _targetGridLocation.y += 0.3f;
            _gameManager.EnemyHitPlayer(_targetGridLocation, targetGrid, collision.gameObject);
        }
        else
        {
            _enemy.PauseAndEnd(targetGrid);
        }
        Destroy(gameObject);
    }

    public void SetTarget(int target)
    {
        targetGrid = target;
    }
}
