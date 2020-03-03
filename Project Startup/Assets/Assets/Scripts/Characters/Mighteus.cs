using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mighteus : Character
{
    private float smiteValue = 0;
    public override void Update()
    {
        if (!isDead && gameManager.fightRunning)
        {
            Rotate();
        
            if (!fighting)
            {
                Move();
                if (TargetInRange(aggroTarget))
                {
                    fighting = true;
                }
            }

            if (fighting)
            {
                // check if aggroTarget TargetInRange, if yes, check attackCooldown against attack speed if yes Attack, else increase by deltatime, else make fighting false
                if (TargetInRange(aggroTarget))
                {
                    if (isUpgraded)
                    {
                        if (attackCooldownValue >= attackCooldown)
                        {
                            attackCooldownValue = 0;
                            Attack();
                        }
                        else
                        {
                            attackCooldownValue += Time.deltaTime;
                        }
                    }
                    else
                    {
                        if (attackCooldownValue >= attackCooldown*2)
                        {
                            attackCooldownValue = 0;
                            Attack();
                        }
                        else
                        {
                            attackCooldownValue += Time.deltaTime;
                        }
                    }
                }
                else
                {
                    fighting = false;
                }
            }
        }
    }

    public override bool Damage(float amount)
    {
        smiteValue += defense / 3;
        return base.Damage(amount);
    }

    public override void Attack()
    {
        StartCoroutine(FakeAttackAnimation(.2f));
        // deal damage to the target mitigated by defense
        if (aggroTarget.Damage(attackDamage+smiteValue - aggroTarget.defense))
        {
            // upgrade would go here, the following is placeholder
            if (!isUpgraded)
            {
                StartCoroutine(ClaimDeath());
                Upgrade();
            }
        }

        smiteValue = 0;
    }
}
