using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Ravenfield.Mutator.Configuration
{
	public abstract class MutatorConfigurationSortableField
	{
		public string id;
		public string displayName;
		public int orderPriority;
	}

	[System.Serializable]
	public class MutatorConfigurationFieldGeneric<T> : MutatorConfigurationSortableField
	{
		public T value;
	}

	[System.Serializable]
	public class MutatorConfigurationLabel : MutatorConfigurationSortableField { }

	[System.Serializable]
	public class IntegerConfigurationField : MutatorConfigurationFieldGeneric<int> { }

	[System.Serializable]
	public class FloatConfigurationField : MutatorConfigurationFieldGeneric<float> { }

	[System.Serializable]
	public class RangeConfigurationField : MutatorConfigurationFieldGeneric<RangeConfigurationField.FieldData>
	{
		[System.Serializable]
		public class FieldData
		{
			public float value, min, max;
			public bool wholeNumbers;
		}
	}

	[System.Serializable]
	public class StringConfigurationField : MutatorConfigurationFieldGeneric<string> { }

	[System.Serializable]
	public class BoolConfigurationField : MutatorConfigurationFieldGeneric<bool> { }

	[System.Serializable]
	public class DropdownConfigurationField : MutatorConfigurationFieldGeneric<DropdownConfigurationField.FieldData> {
		[System.Serializable]
		public class FieldData
		{
			public int index;
			public string[] labels;
		}
	}
}
