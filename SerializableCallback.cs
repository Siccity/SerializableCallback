using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
[Serializable]

public class SerializableCallback : SerializableCallbackBase {
	public Action func;

	public void Invoke() {
		if (!cached) Cache();
		func.Invoke();
	}

	protected override void Cache() {
		func = (System.Action) System.Delegate.CreateDelegate(typeof(System.Action), _target, _methodName);
	}
}

public abstract class SerializableCallback<TReturn> : SerializableCallbackBase {
	public Func<TReturn> func;

	public TReturn Invoke() {
		if (!cached) Cache();
		return func.Invoke();
	}

	protected override void Cache() {
		func = (System.Func<TReturn>) System.Delegate.CreateDelegate(typeof(System.Func<TReturn>), _target, _methodName);
		cached = true;
	}
}

public abstract class SerializableCallback<T0, TReturn> : SerializableCallbackBase {
	public Func<T0, TReturn> func;
	public T0 arg0;

	public TReturn Invoke(T0 arg0) {
		if (!cached) Cache();
		if (_dynamic) return func.Invoke(arg0);
		else return func.Invoke(this.arg0);
	}

	protected override void Cache() {
		func = (System.Func<T0, TReturn>) System.Delegate.CreateDelegate(typeof(System.Func<T0, TReturn>), _target, _methodName);
		arg0 = (T0) _args[0].GetValue(Arg.FromRealType(typeof(T0)));
		cached = true;
	}
}

public abstract class SerializableCallback<T0, T1, TReturn> : SerializableCallbackBase {
	public Func<T0, T1, TReturn> func;
	public T0 arg0;
	public T1 arg1;

	public TReturn Invoke(T0 arg0, T1 arg1) {
		if (!cached) Cache();
		if (_dynamic) return func.Invoke(arg0, arg1);
		else return func.Invoke(this.arg0, this.arg1);
	}

	protected override void Cache() {
		func = (System.Func<T0, T1, TReturn>) System.Delegate.CreateDelegate(typeof(System.Func<T0, T1, TReturn>), _target, _methodName);
		arg0 = (T0) _args[0].GetValue(Arg.FromRealType(typeof(T0)));
		arg1 = (T1) _args[1].GetValue(Arg.FromRealType(typeof(T1)));
		cached = true;
	}
}

public abstract class SerializableCallback<T0, T1, T2, TReturn> : SerializableCallbackBase {
	public Func<T0, T1, T2, TReturn> func;
	public T0 arg0;
	public T1 arg1;
	public T2 arg2;

	public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2) {
		if (!cached) Cache();
		if (_dynamic) return func.Invoke(arg0, arg1, arg2);
		else return func.Invoke(this.arg0, this.arg1, this.arg2);
	}

	protected override void Cache() {
		func = (System.Func<T0, T1, T2, TReturn>) System.Delegate.CreateDelegate(typeof(System.Func<T0, T1, T2, TReturn>), _target, _methodName);
		arg0 = (T0) _args[0].GetValue(Arg.FromRealType(typeof(T0)));
		arg1 = (T1) _args[1].GetValue(Arg.FromRealType(typeof(T1)));
		arg2 = (T2) _args[2].GetValue(Arg.FromRealType(typeof(T2)));
		cached = true;
	}
}