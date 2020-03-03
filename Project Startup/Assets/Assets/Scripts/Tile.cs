using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class Tile : MonoBehaviour
{
    public bool isYours; // bool tracking if the tile is yours
    public Character heldUnit; // the character on the tile, null if empty

    private Transform placementSpot; // the location where placed units go
    public Material off;
    public Material on;
    public Material myOn;
    private MeshRenderer meshRenderer;

    public GameObject particleFx;
    
    // Start is called before the first frame update
    void Awake()
    {
        placementSpot = transform.GetChild(0).GetChild(0);
        myOn = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        myOn.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        myOn.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        myOn.SetInt("_ZWrite", 0);
        myOn.DisableKeyword("_ALPHATEST_ON");
        myOn.DisableKeyword("_ALPHABLEND_ON");
        myOn.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        myOn.renderQueue = 3000;
        Color col = on.color;
        col.a= 10f / 255f;
        myOn.color = col;
        // print(myOn.color);
        myOn.EnableKeyword("_EMISSION");
        myOn.SetColor("_EmissionColor", off.GetColor("_EmissionColor"));
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = off;
        
        CenterUnit();
    }

    public bool UnitPlace(Character unit, Tile origin)
    {
        // check if I am on your team, if yes then swap me
        if (isYours)
        {
            origin.heldUnit = heldUnit;
            heldUnit = unit;
            origin.CenterUnit();
            CenterUnit();
            
            ParticlePlay(particleFx, placementSpot);
            if(origin.heldUnit != null)
                ParticlePlay(particleFx, origin.placementSpot);
            
            return true;
        }

        return false;
    }
    
    public bool UnitPlace(Character unit)
    {
        // check if I am on your team, if yes then place me
        if (isYours)
        {
            heldUnit = unit;
            ParticlePlay(particleFx, placementSpot);
            
            CenterUnit();
            return true;
        }

        return false;
    }
    
    public void UnitPlace(Character unit, bool ignoreRules)
    {
        // place unit regardless of other factors
        if (ignoreRules)
        {
            heldUnit = unit;
            CenterUnit();
        }
    }

    public void CenterUnit()
    {
        meshRenderer.material = off;
        if (heldUnit != null)
        {
            heldUnit.transform.position = placementSpot.position;
            if (!heldUnit.isOnYourTeam)
            {
                heldUnit.transform.eulerAngles = new Vector3(0,180,0);
            }
            switch (heldUnit.type)
            {
                case Character.archetype.Attacker:
                    myOn.color = new Color(0.7924528f, 0.1831613f, 0.2159052f, 10f / 255f);
                    myOn.SetColor("_EmissionColor", new Color(0.7924528f, 0.1831613f, 0.2159052f));
                    break;
                case Character.archetype.Tank:
                    myOn.color = new Color(0.4298683f, 0.6714197f, 0.7924528f, 10f / 255f);
                    myOn.SetColor("_EmissionColor", new Color(0.4298683f, 0.6714197f, 0.7924528f));
                    break;
                case Character.archetype.Assassin:
                    myOn.color = new Color(0.5283019f, 0.2815949f, 0.4347313f, 10f / 255f);
                    myOn.SetColor("_EmissionColor", new Color(0.5283019f, 0.2815949f, 0.4347313f));
                    break;
                case Character.archetype.Support:
                    myOn.color = new Color(0.3177287f, 0.7924528f, 0.4481168f, 10f / 255f);
                    myOn.SetColor("_EmissionColor", new Color(0.3177287f, 0.7924528f, 0.4481168f));
                    break;
            }
            meshRenderer.material = myOn;
        }
    }

    private void ParticlePlay(GameObject particle, Transform target)
    {
        var part = Instantiate(particle);

        part.transform.position = target.position;
        
        particle.GetComponentInChildren<ParticleSystem>().Play();
    }
}
