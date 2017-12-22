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
	public Object target { get { return _target; } set { _target = value; invokable = null; } }
	/// <summary> Target method name </summary>
	public string methodName { get { return _methodName; } set { _methodName = value; invokable = null; } }

	[SerializeField] private Object _target;
	[SerializeField] private string _methodName;
	[SerializeField] private Arg[] _args;
	[SerializeField] private bool _dynamic;
	#pragma warning disable 0414
	[SerializeField] private string _typeName;
	#pragma warning restore 0414
	public bool Cached { get { return invokable != null; } }

	[NonSerialized] protected InvokableCallbackBase invokable;

	protected object Invoke(params object[] args) {
		if (target == null) return null;
		if (!Cached) Cache();
		return invokable != null ? invokable.Invoke(args) : null;
	}

	protected SerializableCallbackBase() {
		_typeName =  base.GetType().AssemblyQualifiedName;
	}

	public void SetMethod(Object target, string methodName, bool dynamic, params Arg[] args) {
		_target = target;
		_methodName = methodName;
		_dynamic = dynamic;
		_args = args;
		ClearCache();
	}

	public void Cache() {
		Type targetType = _target.GetType();
		object[] parameters = new object[_args.Length];
		Type[] types = new Type[_args.Length];
		for (int i = 0; i < parameters.Length; i++) {
			parameters[i] = _args[i].GetValue();
			types[i] = Arg.RealType(_args[i].argType);
		}
		MethodInfo methodInfo = targetType.GetMethod(_methodName, types);
		if (methodInfo == null) return;
		if (_dynamic) invokable = GetDelegate(methodInfo, _target);
		else invokable = GetDelegate(methodInfo, _target, parameters);
	}

	public virtual void ClearCache() {
		invokable = null;
	}

	public bool CanInvoke() {
		return target != null;
	}

	/// <summary> Return a delegate with dynamic arguments </summary>
	protected virtual InvokableCallbackBase GetDelegate(MethodInfo methodInfo, object target) {
		return new InvokableCallback(target, methodInfo);
	}

	/// <summary> Return a delegate with constant arguments </summary>
	protected virtual InvokableCallbackBase GetDelegate(MethodInfo methodInfo, object target, object[] args) {
		return new InvokableCallback(target, methodInfo, args);
	}

	public void OnBeforeSerialize() { }

	public void OnAfterDeserialize() {
		invokable = null;
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