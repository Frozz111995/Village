using UnityEngine;
using UnityEngine.UI;

public class WorkZone : MonoBehaviour
{
    public VillagerTask Task;

    private Villager _assignedVillager;
    private Image _buttonImage;

    private Color _emptyColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color _filledColor = new Color(0.3f, 0.7f, 0.3f, 1f);

    void Start()
    {
        _buttonImage = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(OnTap);
        Refresh();
    }
    
    public void ResetZone()
    {
        _assignedVillager = null;
        Refresh();
    }

    void OnTap()
    {
        if (GameManager.Instance.Phase != GamePhase.Morning) return;

        var vm = VillagerManager.Instance;

        if (_assignedVillager != null && _assignedVillager.IsAlive)
        {
            _assignedVillager.AssignTask(VillagerTask.Idle);
            _assignedVillager = null;
        }
        else
        {
            var idle = vm.Villagers.Find(v => v.IsAlive && v.Task == VillagerTask.Idle);
            if (idle != null)
            {
                idle.AssignTask(Task);
                _assignedVillager = idle;
            }
        }

        Refresh();
        UIManager.Instance?.RefreshHUD();
    }

    public void Refresh()
    {
        if (_assignedVillager != null && _assignedVillager.IsAlive)
            _buttonImage.color = _filledColor;
        else
        {
            _buttonImage.color = _emptyColor;
            _assignedVillager = null;
        }
    }
}