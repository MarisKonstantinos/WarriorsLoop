using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [Header("Particle Prefabs")]
    [SerializeField] private ParticleSystem hitParticlesPrefab;
    [SerializeField] private ParticleSystem healingItemDestroyParticlesPrefab;
    [SerializeField] private ParticleSystem moveParticlePrefab;

    private ObjectPool<ParticleSystem> healingItemDestroyPool;
    private ObjectPool<ParticleSystem> hitParticlePool;
    private ObjectPool<ParticleSystem> moveParticlePool;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        healingItemDestroyPool = CreatePool(healingItemDestroyParticlesPrefab, 3, 10);
        hitParticlePool = CreatePool(hitParticlesPrefab, 1, 15);
        moveParticlePool = CreatePool(moveParticlePrefab, 1, 10);
    }

    public void ToggleLoopingParticle(ParticleSystem ps,bool toogle)
    {
        if(toogle)
            ps.Play();
        else
            ps.Stop();
    }

    public void PlaySimpleParticle(ParticleSystem particle,Transform parent = null)
    {
        ParticleSystem ps = Instantiate(particle,parent);
        ps.Play();

        StartCoroutine(DestroyParticle(ps));
    }

    IEnumerator DestroyParticle(ParticleSystem ps)
    {
        yield return new WaitForSeconds(ps.main.duration);
        Destroy(ps.gameObject);
    }

    private ObjectPool<ParticleSystem> CreatePool(ParticleSystem prefab, int defaultCapacity, int maxSize)
    {
        return new ObjectPool<ParticleSystem>(
            () => Instantiate(prefab),                    // create
            ps => ps.gameObject.SetActive(true),          // on get
            ps => ps.gameObject.SetActive(false),         // on release
            ps => Destroy(ps.gameObject),                 // on destroy
            false, defaultCapacity, maxSize               // collectionCheck, defaultCapacity, maxSize
        );
    }

    public void PlayBoxDestroyParticles(Vector2 position)
    {
        PlayFromPool(healingItemDestroyPool, position);
    }

    public void PlayHitParticle(Vector2 position)
    {
        PlayFromPool(hitParticlePool, position);
    }

    private void PlayFromPool(ObjectPool<ParticleSystem> pool, Vector2 position)
    {
        ParticleSystem ps = pool.Get();
        ps.transform.position = position;
        ps.Play();

        //return to pool when done
        StartCoroutine(ReturnToPool(ps,pool,ps.main.duration));
    }

    private IEnumerator ReturnToPool(ParticleSystem ps, ObjectPool<ParticleSystem> pool,float duration)
    {
        yield return new WaitForSeconds(duration);
        pool.Release(ps);
    }
}
