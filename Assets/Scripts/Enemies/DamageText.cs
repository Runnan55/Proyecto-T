using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour
{
    public Animator animator;
    private TextMeshPro damageText;

    void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
        damageText = GetComponentInChildren<TextMeshPro>();

        if (damageText == null)
        {
            Debug.LogError("No se encontró el componente TextMeshPro en los hijos del objeto.");
        }
    }

    void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }

    public void SetText(string text)
    {
        if (damageText != null)
        {
            damageText.text = text;
        }
        else
        {
            Debug.LogError("damageText es null");
        }
    }
}