using UnityEngine;

public class ForestManager : MonoBehaviour
{
    public static ForestManager Instance;

    public GameObject VillageRoot;
    public GameObject ForestRoot;
    public GameObject VillageBackground;
    public GameObject ForestBackground;
    public GameObject VillagersCanvas;
    public GameObject EveningButtonsCanvas;

    [Header("UI")]
    public GameObject CauldronButton;
    public GameObject ActionCounter;

    void Awake()
    {
        Instance = this;
        CauldronButton.SetActive(false);
        ActionCounter.SetActive(false);
        EveningButtonsCanvas.SetActive(false);
    }

    public void EnterForest()
    {
        VillageRoot.SetActive(false);
        VillageBackground.SetActive(false);
        ForestRoot.SetActive(true);
        ForestBackground.SetActive(true);
        VillagersCanvas.SetActive(false);
        EveningButtonsCanvas.SetActive(false);
        CauldronButton.SetActive(false);
        ActionCounter.SetActive(true);
        ForestSpawner.Instance.SpawnForest();
    }

    public void ExitForest()
    {
        ForestRoot.SetActive(false);
        VillageRoot.SetActive(true);
        ForestBackground.SetActive(false);
        VillageBackground.SetActive(true);
        ActionCounter.SetActive(false);
        CauldronButton.SetActive(true);
        VillagersCanvas.SetActive(false);
        EveningButtonsCanvas.SetActive(true);
        foreach (var btn in FindObjectsByType<RepairButton>(FindObjectsInactive.Exclude))
            btn.Refresh();
        if (GameManager.Instance.ActiveBuff == VillagerBuff.ExpeditionBonus)
            ForestSpawner.Instance.MaxActions--; 
    }
}