using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance;

    [Header("Фоны по фазам")]
    public Sprite Morning;
    public Sprite Day;
    public Sprite Evening;
    public Sprite Night;

    [Header("Image для фона")]
    public Image BackgroundImage;

    void Awake() => Instance = this;

    public void UpdateBackground(GamePhase phase)
    {
        BackgroundImage.sprite = phase switch
        {
            GamePhase.Morning => Morning,
            GamePhase.Day     => Day,
            GamePhase.Evening => Evening,
            GamePhase.Night   => Night,
            _                 => Morning
        };
    }
}