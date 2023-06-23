using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePrefabs : MonoBehaviour
{
    public GameObject _redFire;
    public GameObject _yellowFire;
    public GameObject _orangeFire;
    private int count;


    List<Color> fireColors = new List<Color> { Color.red, Color.yellow, new Color(1.0f, 0.64f, 0) };
    private void FixedUpdate()
    {
        if(count > 30)
        {
            fireColors.Add(Color.red);
            int rnd = Random.Range(0, fireColors.Count);
            _redFire.GetComponent<Renderer>().material.SetColor("_Color", fireColors[rnd]);
            fireColors.RemoveAt(rnd);
            rnd = Random.Range(0, fireColors.Count);
            _orangeFire.GetComponent<Renderer>().material.SetColor("_Color", fireColors[rnd]);
            fireColors.RemoveAt(rnd);
            _yellowFire.GetComponent<Renderer>().material.SetColor("_Color", fireColors[0]);
            fireColors.Clear();
            fireColors = new List<Color> { Color.red, Color.yellow, new Color(1.0f, 0.64f, 0) };
            count = 0;
        }
        count++;
    }
}
