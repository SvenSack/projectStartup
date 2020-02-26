using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSplosion : MonoBehaviour
{
    private ParticleSystem partSys;
    private Material mat;
    public bool unclaimed = true;
    private Transform claimant;
    private Coroutine ensureClaim;

    public void SetUp(Color myColor)
    {
        partSys = GetComponent<ParticleSystem>();
        mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.EnableKeyword("_EMISSION");
        mat.color = myColor;
        mat.SetColor("_EmissionColor", myColor*2);
        partSys.GetComponent<ParticleSystemRenderer>().trailMaterial = mat;
        ensureClaim = StartCoroutine(EnsureClaim());
    }

    public void Claim(Transform target)
    {
        unclaimed = false;
        claimant = target;
        float x = target.position.x - transform.position.x;
        float z = target.position.z - transform.position.z;
        var vOL = partSys.velocityOverLifetime;
        vOL.orbitalOffsetX = x;
        vOL.orbitalOffsetZ = z;
        vOL.orbitalZ = 1;
        StartCoroutine(SecureClaim());
        StopCoroutine(ensureClaim);
    }

    private IEnumerator SecureClaim()
    {
        yield return new WaitForSeconds(.1f);
        float x = claimant.position.x - transform.position.x;
        float z = claimant.position.z - transform.position.z;
        var vOL = partSys.velocityOverLifetime;
        vOL.orbitalOffsetX = x;
        vOL.orbitalOffsetZ = z;
        yield return new WaitForSeconds(.1f);
        x = claimant.position.x - transform.position.x;
        z = claimant.position.z - transform.position.z;
        vOL.orbitalOffsetX = x;
        vOL.orbitalOffsetZ = z;
        yield return new WaitForSeconds(.1f);
        x = claimant.position.x - transform.position.x;
        z = claimant.position.z - transform.position.z;
        vOL.orbitalOffsetX = x;
        vOL.orbitalOffsetZ = z;
        yield return new WaitForSeconds(.1f);
        x = claimant.position.x - transform.position.x;
        z = claimant.position.z - transform.position.z;
        vOL.orbitalOffsetX = x;
        vOL.orbitalOffsetZ = z;
    }

    private IEnumerator EnsureClaim()
    {
        yield return new WaitForSeconds(.6f);
        var vOL = partSys.velocityOverLifetime;
        vOL.radial = 0;
    }
}
