using System;

public interface ITakeDamage 
{
    DamageType GetDamageType();
    bool TakeDamage(float damage);        
}

public enum DamageType
{
    Object,
    Person,
    Family
}
