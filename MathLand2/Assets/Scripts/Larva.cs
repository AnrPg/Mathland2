using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Larva : MonoBehaviour
{
    [SerializeField] private GameObject apple;
    [SerializeField] private GameObject nestAnchor;
    [SerializeField] private float larvaSpeed;
    private float distanceFromApple;

    public GameObject NestAnchor { get => nestAnchor; }
    public float LarvaSpeed { get => larvaSpeed; }

    //public Person(GameObject larvaPrefab)
    
    void Update()
    {
         // Calculate distance value by X axis
        distanceFromApple = Vector2.Distance(apple.transform.position, transform.position);
        
        // If larva reaches apple then distance text shows "Finish!" word
        if (distanceFromApple <= 0)
        {
            Debug.Log("A larva got to the apple! You lose...!");
        }
    }

    public int CompareTo (object obj)
    {
        if (obj == null) return 1;

        Larva otherLarva = obj as Larva;
        if (otherLarva != null)
            return this.distanceFromApple.CompareTo(otherLarva.distanceFromApple);
        else
            throw new ArgumentException("Object is not a GameObject");
    }
}
