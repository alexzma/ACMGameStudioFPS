using UnityEngine;

[System.Serializable]
public class Damageable : MonoBehaviour
{
    [SerializeField] int hp;        // Health
    [SerializeField] int maxhp;
    [SerializeField] int exp;

    public void Setup(int maxhp)
    {
        hp = maxhp;
        this.maxhp = maxhp;
    }

    // Return 1 on death, 0 otherwise
    public int DoDamage(int amt)
    {
        // Do damage, then check if death
        hp -= amt;
        if (hp <= 0)
        {
            Die();
            return 1;
        }
        return 0;
    }
    
    // Health Functions

    public void AddHP(int amt)
    {
        hp += amt;
        if (hp > maxhp) hp = maxhp;
    }

    public void AddMaxHP(int amt)
    {
        maxhp += amt;
        hp += amt;
    }

    // Death Functions

    public void Die()
    {
        Destroy(gameObject);
    }

    public int getHP() { return hp; }
    public int getMaxHP() { return maxhp; }
    public int getExp() { return exp; }
}
