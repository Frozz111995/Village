using System.Collections.Generic;
using UnityEngine;

public enum GamePhase { Morning, Day, Evening, Night }

public enum ResourceType
{
    Wood,
    Berries, Mushrooms, Fish,
    Meat, Roots, Herbs,
    Vegetables
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
                // Назначаем жителей — просто переходим в день
                Phase = GamePhase.Day;
                ActiveBuff = VillagerBuff.None;
                Debug.Log("День начался — жители работают, ты в лесу");
                break;

            case GamePhase.Day:
                // Жители отработали день — собираем ресурсы
                Phase = GamePhase.Evening;
                CollectDayResources();
                Debug.Log("Вечер — вернулись из леса, время готовить");
                break;

            case GamePhase.Evening:
                // Игрок приготовил еду — кормим жителей и идём в ночь
                Phase = GamePhase.Night;
                FeedVillagers();
                Debug.Log("Ночь наступила");
                break;

            case GamePhase.Night:
                Phase = GamePhase.Morning;
                Day++;
                ResetVillagerTasks();
                Debug.Log($"=== День {Day} ===");
                break;
        }
        BackgroundManager.Instance?.UpdateBackground(Phase);
        UIManager.Instance?.RefreshHUD();
    }
    void ResetVillagerTasks()
    {
        foreach (var v in VillagerManager.Instance.Villagers)
            v.AssignTask(VillagerTask.Idle);

        foreach (var zone in FindObjectsByType<WorkZone>(FindObjectsInactive.Exclude))
            zone.ResetZone();
    }
    

    void CollectDayResources()
    {
        var vm = VillagerManager.Instance;
        var collected = vm.ProcessTasks();

        Resources[ResourceType.Wood] += collected.Wood;
        Resources[ResourceType.Vegetables] += collected.Vegetables;
        Resources[ResourceType.Meat] += collected.Meat;
        Resources[ResourceType.Berries] += collected.Berries;
        Resources[ResourceType.Mushrooms] += collected.Mushrooms;
        Resources[ResourceType.Herbs] += collected.Herbs;

        // Кухня даёт порции напрямую
        int kitchenPortions = vm.ProcessKitchen();
        Portions += kitchenPortions;

        Debug.Log($"Ресурсы собраны. Кухня: +{kitchenPortions} порций");
    }

    void FeedVillagers()
    {
        var vm = VillagerManager.Instance;
        int villagerCount = vm.Villagers.Count;
        int consumed = Mathf.Min(Portions, villagerCount);

        Debug.Log($"=== КОРМЁЖКА === порций: {Portions}, жителей: {villagerCount}, съедено: {consumed}");

        vm.ProcessEndOfDay(Portions);
        Portions -= consumed;

        Debug.Log($"Осталось порций: {Portions}, живых жителей: {vm.Villagers.Count}");

        if (vm.Villagers.Count == 0)
            Debug.Log("Все жители погибли. Игра окончена.");
    }

    public void AddResource(ResourceType type, int amount)
    {
        Resources[type] += amount;
        UIManager.Instance?.RefreshHUD();
    }
}