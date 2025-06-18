using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlassCouple
{
    public BreakGlass glass1;
    public BreakGlass glass2;
}

public class GlassBreakManager : MonoBehaviour
{
    public GlassCouple[] glassCouples;

    private void Start()
    {
        foreach(var glassCouple in glassCouples)
        {
            if(Random.value < 0.5f)
            {
                glassCouple.glass1.SetBreakable(true);
                glassCouple.glass2.SetBreakable(false);
            }
            else
            {
                glassCouple.glass1.SetBreakable(false);
                glassCouple.glass2.SetBreakable(true);
            }
        }
    }
}
