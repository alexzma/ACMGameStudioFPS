using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLight : MonoBehaviour
{
    Light _light;

    private Vector3 startPosition1 = new Vector3(52, 4, -30);
    private Vector3 startPosition2 = new Vector3(52, 4, 50);
    private float maxIntensity;
    private float travelDistance;

    // Time Values
    private float glowTime = 1f;
    private float darkTime = 5f;
    private float moveTime = 2f;
    private float pause = 1f;

    // Step Values
    private float lightStep = 10;
    private float moveStep = 80;


    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        maxIntensity = _light.intensity;
        travelDistance = startPosition2.z - startPosition1.z;
        _light.intensity = 0;

        GetComponent<Transform>().position = startPosition1;

        StartCoroutine(BeginProcess());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GlowInPlace(bool direction)
    {
        float multiplier = direction ? 1 : -1;
        for (float i = 0; i != lightStep; i++)
        {
            yield return new WaitForSeconds(glowTime / lightStep);
            _light.intensity += maxIntensity / lightStep * multiplier;
        }
    }

    private IEnumerator MoveAcross(bool direction)
    {
        float multiplier = direction ? 1 : -1;
        for (float i = 0; i < travelDistance; i += travelDistance / moveStep)
        {
            yield return new WaitForSeconds(moveTime / moveStep);
            GetComponent<Transform>().position += new Vector3(0, 0, travelDistance / moveStep * multiplier);
        }
        if (direction)
            transform.position = startPosition2;
        else
            transform.position = startPosition1;
    }

    private IEnumerator BeginProcess()
    {
        StartCoroutine(GlowInPlace(true));
        yield return new WaitForSeconds(glowTime + pause);

        StartCoroutine(MoveAcross(true));
        yield return new WaitForSeconds(moveTime + pause * 2);

        StartCoroutine(GlowInPlace(false));
        yield return new WaitForSeconds(glowTime + pause + darkTime);

        StartCoroutine(GlowInPlace(true));
        yield return new WaitForSeconds(glowTime + pause);

        StartCoroutine(MoveAcross(false));
        yield return new WaitForSeconds(moveTime + pause * 2);

        StartCoroutine(GlowInPlace(false));
        yield return new WaitForSeconds(glowTime + pause + darkTime);

        StartCoroutine(BeginProcess());
    }
}
