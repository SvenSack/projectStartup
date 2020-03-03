using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Random = UnityEngine.Random;

public class Friend : Character
{
    private int hugs = 0;
    private bool isHugging;
    private float oldSpeed;
    public GameObject embraceCannon;
    private GameObject embraceCannonInst;

    public AudioSource embraceSfx;

    public override void FindAggroTarget()
    {
        switch (isOnYourTeam)
        {
            case true:
                float closest = 0;
                int closestIndex = teamManager.enemyTeam.Length + 1;
                for (int i = 0; i < teamManager.enemyTeam.Length; i++)
                {
                    if (teamManager.enemyTeam[i] != null)
                        if (teamManager.enemyTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.enemyTeam[i].transform.position - transform.position;
                            if (closest == 0 || closest < distanceVector.magnitude)
                            {
                                closest = distanceVector.magnitude;
                                closestIndex = i;
                            }
                        }
                }

                aggroTarget = teamManager.enemyTeam[closestIndex];
                break;
            case false:
                float closest1 = 0;
                int closestIndex1 = teamManager.yourTeam.Length + 1;
                for (int i = 0; i < teamManager.yourTeam.Length; i++)
                {
                    if (teamManager.yourTeam[i] != null)
                        if (teamManager.yourTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.yourTeam[i].transform.position - transform.position;
                            if (closest1 == 0 || closest1 < distanceVector.magnitude)
                            {
                                closest1 = distanceVector.magnitude;
                                closestIndex1 = i;
                            }
                        }
                }

                aggroTarget = teamManager.yourTeam[closestIndex1];
                // Debug.Log(name + " has decided to target " + aggroTarget.name);
                break;
        }

        if (hugs < 1 + Convert.ToInt32(isUpgraded))
        {
            // Play Embrace cannon SFX
            embraceSfx.Play();
            
            isHugging = true;
            hugs++;
            oldSpeed = movementSpeed;
            movementSpeed = 0;
            StartCoroutine(Hug());
        }
    }

    public override void Upgrade()
    {
        base.Upgrade();
        isHugging = true;
        hugs++;
        oldSpeed = movementSpeed;
        movementSpeed = 0;
        StartCoroutine(Hug());
    }

    private IEnumerator Hug()
    {
        embraceCannonInst = Instantiate(embraceCannon, transform.position, transform.rotation);
        embraceCannonInst.GetComponent<Projectile>().target = aggroTarget.transform;
        yield return new WaitForSeconds(0.3f);
        aggroTarget.transform.LeanMove(transform.position + transform.forward * range/2, 0.3f);
        embraceCannonInst.transform.LeanMove(transform.position + transform.forward * range/2, 0.3f);
        yield return new WaitForSeconds(0.3f);
        Destroy(embraceCannonInst);
        isHugging = false;
        movementSpeed = oldSpeed;
        aggroTarget.healthBar.transform.parent.position = Camera.main.WorldToScreenPoint(aggroTarget.transform.position + new Vector3(0, 1.5f, 0));
    }
    
    public override bool Damage(float amount)
    {
        if (amount < 0)
            amount = 0;
        health -= amount;
        if (health > 0)
        {
            GameObject newDamage = Instantiate(damageText,  Camera.main.WorldToScreenPoint(transform.position + new Vector3(Random.Range(-.5f,.5f),
                                                                                               1, 0)), Quaternion.identity, FindObjectOfType<Canvas>().transform);
            TextMeshProUGUI textMesh = newDamage.GetComponent<TextMeshProUGUI>();
            AttackText newText = newDamage.GetComponent<AttackText>();
            if (amount > 0)
            {
                // DAMAGE FX HERE
                int randomParticle = Random.Range(0, 2);
                Instantiate(damageParticle[randomParticle], transform.position, new Quaternion());
                // FindAggroTarget();
                textMesh.text = Mathf.RoundToInt(amount*10) + " !";
                textMesh.color = new Color(0.7924528f, 0.1831613f, 0.2159052f);
                newText.baseColor = new Color(0.7924528f, 0.1831613f, 0.2159052f);
            }
            else
            {
                // BLOCK FX HERE
                Instantiate(blockParticle, transform.position, new Quaternion());
                textMesh.text = "Blocked !";
            }
            healthBar.value = health;
            return false;
        }
        else
        {
            Die();
            return true;
        }
    }
}
