using UnityEngine;

public class ForestManager : MonoBehaviour
{
    public static ForestManager Instance;

    public GameObject VillageRoot;
    public GameObject ForestRoot;
    public GameObject VillageBackground;
    public GameObject ForestBackground;

    [Header("UI")]
    public GameObject CauldronButton;
    public GameObject ActionCounter;

    void Awake()
    {
        Instance = this;
        CauldronButton.SetActive(false);
        ActionCounter.SetActive(false);
    }

    public void EnterForest()
    {
        VillageRoot.SetActive(false);
        ForestRoot.SetActive(true);
        VillageBackground.SetActive(false);
        ForestBackground.SetActive(true);

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
    }
}