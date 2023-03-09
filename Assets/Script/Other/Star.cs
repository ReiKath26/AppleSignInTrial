using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star
{
    public string name;
    public int neededPoint;
    public string sprite;

    public Star(string name, int neededPoint, string sprite)
    {
        this.name = name;
        this.neededPoint = neededPoint;
        this.sprite = sprite;
    }
}