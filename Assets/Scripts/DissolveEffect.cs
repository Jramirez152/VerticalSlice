using UnityEngine;
using System.Collections;

public class DissolveEffect : MonoBehaviour
{
    [Header("Settings")]
    public Material dissolveMaterial;
    public float dissolveSpeed = 2f;

    private Renderer[] _renderers;
    private Material[][] _originalMaterials;
    private bool _dissolving = false;

    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalMaterials = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
            _originalMaterials[i] = _renderers[i].materials;
    }

    public void StartDissolve()
    {
        if (!_dissolving)
            StartCoroutine(DissolveCoroutine());
    }

    IEnumerator DissolveCoroutine()
    {
        _dissolving = true;

       
        foreach (var r in _renderers)
        {
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = dissolveMaterial;
            r.materials = mats;
        }

        float dissolveAmount = 0f;

        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("DissolveAmount", dissolveAmount);
            yield return null;
        }

     
        GameManager.Instance?.OnPlayerDeath();
    }
}