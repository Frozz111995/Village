using UnityEngine;
using UnityEngine.UI;

public class WallBoundary : MonoBehaviour
{
    public RectTransform[] Points; // 4 точки сверху вниз

    // Получить X границы для заданной Y позиции
    public float GetWallX(float worldY)
    {
        if (Points == null || Points.Length < 2) return 0f;

        for (int i = 0; i < Points.Length - 1; i++)
        {
            float y0 = Points[i].position.y;
            float y1 = Points[i + 1].position.y;

            if (worldY <= y0 && worldY >= y1)
            {
                float t = Mathf.InverseLerp(y0, y1, worldY);
                return Mathf.Lerp(Points[i].position.x, Points[i + 1].position.x, t);
            }
        }

        return Points[Points.Length - 1].position.x;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Points == null || Points.Length < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            if (Points[i] == null || Points[i + 1] == null) continue;
            Gizmos.DrawLine(Points[i].position, Points[i + 1].position);
            Gizmos.DrawSphere(Points[i].position, 5f);
        }
        Gizmos.DrawSphere(Points[Points.Length - 1].position, 5f);
    }
#endif
}