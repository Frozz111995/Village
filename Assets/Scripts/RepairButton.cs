using UnityEngine;
using UnityEngine.UI;

public enum RepairType { Wall, Stakes }

public class RepairButton : MonoBehaviour
{
    public RepairType Type;
    public int WoodCost = 1;

    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        Refresh();
    }

    void OnClick()
    {
        var gm = GameManager.Instance;
        if (gm.Resources[ResourceType.Wood] < WoodCost) return;

        gm.Resources[ResourceType.Wood] -= WoodCost;

        if (Type == RepairType.Wall)
            gm.WallHP = Mathf.Min(gm.WallHP + 1, gm.MaxWallHP);
        else
            gm.StakeCount = Mathf.Min(gm.StakeCount + 1, gm.MaxStakeCount);

        UIManager.Instance.RefreshHUD();
        Refresh();
    } 

    public void Refresh()
    {
        if (_button == null) _button = GetComponent<Button>();
    
        var gm = GameManager.Instance;
        bool canAfford = gm.Resources[ResourceType.Wood] >= WoodCost;
        bool notFull = Type == RepairType.Wall
            ? gm.WallHP < gm.MaxWallHP
            : gm.StakeCount < gm.MaxStakeCount;

        _button.interactable = canAfford && notFull;
    }
}