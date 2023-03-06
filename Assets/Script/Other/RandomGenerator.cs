using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class RandomGenerator : MonoBehaviour
{
    CancellationTokenSource tokenSource;

    void Start()
    {
        tokenSource = new CancellationTokenSource();
        Generate();
    }

    private async void Generate()
    {
        System.Random rnd = new System.Random();

        var res = await Task.Run(() =>
        {
            float[] randomList = new float[1000000];
            for(int i=0;i<randomList.Length;i++)
            {
                randomList[i] = (float) rnd.NextDouble();

                if(tokenSource.IsCancellationRequested)
                {
                    return randomList;
                }

            }
            return randomList;
        }, tokenSource.Token);
      

        if(tokenSource.IsCancellationRequested)
        {
            return;
        }
    }
    
    private void OnDisable()
    {
        tokenSource.Cancel();
    }
}

