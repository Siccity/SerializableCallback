using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializableCallback {
	public abstract class SerializableCallbackBase<TReturn> : SerializableCallbackBase {
		/// <summary> Target object </summary>
		public Object target { get { return _target; } set { _target = value; invokable = null; } }
		/// <summary> Target method name </summary>
		public string methodName { get { return _methodName; } set { _methodName = value; invokable = null; } }

		public bool Cached { get { return invokable != null; } }

		[NonSerialized] protected InvokableCallbackBase<TReturn> invokable;

		public virtual void ClearCache() {
			invokable = null;
		}

		public void SetMethod(Object target, string methodName, bool dynamic, params Arg[] args) {
			_target = target;
			_methodName = methodName;
			_dynamic = dynamic;
			_args = args;
			ClearCache();
		}

		/// <summary> Return a delegate with dynamic arguments </summary>
		protected abstract InvokableCallbackBase<TReturn> GetDelegate(string methodInfo, object target);

		/// <summary> Return a delegate with constant arguments </summary>
		protected InvokableCallbackBase<TReturn> GetDelegate(string methodInfo, object target, Arg[] args) {
			Type[] genericParms = args.Select(x => Arg.RealType(x.argType)).Concat(new Type[] { typeof(TReturn) }).ToArray();
			object[] _args = args.Select(x => x.GetValue()).ToArray();
			if (genericParms.Length == 1) {
				Type cb = typeof(CachedInvokableCallback<>).MakeGenericType(genericParms);
				return (InvokableCallbackBase<TReturn>) Activator.CreateInstance(cb, target, methodName);
			} else if (genericParms.Length == 2) {
				Type cb = typeof(CachedInvokableCallback<,>).MakeGenericType(genericParms);
				return (InvokableCallbackBase<TReturn>) Activator.CreateInstance(cb, target, methodName, _args[0]);
			} else if (genericParms.Length == 3) {
				Type cb = typeof(CachedInvokableCallback<, ,>).MakeGenericType(genericParms);
				return (InvokableCallbackBase<TReturn>) Activator.CreateInstance(cb, target, methodName, _args[0], _args[1]);
			}
			Debug.LogError("Too many arguments! - "+_args.Length);
			return null;
		}

		/*public void Cache() {
			if (_dynamic) invokable = GetDelegate(_methodName, _target);
			else invokable = GetDelegate(_methodName, _target, _args);
		}*/

		public override void OnAfterDeserialize() {
			invokable = null;
			base.OnAfterDeserialize();
		}
	}

	/// <summary> An inspector-friendly serializable function </summary>
	[System.Serializable]
	public abstract class SerializableCallbackBase : ISerializationCallbackReceiver {

		[SerializeField] protected Object _target;
		[SerializeField] protected string _methodName;
		[SerializeField] protected Arg[] _args;
		[SerializeField] protected bool _dynamic;
#pragma warning disable 0414
		[SerializeField] protected string _typeName;
#pragma warning restore 0414

		protected SerializableCallbackBase() {
			_typeName = base.GetType().AssemblyQualifiedName;
		}

		public bool CanInvoke() {
			return _target != null;
		}

		public virtual void OnBeforeSerialize() { }

		public virtual void OnAfterDeserialize() {
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
}