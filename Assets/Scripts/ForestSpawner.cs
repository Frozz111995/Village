using UnityEngine;
using System.Collections.Generic;

public class ForestSpawner : MonoBehaviour
{
    public static ForestSpawner Instance;
    private List<Vector2> _spawnedPositions = new();
    private float _minDistance = 150f;
    [Header("Спавн")]
    public RectTransform SpawnArea;
    public GameObject ForestObjectPrefab;
    public int SpawnCount = 8;

    [Header("Действия")]
    public int MaxActions = 5;
    public int ActionsLeft { get; private set; }
    
    public Vector2[] ResourceSizes = {
        new Vector2(80, 80),   // Berries
        new Vector2(60, 60),   // Mushrooms
        new Vector2(60, 60),   // Herbs
        new Vector2(100, 100), // Meat
        new Vector2(70, 70),   // Roots
        new Vector2(90, 70),   // Fish
        new Vector2(120, 150)  // Wood
    };
    public float[] OutlineWidths = { 15f, 25f, 20f, 18f, 22f, 12f, 10f };
    
    [Header("Ресурсы и веса")]
    public ResourceType[] PossibleResources = {
        ResourceType.Berries,
        ResourceType.Mushrooms,
        ResourceType.Herbs,
        ResourceType.Meat,
        ResourceType.Roots,
        ResourceType.Fish,
        ResourceType.Wood
    };
    public int[] Weights = { 30, 25, 20, 10, 10, 10, 20 };

    [Header("Спрайты ресурсов")]
    public Sprite[] ResourceSprites;

    private List<GameObject> _spawnedObjects = new();
    private bool _survivorFoundThisRun = false;

    void Awake() => Instance = this;

    public void SpawnForest()
    {
        ActionsLeft = MaxActions;
        _survivorFoundThisRun = false;
        _spawnedPositions.Clear();

        foreach (var obj in _spawnedObjects)
            Destroy(obj);
        _spawnedObjects.Clear();

        Rect rect = SpawnArea.rect;

        float minX = rect.x;
        float maxX = rect.x + rect.width;
        float minY = rect.y;
        float maxY = rect.y + rect.height;

        int maxAttempts = 30;

        for (int i = 0; i < SpawnCount; i++)
        {
            Vector2 pos = Vector2.zero;
            bool found = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

                bool tooClose = false;
                foreach (var existing in _spawnedPositions)
                {
                    if (Vector2.Distance(pos, existing) < _minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose) { found = true; break; }
            }

            if (!found) continue;

            _spawnedPositions.Add(pos);

            var go = Instantiate(ForestObjectPrefab, SpawnArea);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;

            ResourceType resource = GetRandomResource();
            int index = System.Array.IndexOf(PossibleResources, resource);

            var fo = go.GetComponent<ForestObject>();
            fo.Setup(resource, Random.Range(1, 3), ResourceSprites[index], ResourceSizes[index], OutlineWidths[index]);

            _spawnedObjects.Add(go);
        }

        // Сортируем по Y — нижние объекты рисуются поверх верхних
        _spawnedObjects.Sort((a, b) =>
            b.GetComponent<RectTransform>().anchoredPosition.y
                .CompareTo(a.GetComponent<RectTransform>().anchoredPosition.y));

        for (int i = 0; i < _spawnedObjects.Count; i++)
            _spawnedObjects[i].transform.SetSiblingIndex(i);

        UIManager.Instance?.RefreshHUD();
    }

    public void UseAction()
    {
        if (ActionsLeft > 0)
            ActionsLeft--;
        UIManager.Instance?.RefreshHUD();
    }

    public bool TryFindSurvivor()
    {
        if (_survivorFoundThisRun) return false;
        if (Random.Range(0, 100) >= 15) return false;

        _survivorFoundThisRun = true;
        return true;
    }

    ResourceType GetRandomResource()
    {
        int total = 0;
        foreach (var w in Weights) total += w;

        int roll = Random.Range(0, total);
        int cumulative = 0;

        for (int i = 0; i < PossibleResources.Length; i++)
        {
            cumulative += Weights[i];
            if (roll < cumulative) return PossibleResources[i];
        }

        return PossibleResources[0];
    }
}