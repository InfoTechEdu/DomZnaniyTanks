﻿using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody target = colliders[i].GetComponent<Rigidbody>();
            if (!target)
                continue;

            target.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth tankHealth = target.GetComponent<TankHealth>();
            if (!tankHealth)
                continue;

            float damage = CalculateDamage(target.position);
            tankHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionTarget = targetPosition - transform.position;
        float distance = explosionTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - distance) / m_ExplosionRadius;
        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        // Calculate the amount of damage a target should take based on it's position.
        return damage;
    }
}