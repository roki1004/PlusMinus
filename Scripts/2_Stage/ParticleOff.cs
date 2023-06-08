using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleOff : MonoBehaviour
{
    //============================================
    private ParticleSystem ps;
    //============================================
    private void Awake()
    {
        ps = this.GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckStop());
    }
    //============================================
    IEnumerator CheckStop()
    {
        while(ps.isStopped == false)
        {
            yield return new WaitForEndOfFrame();
        }

        this.gameObject.SetActive(false);
    }
    //============================================
}
