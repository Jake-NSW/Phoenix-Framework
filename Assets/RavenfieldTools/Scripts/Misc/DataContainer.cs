using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ravenfield.Mods.Data
{
	public class DataContainer : MonoBehaviour
	{
		public BoolEntry[] bools;
		public IntEntry[] ints;
		public FloatEntry[] floats;
		public StringEntry[] strings;

		public VectorEntry[] vectors;
		public RotationEntry[] rotations;

		public TextureEntry[] textures;
		public SpriteEntry[] sprites;
		public AudioClipEntry[] audioClips;
		public MaterialEntry[] materials;

		public GameObjectEntry[] gameObjects;
		public ActorSkinEntry[] skins;
		public WeaponEntryEntry[] weaponEntries;
	}

	[System.Serializable] public class BoolEntry : DataEntry<bool> { }
	[System.Serializable] public class IntEntry : DataEntry<int> { }
	[System.Serializable] public class FloatEntry : DataEntry<float> { }
	[System.Serializable] public class StringEntry : DataEntry<string> { }

	[System.Serializable] public class VectorEntry : DataEntry<Vector3> { }
	[System.Serializable] public class RotationEntry : DataEntry<Quaternion> { }

	[System.Serializable] public class TextureEntry : DataEntry<Texture> { }
	[System.Serializable] public class SpriteEntry : DataEntry<Sprite> { }
	[System.Serializable] public class AudioClipEntry : DataEntry<AudioClip> { }
	[System.Serializable] public class MaterialEntry : DataEntry<Material> { }

	[System.Serializable] public class GameObjectEntry : DataEntry<UnityEngine.GameObject> { }
	[System.Serializable] public class ActorSkinEntry : DataEntry<ActorSkin> { }
	[System.Serializable] public class WeaponEntryEntry : DataEntry<WeaponManager.WeaponEntry> { }

	[System.Serializable]
	public class DataEntry<T> : DataEntryBase
	{
		public T value;
	}

	[System.Serializable]
	public abstract class DataEntryBase
	{
		public string id;
	}
}
