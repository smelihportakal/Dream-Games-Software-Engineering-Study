
using System.Collections;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartCubeParticle(int x, int y, Material mat)
    {
        StartCoroutine(CubeParticleCoroutine(x, y, mat));
    }

    public IEnumerator CubeParticleCoroutine(int x, int y, Material mat)
    {
        GameObject particles = ObjectPooler.Instance.SpawnFromPool("cube_particle", GameManager.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity);
        particles.GetComponent<ParticleSystemRenderer>().material = mat;
        yield return new WaitForSecondsRealtime(1f);

        ObjectPooler.Instance.ReturnObjectToPool("cube_particle", particles);
    }
    
    public void StartTntParticle(int x, int y, GameObject prefab)
    {
        StartCoroutine(TntParticleCoroutine(x, y, prefab));
    }

    public IEnumerator TntParticleCoroutine(int x, int y, GameObject prefab )
    {

        GameObject particles = Instantiate(prefab, GameManager.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        yield return new WaitForSecondsRealtime(1f);
        Destroy(particles);
    }
    
    public void StartObstacleParticle(int x, int y, GameObject prefab)
    {
        StartCoroutine(ObstacleParticleCoroutine(x, y, prefab));
    }

    public IEnumerator ObstacleParticleCoroutine(int x, int y, GameObject prefab )
    {

        GameObject particles = Instantiate(prefab, GameManager.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        yield return new WaitForSecondsRealtime(5f);
        Destroy(particles);
    }

}
