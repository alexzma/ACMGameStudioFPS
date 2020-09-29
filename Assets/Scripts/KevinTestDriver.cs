using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KevinTestDriver : MonoBehaviour
{
    // Start is called before the first frame update
    SpawnEffectController sec;
    private float time = 0f;
    void Start()
    {
        sec = this.GetComponent<SpawnEffectController>();
        
        StartCoroutine(sec.DespawnEffect(2f));
    }

    // Update is called once per frame
    void Update()
    {
        if (time < 2.5f)
            time += Time.deltaTime;
        else
        {

            StartCoroutine(sec.RespawnEffect(1f));
        }
    }
}
