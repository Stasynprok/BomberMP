using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepScript : MonoBehaviour
{
    
    private float dur;
    private float time;
    private SpriteRenderer sr;
    FootStepPool FootStepPool;

    private void Awake()
    {
        dur = 15;
        time = 0;
        sr = transform.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        float _a = sr.color.a;
        float alpha = _a - (_a * time / dur);
        if (alpha <= 0.05f)
        {
            
            StopStepWork();
            return;
        }
        time += Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
    }

    public void Initialize(FootStepPool action)
    {
        FootStepPool = action;
    }

    private void StopStepWork()
    {
        time = 0;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1.0f);
        FootStepPool.ReturnObjectInPool(gameObject);
    }
}
