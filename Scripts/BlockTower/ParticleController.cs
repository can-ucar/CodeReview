using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem m_BlockBlastParticle;
    public ParticleSystem m_BlockFallenParticle;
    public ParticleSystem m_BlockRenewParticle;

    public ParticleSystem m_ChestGlow;
    public Animator m_ChestAnimator;

    public Material m_Material;

    public void PlayBlastParticle(Vector3 pos)
    {
        ParticleSystem particle = Instantiate(m_BlockBlastParticle);
        particle.transform.position = pos;
        particle.GetComponent<Renderer>().material = m_Material;
        particle.Play();
    }
    
    public void PlayFallenParticle(Vector3 pos)
    {
        ParticleSystem particle = Instantiate(m_BlockFallenParticle);
        particle.transform.position = pos;
        particle.GetComponent<Renderer>().material = m_Material;
        particle.Play();
    }
    
    public void PlayRenewParticle(Vector3 pos)
    {
        ParticleSystem particle = Instantiate(m_BlockRenewParticle);
        particle.transform.position = pos;
        particle.GetComponent<Renderer>().material = m_Material;
        particle.Play();
    }

    public void PlayChestParticleAndAnimation(Vector3 pos)
    {
        m_ChestAnimator.Play("ChestOpen");
        
        ParticleSystem particle = Instantiate(m_ChestGlow);
        pos.z += -0.1f;
        particle.transform.position = pos;
        particle.Play();
    }
}
