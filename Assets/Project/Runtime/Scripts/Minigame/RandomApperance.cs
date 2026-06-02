using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomApperance : MonoBehaviour
{
    public GameObject[] colors;
    public GameObject[] hairs;

    // Start is called before the first frame update
    void Start()
    {
        int randomColor = Random.Range(0, colors.Length);
        int randomHair = Random.Range(0, hairs.Length);

        for(int i = 0; i < colors.Length; i++)
        {
            if(i == randomColor)
            colors[i].SetActive(true);
            else
                colors[i].SetActive(false);
        }

        for(int i = 0;i < hairs.Length;i++)
        {
            if(i == randomHair)
            hairs[i].SetActive(true);
            else
                hairs[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
