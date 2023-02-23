/*********************************************************************************************
* Project: Alarm Im Darm
* File   : IDamageable
* Date   : 15.11.2022
* Author : Levi
*
* This is the interface for dealing damage
* 
*********************************************************************************************/

public interface IDamageable
{
    //gets called when the hit object takes damage
    public abstract void TakeDamage(int damage);
}
