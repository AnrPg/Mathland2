using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    [SerializeField] private float FloatStrenght;
    [SerializeField] private bool floatAround;

    public bool FloatAround { get => floatAround; set => floatAround = value; }

    /*
    private void Awake()
    {
        Debug.Log("GetComponent<Rigidbody>(): " + GetComponent<Rigidbody>());
    }
    */

    void Update ()
    {
        if (FloatAround)
        {
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle;
            //randomDirection.Normalize();

            GetComponent<Rigidbody2D>().AddForce(randomDirection * FloatStrenght);
            //GetComponent<RectTransform>().Rotate(0, 0, RandomRotationStrenght);
        }        
    }
}
