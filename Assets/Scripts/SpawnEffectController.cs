using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffectController : MonoBehaviour
{
    public float time = 0;
    List<Material[]> originalMats;
    Material spawnMat;
    public Shader shader;
    Renderer[] rendArr;
    Renderer rend;
    private float edgeWidth = 0.3f;
    private float noiseScale = 30f;

    // Start is called before the first frame update
    private void Awake()
    {
        List<Renderer> rendList = GetRenderers();
        originalMats = new List<Material[]>();
        for(int i=0; i<rendList.Count; i++)
        {
            originalMats.Add(rendList[i].materials);
        }

        //spawnMat = new Material(Shader.Find("Shader Graphs/RespawnEffect"));
        spawnMat = new Material(shader);
        spawnMat.SetFloat("_EdgeWidth", edgeWidth);
        spawnMat.SetFloat("_NoiseScale", noiseScale);
    }
   
    
    public IEnumerator DespawnEffect(float timeToRender)
    {
        // Play Despawn sound
        GetComponent<AudioManager>().Play("Despawn");

        //Debug.Log("Despawn");
        time = 0f;
        List<Renderer> rendList = GetRenderers();
        for (int i = 0; i < rendList.Count; i++)
        {
            Renderer rend = rendList[i];
            Material[] spawnMats = new Material[rend.materials.Length];
            for (int j = 0; j < rend.materials.Length; j++)
            {
                spawnMats[j] = spawnMat;
            }
            rend.materials = spawnMats;
        }

        while (time <=timeToRender)
        {
            time += Time.deltaTime;
            for (int i = 0; i < rendList.Count; i++)
            {
                Renderer rend = rendList[i];
                for (int j = 0; j < rend.materials.Length; j++)
                {
                    rend.materials[j].SetFloat("_CurrentTime", time / timeToRender);

                }
            }
            yield return null;
        }

    }
    public IEnumerator RespawnEffect(float timeToRender)
    {
        // Play respawn sound
        GetComponent<AudioManager>().Play("Respawn");

        time = timeToRender;
        List<Renderer> rendList = GetRenderers();
        for(int i=0; i<rendList.Count; i++)
        {
            Renderer rend = rendList[i];
            Material[] spawnMats = new Material[rend.materials.Length];
            for(int j=0; j<rend.materials.Length; j++)
            {
                spawnMats[j] = spawnMat;
            }
            rend.materials = spawnMats;
        }
        
        
        while (time >= 0)
        {
            time -= Time.deltaTime;
            for(int i=0; i<rendList.Count; i++)
            {
                Renderer rend = rendList[i];
                for (int j = 0; j < rend.materials.Length; j++)
                {
                    //Debug.Log(rend.materials[j]);
                    rend.materials[j].SetFloat("_CurrentTime", time / timeToRender);

                }
            }
            
            yield return null;
        }
        //reset materials
        for (int i = 0; i < rendList.Count; i++)
        {
            Renderer rend = rendList[i];
            rend.materials = originalMats[i];
        }

    }
    List<Renderer> GetRenderers()
    {
        List<Renderer> rendererList = new List<Renderer>();
        foreach (Renderer objectRenderer in this.gameObject.GetComponentsInChildren<Renderer>())
        {
            if(objectRenderer.GetType() != typeof(ParticleSystemRenderer))
                rendererList.Add(objectRenderer);
        }
        return rendererList;
    }
}

