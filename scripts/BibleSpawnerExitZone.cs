
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleSpawnerExitZone : UdonSharpBehaviour
{
    [SerializeField] private int pool_start_offset = 0;

    [SerializeField] private Transform spawn_transform;
    [SerializeField] private GameObject prefab_spawn;

    [SerializeField] private GameObject master_object;

    private GameObject[] object_pool;

    [UdonSynced] private uint pool_bitmask = 0u;
    [UdonSynced] private  int pool_current = 0;

    public GameObject current_object {
        get => object_pool[pool_current];
        private set => pool_current = IndexOf(value);
    }

    private void Start()
    {
        if (!Networking.IsMaster)
            Destroy(master_object);

        if (spawn_transform.childCount == 0) return;

        object_pool = new GameObject[spawn_transform.childCount];
        for (var i = 0; i < object_pool.Length; i++)
        {
            object_pool[i] = spawn_transform.GetChild((i + pool_start_offset) % object_pool.Length).gameObject;
            if (Networking.IsMaster)
                SetPoolIndexActive(i, i == 0);
            else
                DesPoolIndexActive(i);

            var bible_pickup = object_pool[i].GetComponent<BiblePickup>();
            if (bible_pickup == null) continue;

            bible_pickup.spawner = this;
        }

        if (Networking.IsMaster)
            RequestSerialization();
    }

    public override void OnDeserialization()
    {
        for (var i = 0; i < object_pool.Length; i++)
            DesPoolIndexActive(i);
    }

    public void Sync()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != current_object) return;

        if (Networking.IsOwner(other.gameObject))
        {
            if (current_object.GetComponent<BiblePickup>() != null)
                current_object.GetComponent<BiblePickup>().ClaimHolder();

            current_object = GetNextAvailable();
            SetPoolObjectActive(current_object, true);

            Sync();
        }
    }

    public void ReturnUsedBible(GameObject obj)
    {
        if (current_object == null || current_object.GetComponent<BiblePickup>() == null)
        {
            if (current_object != null)
                SetPoolObjectActive(current_object, false);
            current_object = obj;
        }

        SetPoolObjectActive(obj, current_object == obj);
        obj.transform.SetPositionAndRotation(spawn_transform.position, spawn_transform.rotation);

        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }

    public void ReturnAll()
    {
        for (var i = 0; i < object_pool.Length; i++)
        {
            var pickup = object_pool[i].GetComponent<BiblePickup>();
            if (pickup == null) continue;

            pickup.ReturnToSpawner();
        }
    }

    private void SetPoolObjectActive(GameObject obj, bool active)
    {
        if (obj == null) return;
        SetPoolIndexActive(IndexOf(obj), active);
    }

    private void SetPoolIndexActive(int i, bool active)
    {
        object_pool[i].SetActive(active);
        pool_bitmask = active ? (pool_bitmask | 1u << i) : (pool_bitmask & ~(1u << i));

        // Debug.Log($"\n\nSetPoolObjectActive ::\n\nPool owner: {Networking.GetOwner(gameObject).displayName} ({(Networking.IsOwner(gameObject) ? "you!" : "someone else")})\nthe object in question: {obj}\npool bitmask: {pool_bitmask}\nbitmask position: {i}\nbitmask bit: {pool_bitmask & (1u << i)}\nobject active?: {active}\nbitmask active?: {(pool_bitmask & (1u << i)) != 0}\n\n");
    }

    private void DesPoolIndexActive(int i)
    {
        object_pool[i].SetActive((pool_bitmask & (1 << i)) != 0);
    }

    private GameObject GetNextAvailable()
    {
        foreach (GameObject obj in object_pool)
            if (!obj.activeSelf) return obj;
        return null;
    }

    private int IndexOf(GameObject obj)
    {
        for (var i = 0; i < object_pool.Length; i++)
            if (object_pool[i] == obj) return i;
        return -1;
    }
}
