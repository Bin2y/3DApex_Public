using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLight : MonoBehaviour
{
    public Material highLightMaterial;
    public float highLightDuration = 2f;

    private Dictionary<GameObject, Material[]> original = new Dictionary<GameObject, Material[]>();

    public void HightlightCharacter(List<GameObject> objects)
    {
        foreach (var obj in objects)
        {
            var rend = obj.GetComponentInChildren<SkinnedMeshRenderer>();
            if (rend != null && !original.ContainsKey(obj))
            {
                original[obj] = rend.sharedMaterials;
                //skinMaterial 리스트 모두 바꿔준다.
                var highLightsMaterials = new Material[rend.materials.Length];
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    highLightsMaterials[i] = highLightMaterial;
                }
                rend.materials = highLightsMaterials;
            }
        }
        StartCoroutine(ResetHighLight());
    }

    private IEnumerator ResetHighLight()
    {
        yield return new WaitForSeconds(highLightDuration);

        foreach (var ori in original)
        {
            if (ori.Key != null)
            {
                SkinnedMeshRenderer smr = ori.Key.GetComponentInChildren<SkinnedMeshRenderer>();
                smr.sharedMaterials = ori.Value;
            }
        }
        original.Clear();
    }
}
