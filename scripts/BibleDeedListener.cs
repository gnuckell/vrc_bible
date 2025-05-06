
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleDeedListener : UdonSharpBehaviour
{
    [SerializeField] public BibleDeed deed;

    protected virtual void Start()
    {
        // Refresh();
    }

    public override void OnDeserialization()
    {
        // Refresh();
    }

    public virtual void Sync()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        // Refresh();
        RequestSerialization();
    }

    public virtual void Refresh() { }
}
