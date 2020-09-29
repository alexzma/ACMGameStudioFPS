using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackTestDriver : MonoBehaviour
{
    JetpackEffect je;
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        je = this.GetComponent<JetpackEffect>();
        je.ActivateJetpackEffect();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 3f)
            je.DeactivateJetpackEffect();
    }
}
