using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame : MonoBehaviour
{
    [SerializeField] private GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("116/255: " + 116/255 + " 42/255: " + 42/255 + " 42/255: " + 42/255);
            background.GetComponent<Image>().color = new Color(116.0f/255, 42.0f/255, 42.0f/255, 1);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            background.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }
}
