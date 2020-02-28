using System.Collections;
using System.Collections.Generic;
using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Range(1.0f, 10.0f)] public float range = 1.0f; // the characters range
    [Range(1.0f, 10.0f)] public float movementSpeed = 1.0f; // the characters move speed
    [Range(1.0f, 100.0f)] public float health = 1.0f; // the characters health
    [Range(1.0f, 10.0f)] public float attackDamage = 1.0f; // the characters damage
    [Range(0.1f, 4.0f)] public float attackCooldown = 1.0f; // the characters attack cd (basically attackspeed reversed)
    [Range(1.0f, 10.0f)] public float defense = 1.0f; // the characters defense
    public string name = "Sven, greatest of all Programmers"; // the characters name
    public Sprite profilePic; // the characters inventory picture
    public int instanceNumber; // the characters possible unit index
    [HideInInspector] public bool fighting; // bool checking if the unit is fighting at the moment
    [HideInInspector] public float attackCooldownValue; // value tracking the time since last attack
    [HideInInspector] public bool isUpgraded; // bool checking if the unit is upgraded
    public string ability = "Oh no, something went horribly wrong !";
    public GameObject projectile;
    public GameObject deathParticle;
    public GameObject upgradeParticle;
    public int rarity = 1;

    public enum archetype {Attacker, Tank, Assassin, Support};
    public archetype type;
    public GameObject damageText;
    

    [HideInInspector] public TeamManager teamManager;
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public InventoryManager inventoryManager;
    
    public bool isOnYourTeam; // bool tracking if unit is on your team
    public bool isDead; // bool tracking if the unit is dead
    public Character aggroTarget; // the unit that the character is currently aggroing on

    [HideInInspector] public Slider healthBar;


    // Start is called before the first frame update
    public virtual void Start()
    {
        teamManager = GameObject.FindGameObjectWithTag("TeamManager").GetComponent<TeamManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
    }

    // Update is called once per frame
    public virtual void Update()
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
                    fighting = false;
                }
            }
        }
    }

    #region target selection
    public virtual void FindAggroTarget()
    {
        // run through all enemies that are !isDead, assign the closest one to aggroTarget
        switch (isOnYourTeam)
        {
            case true:
                float closest = 0;
                int closestIndex = teamManager.enemyTeam.Length+1;
                for (int i = 0; i < teamManager.enemyTeam.Length; i++)
                {
                    if(teamManager.enemyTeam[i] != null)
                        if (teamManager.enemyTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.enemyTeam[i].transform.position - transform.position;
                            if (closest == 0 || closest > distanceVector.magnitude)
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
                int closestIndex1 = teamManager.yourTeam.Length+1;
                for (int i = 0; i < teamManager.yourTeam.Length; i++)
                {
                    if(teamManager.yourTeam[i] != null)
                        if (teamManager.yourTeam[i].isDead == false)
                        {
                            // Debug.Log(name + " considered targeting " + teamManager.enemyTeam[i].name);
                            Vector3 distanceVector = teamManager.yourTeam[i].transform.position - transform.position;
                            if (closest1 == 0 || closest1 > distanceVector.magnitude)
                            {
                                closest1 = distanceVector.magnitude;
                                closestIndex1 = i;
                            }
                        }
                }
                aggroTarget = teamManager.yourTeam[closestIndex1];
                Debug.Log(name + " has decided to target " + aggroTarget.name);
                break;
        }
    }
    
    #endregion
    
    #region attack behaviour
    public virtual bool TargetInRange(Character target)
    {
        // check if target is in range, return accordingly
        Vector3 targetVector = target.transform.position - transform.position;
        // Debug.Log(name + "says his target is " + targetVector.magnitude + " away");
        if (targetVector.magnitude <= range)
            return true;
        return false;
    }

    public virtual void Attack()
    {
        StartCoroutine(FakeAttackAnimation(.2f));
        // deal damage to the target mitigated by defense
        if (aggroTarget.Damage(attackDamage - aggroTarget.defense))
        {
            // upgrade would go here, the following is placeholder
            if (!isUpgraded)
            {
                StartCoroutine(ClaimDeath());
                Upgrade();
            }
        }
    }

    public virtual IEnumerator FakeAttackAnimation(float animationLength)
    {
        Vector3 direction = transform.forward * .3f;
        transform.LeanMove(transform.position - direction, animationLength/2);
        yield return new WaitForSeconds(animationLength/2);
        transform.LeanMove(transform.position + direction, animationLength/2);
        yield return new WaitForSeconds(animationLength/2);
        if (projectile != null)
        {
            GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
            proj.GetComponent<Projectile>().target = aggroTarget.transform;
        }
    }

    public virtual void Upgrade()
    {
        isUpgraded = true;
        StartCoroutine(UpgradeClaim());
    }

    private IEnumerator UpgradeClaim()
    {
        yield return new WaitForSeconds(1.6f);
        GameObject uP = Instantiate(upgradeParticle, transform.position, new Quaternion());
        uP.transform.SetParent(transform,true);
        ParticleSystem pS = uP.GetComponent<ParticleSystem>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.EnableKeyword("_EMISSION");
        Color myColor = Color.magenta;
        switch (type)
        {
            case archetype.Attacker:
                myColor = new Color(0.7924528f, 0.1831613f, 0.2159052f);
                break;
            case archetype.Tank:
                myColor = new Color(0.4298683f, 0.6714197f, 0.7924528f);
                break;
            case archetype.Assassin:
                myColor = new Color(0.5283019f, 0.2815949f, 0.4347313f);
                break;
            case archetype.Support:
                myColor = new Color(0.3177287f, 0.7924528f, 0.4481168f);
                break;
        }
        mat.color = myColor;
        mat.SetColor("_EmissionColor", myColor*2);
        pS.GetComponent<ParticleSystemRenderer>().trailMaterial = mat;
        transform.LeanScale(new Vector3(1.3f, 1.3f, 1.3f), 0.2f);
    }

    public virtual bool Damage(float amount)
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
                FindAggroTarget();
                textMesh.text = Mathf.RoundToInt(amount*10) + " !";
                textMesh.color = new Color(0.7924528f, 0.1831613f, 0.2159052f);
                newText.baseColor = new Color(0.7924528f, 0.1831613f, 0.2159052f);
            }
            else
            {
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
    #endregion

    #region death
    public void Die()
    {
        // check if all on my team are dead, if yes, finish game
        // run through all living characters, check if they have me as their AggroTarget, if yes, make them FindAggroTarget
        isDead = true;
        transform.LeanScale(new Vector3(0,0,0), 1.5f);
        healthBar.transform.parent.gameObject.SetActive(false);
        // play death explosion
        GameObject dP = Instantiate(deathParticle, transform.position, new Quaternion());
        Color passColor = Color.magenta;
        switch (type)
        {
            case archetype.Attacker:
                passColor = new Color(0.7924528f, 0.1831613f, 0.2159052f);
                break;
            case archetype.Tank:
                passColor = new Color(0.4298683f, 0.6714197f, 0.7924528f);
                break;
            case archetype.Assassin:
                passColor = new Color(0.5283019f, 0.2815949f, 0.4347313f);
                break;
            case archetype.Support:
                passColor = new Color(0.3177287f, 0.7924528f, 0.4481168f);
                break;
        }
        dP.GetComponent<DeathSplosion>().SetUp(passColor);
        
        bool deathCheck = true;
        switch (isOnYourTeam)
        {
            case true:
                foreach (var enemy in teamManager.yourTeam)
                {
                    if(enemy != null)
                        if (enemy.isDead != true)
                            deathCheck = false;
                }

                if (deathCheck)
                {
                    // game over, loss
                    gameManager.FightOver(false);
                    foreach (var enemy in teamManager.enemyTeam)
                    {
                        if(enemy != null)
                            if (enemy.isDead == false)
                            {
                                enemy.StartCelebration();
                            }
                    }
                }
                else
                {
                    foreach (var enemy in teamManager.enemyTeam)
                    {
                        if(enemy != null)
                            if (enemy.aggroTarget == this && enemy.isDead == false)
                                enemy.FindAggroTarget();
                    }
                }
                break;
            case false:
                foreach (var enemy in teamManager.enemyTeam)
                {
                    if(enemy != null)
                        if (enemy.isDead != true)
                            deathCheck = false;
                }

                if (deathCheck)
                {
                    // game over, win
                    gameManager.FightOver(true);
                    foreach (var enemy in teamManager.yourTeam)
                    {
                        if(enemy != null)
                            if (enemy.isDead == false)
                            {
                                enemy.StartCelebration();
                            }
                    }
                }
                else
                {
                    foreach (var enemy in teamManager.yourTeam)
                    {
                        if(enemy != null)
                            if (enemy.aggroTarget == this && enemy.isDead == false)
                                enemy.FindAggroTarget();
                    }
                }
                break;
        }
    }
    #endregion

    #region movement

    protected void Move()
    {
        transform.Translate(Vector3.forward*(Time.deltaTime*movementSpeed));
        healthBar.transform.parent.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));
    }

    protected void Rotate()
    {
        // check current rotation against the rotation of the target vector, correct as needed
        Vector3 targetVector = (aggroTarget.transform.position - transform.position).normalized;
        float angleDifference = Vector3.Angle(targetVector, transform.forward);
        // Debug.DrawRay(transform.position, targetVector, Color.blue, 2.0f);
        // Debug.DrawRay(transform.position, transform.forward, Color.green, 2.0f);
        if (angleDifference != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetVector), Time.deltaTime * 5.0f);
        }
    }
    #endregion

    #region post-game
    public virtual IEnumerator Celebrate()
    {
        yield return new WaitForSeconds(Random.Range(0,.4f));
        transform.LeanMove(transform.position + new Vector3(0, 2, 0), 0.2f);
        yield return new WaitForSeconds(.3f);
        transform.LeanMove(transform.position + new Vector3(0, -2, 0), 0.4f);
        yield return new WaitForSeconds(.7f);
        if(gameManager.endScreen)
            StartCoroutine(Celebrate());
    }

    public void StartCelebration()
    {
        StartCoroutine(Celebrate());
    }
    
    #endregion

    public void Heal(float amount)
    {
        GameObject newDamage = Instantiate(damageText,  Camera.main.WorldToScreenPoint(transform.position + new Vector3(Random.Range(-.5f,.5f),
                                                                                           1, 0)), Quaternion.identity, FindObjectOfType<Canvas>().transform);
        TextMeshProUGUI textMesh = newDamage.GetComponent<TextMeshProUGUI>();
        AttackText newText = newDamage.GetComponent<AttackText>();
        textMesh.text = Mathf.RoundToInt(amount*10) + " !";
        textMesh.color = new Color(0.3177287f, 0.7924528f, 0.4481168f);
        newText.baseColor = new Color(0.3177287f, 0.7924528f, 0.4481168f);
        health += amount;
        healthBar.value = health;
    }

    public IEnumerator ClaimDeath()
    {
        yield return new WaitForSeconds(.5f);
        DeathSplosion[] dS = FindObjectsOfType<DeathSplosion>();
        foreach (var target in dS)
        {
            if (target.unclaimed)
            {
                target.Claim(transform);
            }
        }
    }
}
