using UnityEngine;
using UnityEngine.UI;

public class Pike : MonoBehaviour
{
    private int _hp = 2;
    public bool IsAlive => _hp > 0;

    public void DealDamageTo(Enemy enemy)
    {
        enemy.TakeDamage(2);
        _hp--;
    }
    
    public void ResetHP()
    {
        _hp = 2;
    }
    
    public void TakeDamage()
    {
        _hp--;
        if (_hp <= 0)
        {
            GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            gameObject.SetActive(false);
            GameManager.Instance.StakeCount = Mathf.Max(0, GameManager.Instance.StakeCount - 1);
            UIManager.Instance.RefreshHUD(); // добавить сюда
        }
        Debug.Log("Pikes hp = " + _hp);
    }
}