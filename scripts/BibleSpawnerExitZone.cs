
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleSpawnerExitZone : UdonSharpBehaviour
{
    [SerializeField] private Transform spawn_transform;
    [SerializeField] private GameObject prefab_spawn;
    private GameObject current_spawn;

    void Start()
    {
        Destroy(spawn_transform.GetChild(0).gameObject);
        SpawnNewBible();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != current_spawn) return;
        SpawnNewBible();
    }

    public void SpawnNewBible()
    {
        current_spawn = Instantiate(prefab_spawn, spawn_transform.position, spawn_transform.rotation);
    }
}
