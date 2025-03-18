using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel, damage, maxHP, currentHP;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // Ensure currentHP does not go below 0

        return currentHP <= 0;
    }
}
