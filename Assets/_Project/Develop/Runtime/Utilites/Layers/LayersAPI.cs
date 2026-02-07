using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites
{
	public static class LayersAPI
	{
		public static readonly int LayerDefault = LayerMask.NameToLayer("Default");
		public static readonly int LayerTransparentFX = LayerMask.NameToLayer("TransparentFX");
		public static readonly int LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
		public static readonly int LayerWater = LayerMask.NameToLayer("Water");
		public static readonly int LayerUI = LayerMask.NameToLayer("UI");
		public static readonly int LayerHittable = LayerMask.NameToLayer("Hittable");
		public static readonly int LayerSpawner = LayerMask.NameToLayer("Spawner");
		public static readonly int LayerBallista = LayerMask.NameToLayer("Ballista");

		public static readonly int LayerMaskDefault = 1 << LayerDefault;
		public static readonly int LayerMaskTransparentFX = 1 << LayerTransparentFX;
		public static readonly int LayerMaskIgnoreRaycast = 1 << LayerIgnoreRaycast;
		public static readonly int LayerMaskWater = 1 << LayerWater;
		public static readonly int LayerMaskUI = 1 << LayerUI;
		public static readonly int LayerMaskHittable = 1 << LayerHittable;
		public static readonly int LayerMaskSpawner = 1 << LayerSpawner;
		public static readonly int LayerMaskBallista = 1 << LayerBallista;

	}
}
