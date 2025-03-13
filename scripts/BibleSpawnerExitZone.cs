
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleSpawnerExitZone : UdonSharpBehaviour
{
    [SerializeField] private Transform spawn_transform;
    [SerializeField] private GameObject prefab_spawn;
    private BiblePickup current_bible_pickup;

    void Start()
    {
        Destroy(spawn_transform.GetChild(0).gameObject);
        SpawnNewBible();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BiblePickup>() != current_bible_pickup) return;
        current_bible_pickup.ClaimHolder();
        SpawnNewBible();
    }

    public void SpawnNewBible()
    {
        current_bible_pickup = Instantiate(prefab_spawn, spawn_transform.position, spawn_transform.rotation).GetComponent<BiblePickup>();
    }
}
