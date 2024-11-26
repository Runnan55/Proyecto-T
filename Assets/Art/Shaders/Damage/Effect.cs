using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public Material damage;
    private Coroutine damageProcess;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ScreenDamageEffect(Random.Range(0.1f, 1));
        }
    }

    public void ScreenDamageEffect(float intensity)
    {
        if (damageProcess != null)
        {
            StopCoroutine(damageProcess);
        }
        damageProcess = StartCoroutine(screenDamage(intensity));
    }

    private IEnumerator screenDamage(float intensity)
    {
        var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.1f);
		var curRadius = 1f;
		for(float t = 0; curRadius != targetRadius; t += Time.deltaTime)
		{
			curRadius = Mathf.Clamp(Mathf.Lerp(1, targetRadius, t), 1, targetRadius);
			damage.SetFloat("_Vignette_radius", curRadius);
			yield return null;
		}
		for(float t = 0; curRadius < 1; t += Time.deltaTime)
		{
			curRadius = Mathf.Lerp(targetRadius, 1, t);
			damage.SetFloat("_Vignette_radius", curRadius);
			yield return null;
		}
    }
    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }
}
