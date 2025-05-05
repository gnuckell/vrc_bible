
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BiblePickup : UdonSharpBehaviour
{
	[SerializeField] public BibleDeed deed;

	[HideInInspector] public BibleSpawnerExitZone spawner;

    public void ClaimHolder()
	{
		// Set the deed's claimant to the player holding THIS pickup.
		deed.claimant = Networking.GetOwner(gameObject);
	}


	public void TryReturnToSpawner()
	{
		if (deed.is_claimed_by_other) return;

		ReturnToSpawner();
	}

	public void ReturnToSpawner()
	{
		deed.Unclaim();
		spawner.ReturnUsedBible(gameObject);
	}
}
