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
    public int WallHP = 5;
    public int MaxWallHP = 5;
    public int StakeCount = 0;
    public int MaxStakeCount = 7;
    public Dictionary<ResourceType, int> Resources = new();
    public int Portions = 0;
    public VillagerBuff ActiveBuff = VillagerBuff.None;
    public bool IsGameOver = false;
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
                ForestManager.Instance.EnterForest();
                Debug.Log("День начался — жители работают, ты в лесу");
                break;

            case GamePhase.Day:
                Phase = GamePhase.Evening;
                CollectDayResources();
                ForestManager.Instance.ExitForest();
                ActiveBuff = VillagerBuff.None;
                Debug.Log("Вечер — вернулись из леса, время готовить");
                break;

            case GamePhase.Evening:
                Phase = GamePhase.Night;
                FeedVillagers();
                if (IsGameOver) return;
                ForestManager.Instance.CauldronButton.SetActive(false);
                NightManager.Instance.EnterNight();
                break;

            case GamePhase.Night:
                Phase = GamePhase.Morning;
                Day++;
                NightManager.Instance.ExitNight();
                ResetVillagerTasks();
                Debug.Log($"=== День {Day} ===");
                break;
        }

        BackgroundManager.Instance?.UpdateBackground(Phase);
        UIManager.Instance?.RefreshHUD();
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
        {
            IsGameOver = true;
            GameOverUI.Instance.Show(Day);
            return;
        }
    }

    void ResetVillagerTasks()
    {
        foreach (var v in VillagerManager.Instance.Villagers)
            v.AssignTask(VillagerTask.Idle);

        foreach (var zone in FindObjectsByType<WorkZone>(FindObjectsInactive.Exclude))
            zone.ResetZone();
    }

    public void AddResource(ResourceType type, int amount)
    {
        Resources[type] += amount;
        UIManager.Instance?.RefreshHUD();
    }
}