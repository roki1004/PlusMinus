using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : WeakSingletonMonoBehaviour<ParticleManager>
{
    //============================================
    public GameObject particlePrefab;
    //============================================
    private int count;
    private List<GameObject> list = new List<GameObject>();
    //============================================
    private void Awake()
    {
        base.Awake();

        if (UIObjects.Instance != null)
            count = UIObjects.Instance.TILE_SIZE * UIObjects.Instance.TILE_SIZE;
        else
            count = 10;
    }
    //============================================
    public void Init(Transform target)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(particlePrefab) as GameObject;
            go.transform.SetParent(target);
            go.transform.localScale = Vector3.one;
            go.SetActive(false);
            list.Add(go);
        }
    }

    public void Push(GameObject go)
    {
        go.SetActive(false);
    }

    public GameObject Pop()
    {
        foreach(GameObject go in list)
        {
            if(go.activeSelf == false)
            {
                go.SetActive(true);
                return go;
            }
        }

        return null;
    }
    //============================================
}
