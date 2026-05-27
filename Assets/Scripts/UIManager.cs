using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public TMP_Text DayText;
    public TMP_Text PhaseText;
    public TMP_Text WoodText;
    public TMP_Text PortionsText;
    public TMP_Text VillagersText;
    public TMP_Text BuffText;
    public TMP_Text HintText;
    public TMP_Text ActionCounterText;
    public Button EndDayButton;
    public TMP_Text WallHealthText;
    void Awake() => Instance = this;

    void Start()
    {
        EndDayButton.onClick.AddListener(OnEndDayClicked);
        RefreshHUD();
    }

    public void RefreshHUD()
    {
        var gm = GameManager.Instance;
        var vm = VillagerManager.Instance;

        DayText.text = $"День {gm.Day}";
        PhaseText.text = gm.Phase.ToString();
        WoodText.text = $"Дерево: {gm.Resources[ResourceType.Wood]}";
        PortionsText.text = $"Еда: {gm.Portions}";
        VillagersText.text = $"Люди: {vm.Villagers.Count}";
        BuffText.text = gm.ActiveBuff == VillagerBuff.None ? "" : $"{gm.ActiveBuff}";

        bool isNight = gm.Phase == GamePhase.Night;
        bool isEvening = gm.Phase == GamePhase.Evening;
        bool isDay = gm.Phase == GamePhase.Day;

        WallHealthText.gameObject.SetActive(isNight || isEvening);
        WallHealthText.text = $"Стена: {gm.WallHP}/{gm.MaxWallHP}  Колья: {gm.StakeCount}/{gm.MaxStakeCount}";

        ActionCounterText.gameObject.SetActive(isDay);
        if (isDay)
            ActionCounterText.text = $"Действий: {ForestSpawner.Instance.ActionsLeft}/{ForestSpawner.Instance.MaxActions}";

        HintText.text = gm.Phase switch
        {
            GamePhase.Morning => "Назначь жителей на работы",
            GamePhase.Day     => "Собирай ресурсы, возвращайся до темноты",
            GamePhase.Evening => "Приготовь еду в котле, почини укрепления",
            GamePhase.Night   => "Переживи ночь",
            _                 => ""
        };

        EndDayButton.GetComponentInChildren<TMP_Text>().text = gm.Phase switch
        {
            GamePhase.Morning => "Уйти в лес",
            GamePhase.Day     => "Вернуться в деревню",
            GamePhase.Evening => "Завершить вечер",
            GamePhase.Night   => "Рассвет",
            _                 => "Далее"
        };
    }

    void OnEndDayClicked() => GameManager.Instance.AdvancePhase();
}