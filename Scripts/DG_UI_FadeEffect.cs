using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DG_UI_FadeEffect : MonoBehaviour
{
    public float fadeDuration = 1f;
    private Renderer[] renderers;
    private CanvasGroup[] canvasGroups;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        canvasGroups = GetComponentsInChildren<CanvasGroup>();
    }

    public void FadeOut() // �������
    {
        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_Color"))
            {
                rend.material.DOFade(0, fadeDuration);
            }
        }

        foreach (CanvasGroup cg in canvasGroups)
        {
            cg.DOFade(0, fadeDuration);
        }
    }

    public void FadeIn() // �ٽ� ��Ÿ����
    {
        foreach (Renderer rend in renderers)
        {
            if (rend.material.HasProperty("_Color"))
            {
                rend.material.DOFade(1, fadeDuration);
            }
        }

        foreach (CanvasGroup cg in canvasGroups)
        {
            cg.DOFade(1, fadeDuration);
        }
    }
}
