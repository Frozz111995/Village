using System.Collections.Generic;
using UnityEngine;

public enum GamePhase { Morning, Day, Evening, Night }

public enum ResourceType
{
    Wood,
    Berries, Mushrooms, Fish,
    Meat, Roots, Herbs
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Day = 1;
    public GamePhase Phase = GamePhase.Morning;

    public Dictionary<ResourceType, int> Resources = new();
    public int Portions = 0;
    public VillagerBuff ActiveBuff = VillagerBuff.None;

    void Awake()
    {
        Instance = this;
        InitResources();
    }

    void InitResources()
    {
        foreach (ResourceType r in System.Enum.GetValues(typeof(ResourceType)))
            Resources[r] = 0;

        Resources[ResourceType.Wood] = 5;
        Resources[ResourceType.Berries] = 3;
        Resources[ResourceType.Mushrooms] = 2;
    }

    public void AdvancePhase()
    {
        switch (Phase)
        {
            case GamePhase.Morning:
                Phase = GamePhase.Day;
                ActiveBuff = VillagerBuff.None;
                break;

            case GamePhase.Day:
                Phase = GamePhase.Evening;
                CollectDayResources();
                break;

            case GamePhase.Evening:
                Phase = GamePhase.Night;
                ProcessEvening();
                break;

            case GamePhase.Night:
                Phase = GamePhase.Morning;
                Day++;
                Debug.Log($"=== День {Day} ===");
                break;
        }

        Debug.Log($"Фаза: {Phase}");
        UIManager.Instance?.RefreshHUD();
    }

    void CollectDayResources()
    {
        int wood = VillagerManager.Instance.CollectResources(VillagerTask.Wood);
        int food = VillagerManager.Instance.CollectResources(VillagerTask.Food);

        Resources[ResourceType.Wood] += wood;
        Resources[ResourceType.Berries] += food;

        Debug.Log($"Собрано: дерево +{wood}, ягоды +{food}");
    }

    void ProcessEvening()
    {
        VillagerManager.Instance.ProcessEndOfDay(Portions);

        int consumed = Mathf.Min(Portions, VillagerManager.Instance.Villagers.Count);
        Portions -= consumed;

        Debug.Log($"Съедено порций: {consumed}, осталось: {Portions}");

        if (VillagerManager.Instance.Villagers.Count == 0)
            Debug.Log("Все жители погибли. Игра окончена.");
    }

    public bool TryCook(List<ResourceType> ingredients)
    {
        foreach (var ing in ingredients)
        {
            if (Resources[ing] <= 0)
            {
                Debug.Log($"Не хватает: {ing}");
                return false;
            }
        }

        foreach (var ing in ingredients)
            Resources[ing]--;

        var recipe = RecipeBook.Instance.FindRecipe(ingredients);

        if (recipe != null)
        {
            Portions += recipe.Portions;
            ActiveBuff = recipe.BuffType;
            Debug.Log($"Приготовлено: {recipe.Name} +{recipe.Portions} порций. {recipe.Buff}");
        }
        else
        {
            Portions += 1;
            Debug.Log("Похлёбка. +1 порция");
        }

        UIManager.Instance?.RefreshHUD();
        return true;
    }

    public void AddResource(ResourceType type, int amount)
    {
        Resources[type] += amount;
        UIManager.Instance?.RefreshHUD();
    }
}