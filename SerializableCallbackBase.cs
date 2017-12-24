using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary> An inspector-friendly serializable function </summary>
[System.Serializable]
public abstract class SerializableCallbackBase : ISerializationCallbackReceiver {

	/// <summary> Target object </summary>
	public Object target { get { return _target; } set { _target = value; ClearCache(); } }
	/// <summary> Target method name </summary>
	public string methodName { get { return _methodName; } set { _methodName = value; ClearCache(); } }
	public Arg[] args { get { return _args; } set { _args = value; ClearCache(); } }
	public bool dynamic  { get { return _dynamic; } set { _dynamic = value; ClearCache(); } }

	[SerializeField] protected Object _target;
	[SerializeField] protected string _methodName;
	[SerializeField] protected Arg[] _args;
	[SerializeField] protected bool _dynamic;
#pragma warning disable 0414
	[SerializeField] private string _typeName;
#pragma warning restore 0414
	protected bool cached = false;

	protected SerializableCallbackBase() {
		_typeName = base.GetType().AssemblyQualifiedName;
	}

	public void ClearCache() {
		cached = false;
	}

	public void SetMethod(Object target, string methodName, bool dynamic, params Arg[] args) {
		_target = target;
		_methodName = methodName;
		_dynamic = dynamic;
		_args = args;
		ClearCache();
	}

	protected abstract void Cache();

	public void OnBeforeSerialize() {
		ClearCache();
	}

	public void OnAfterDeserialize() {
		_typeName = base.GetType().AssemblyQualifiedName;
	}
}

[System.Serializable]
public struct Arg {
	public enum ArgType { Unsupported, Bool, Int, Float, String, Object }
	public bool boolValue;
	public int intValue;
	public float floatValue;
	public string stringValue;
	public Object objectValue;
	public ArgType argType;

	public object GetValue() {
		return GetValue(argType);
	}

	public object GetValue(ArgType type) {
		switch (type) {
			case ArgType.Bool:
				return boolValue;
			case ArgType.Int:
				return intValue;
			case ArgType.Float:
				return floatValue;
			case ArgType.String:
				return stringValue;
			case ArgType.Object:
				return objectValue;
			default:
				return null;
		}
	}

	public static Type RealType(ArgType type) {
		switch (type) {
			case ArgType.Bool:
				return typeof(bool);
			case ArgType.Int:
				return typeof(int);
			case ArgType.Float:
				return typeof(float);
			case ArgType.String:
				return typeof(string);
			case ArgType.Object:
				return typeof(Object);
			default:
				return null;
		}
	}

	public static ArgType FromRealType(Type type) {
		if (type == typeof(bool)) return ArgType.Bool;
		else if (type == typeof(int)) return ArgType.Int;
		else if (type == typeof(float)) return ArgType.Float;
		else if (type == typeof(String)) return ArgType.String;
		else if (type == typeof(Object)) return ArgType.Object;
		else return ArgType.Unsupported;
	}

	public static bool IsSupported(Type type) {
		return FromRealType(type) != ArgType.Unsupported;
	}
}