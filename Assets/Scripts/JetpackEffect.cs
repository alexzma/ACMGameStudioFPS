using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackEffect : MonoBehaviour
{
    public GameObject psfire;
    public Transform spawnPosition1;
    public Transform spawnPosition2;
    public Transform spawnPosition3;
    private GameObject ps1;
    private GameObject ps2;
    private GameObject ps3;
    // Update is called once per frame
    public void ActivateJetpackEffect()
    {
        ps1 = Instantiate(psfire, spawnPosition1);
        ps2 = Instantiate(psfire, spawnPosition2);
        ps3 = Instantiate(psfire, spawnPosition3);
    }
    public void DeactivateJetpackEffect()
    {
        if(ps1)
            Destroy(ps1);
        if(ps2)
            Destroy(ps2);
        if(ps3)
            Destroy(ps3);
    }
}