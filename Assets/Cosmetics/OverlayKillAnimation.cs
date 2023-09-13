using System;
using System.Collections;
using System.Collections.Generic;
using PowerTools;
using UnityEngine;

public abstract class OverlayAnimation : MonoBehaviour
{
	protected const float TwoFramesDelay = 0.083333336f;
}

public class OverlayKillAnimation : OverlayAnimation
{
	private void OnDestroy()
	{
		this.petObjects = null;
	}

	[SerializeField]
	private Vector3 VictimPetPosition = new Vector3(0.282f, -0.37f, 0.1f);

	[SerializeField]
	private AudioClip Stinger;

	[SerializeField]
	private AudioClip Sfx;

	[SerializeField]
	private float StingerVolume = 0.6f;
	private string victimHat;
	private HashSet<GameObject> petObjects;
}