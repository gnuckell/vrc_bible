
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BiblePickup : UdonSharpBehaviour
{
	[SerializeField] public BibleOwner owner;

	[HideInInspector] public BibleSpawnerExitZone spawner;

    public void ClaimHolder()
	{
		owner.Claim(Networking.GetOwner(gameObject));
		// owner.ClaimLocal();
	}

	public void ReturnToSpawner()
	{
		if (owner.is_owner_or_unclaimed) return;

		owner.Unclaim();
		spawner.ReturnUsedBible(gameObject);
	}
}
