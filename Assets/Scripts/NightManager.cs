using UnityEngine;

public class NightManager : MonoBehaviour
{
    public static NightManager Instance;

    public GameObject NightRoot;
    public GameObject NightBackground;
    public GameObject VillageRoot;
    public GameObject VillageBackground;
    public GameObject VillagersCanvas;
    public GameObject EveningButtonsCanvas;

    void Awake() => Instance = this;

    public void EnterNight()
    {
        VillageRoot.SetActive(false);
        VillageBackground.SetActive(false);
        NightRoot.SetActive(true);
        NightBackground.SetActive(true);
        EveningButtonsCanvas.SetActive(false);
        NightBattleManager.Instance.StartBattle();
    }

    public void ExitNight()
    {
        NightRoot.SetActive(false);
        NightBackground.SetActive(false);
        VillageRoot.SetActive(true);
        VillageBackground.SetActive(true);
        VillagersCanvas.SetActive(true);
        EveningButtonsCanvas.SetActive(false);
    }
}