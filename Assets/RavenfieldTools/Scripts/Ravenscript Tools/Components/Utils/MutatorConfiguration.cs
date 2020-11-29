using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Ravenfield.Mutator.Configuration
{
	[System.Serializable]
	public class MutatorConfiguration {
		public MutatorConfigurationLabel[] labels;
		public IntegerConfigurationField[] integers;
		public FloatConfigurationField[] floats;
		public RangeConfigurationField[] ranges;
		public StringConfigurationField[] strings;
		public BoolConfigurationField[] bools;
		public DropdownConfigurationField[] dropdowns;

		

		public bool HasAnyFields() {
			foreach(var field in GetAllFields()) {
				return true;
			}
			return false;
		}

		public IEnumerable<MutatorConfigurationSortableField> GetAllFields() {
			try {
				var fields = Enumerable.Empty<MutatorConfigurationSortableField>();
				ConcatField(ref fields, this.labels);
				ConcatField(ref fields, this.integers);
				ConcatField(ref fields, this.floats);
				ConcatField(ref fields, this.ranges);
				ConcatField(ref fields, this.strings);
				ConcatField(ref fields, this.bools);
				ConcatField(ref fields, this.dropdowns);

				return fields;
			}
			catch(System.Exception) {
				return Enumerable.Empty<MutatorConfigurationSortableField>();
			}
		}

		void ConcatField(ref IEnumerable<MutatorConfigurationSortableField> allFields, MutatorConfigurationSortableField[] fields) {
			if(fields != null) {
				allFields = allFields.Concat(fields);
			}
		}
	}
}