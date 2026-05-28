using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IngredientFlyAnimation : MonoBehaviour
{
    public static GameObject Play(RectTransform item, RectTransform target, Transform container, Vector2 offset, System.Action onComplete)
    {
        var iconTransform = item.Find("Icon");
        if (iconTransform == null)
        {
            onComplete?.Invoke();
            return null;
        }

        var copy = Instantiate(iconTransform, container);
        var copyRect = copy.GetComponent<RectTransform>();
        copyRect.position = iconTransform.position;
        copyRect.sizeDelta = iconTransform.GetComponent<RectTransform>().sizeDelta;

        Vector3 targetPos = target.position + new Vector3(offset.x, offset.y, 0);

        item.GetComponent<MonoBehaviour>().StartCoroutine(
            FlyArc(copyRect, iconTransform.position, targetPos, 0.6f, onComplete)
        );

        return copy.gameObject;
    }

    static IEnumerator FlyArc(RectTransform obj, Vector3 from, Vector3 to, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        Vector3 peak = (from + to) / 2f + Vector3.up * 200f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 pos = Mathf.Pow(1 - t, 2) * from
                          + 2 * (1 - t) * t * peak
                          + Mathf.Pow(t, 2) * to;

            obj.position = pos;
            obj.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t * t);

            yield return null;
        }

        Destroy(obj.gameObject);
        onComplete?.Invoke();
    }
}