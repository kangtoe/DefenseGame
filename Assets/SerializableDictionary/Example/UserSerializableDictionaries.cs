using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> 
{
    public StringIntDictionary() : base() { }
	public StringIntDictionary(IDictionary<string, int> dict) : base(dict) {}

	protected StringIntDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

[Serializable]
public class StringStringDictionary : SerializableDictionary<string, string> {}

[Serializable]
public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> {}

[Serializable]
public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> {}

[Serializable]
public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> {}