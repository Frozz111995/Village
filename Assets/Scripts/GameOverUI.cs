using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    public GameObject Panel;
    public TMP_Text DaysText;
    public Button RestartButton;

    void Awake()
    {
        Instance = this;
        Panel.SetActive(false);
    }

    void Start()
    {
        RestartButton.onClick.AddListener(OnRestart);
    }

    public void Show(int days)
    {
        Panel.SetActive(true);
        DaysText.text = $"Вы продержались {days} дней";
    }

    void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}