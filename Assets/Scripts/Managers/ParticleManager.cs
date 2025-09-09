using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    [Header("Particle Prefabs")]
    //[SerializeField] private ParticleSystem dashParticlesPrefab;
    [SerializeField] private ParticleSystem boxDestroyParticlesPrefab;

    private ObjectPool<ParticleSystem> boxDestroyPool;

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
        boxDestroyPool = CreatePool(boxDestroyParticlesPrefab, 3, 10);
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
        PlayFromPool(boxDestroyPool, position);
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
