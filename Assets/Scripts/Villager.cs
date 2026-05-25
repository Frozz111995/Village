public class Villager
{
    public string Name;
    public int HP;
    public int MaxHP = 3;
    public VillagerTask Task = VillagerTask.Idle;
    public bool IsAlive => HP > 0;

    public Villager(string name)
    {
        Name = name;
        HP = MaxHP;
    }

    public void Feed()
    {
        if (HP < MaxHP)
        {
            HP += 1;
            UnityEngine.Debug.Log($"{Name} поел, HP: {HP}");
        }
    }
    

    public void Starve()
    {
        HP -= 1;
        UnityEngine.Debug.Log($"{Name} голодает, HP: {HP}");
    }

    public void AssignTask(VillagerTask task)
    {
        Task = task;
        UnityEngine.Debug.Log($"{Name} назначен на: {task}");
    }
}