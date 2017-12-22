using System;
using System.Linq.Expressions;
using System.Reflection;

[Serializable]
public class SerializableCallback : SerializableCallbackBase { 
	public object Invoke() {
		object o = base.Invoke();
		if (o != null) return o;
		else return null;
	}
}

public abstract class SerializableCallback<TReturn> : SerializableCallbackBase {
	public TReturn Invoke() {
		object o = base.Invoke();
		if (o != null) return (TReturn) o;
		else return default(TReturn);
	}
}

public abstract class SerializableCallback<T0, TReturn> : SerializableCallbackBase {
	public TReturn Invoke(T0 arg0) {
		object o = base.Invoke(arg0);
		if (o != null) return (TReturn) o;
		else return default(TReturn);
	}

	protected override InvokableCallbackBase GetDelegate(MethodInfo methodInfo, object target) {
		return new InvokableCallback<T0>(target, methodInfo);
	}
}

public abstract class SerializableCallback<T0, T1, TReturn> : SerializableCallbackBase {
	public TReturn Invoke(T0 arg0, T1 arg1) {
		object o = base.Invoke(arg0, arg1);
		if (o != null) return (TReturn) o;
		else return default(TReturn);
	}

	protected override InvokableCallbackBase GetDelegate(MethodInfo methodInfo, object target) {
		return new InvokableCallback<T0, T1>(target, methodInfo);
	}
}

public abstract class SerializableCallback<T0, T1, T2, TReturn> : SerializableCallbackBase {
	public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2) {
		object o = base.Invoke(arg0, arg1, arg2);
		if (o != null) return (TReturn) o;
		else return default(TReturn);
	}

	protected override InvokableCallbackBase GetDelegate(MethodInfo methodInfo, object target) {
		return new InvokableCallback<T0, T1, T2>(target, methodInfo);
	}
}

public abstract class SerializableCallback<T0, T1, T2, T3, TReturn> : SerializableCallbackBase {
	public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3) {
		object o = base.Invoke(arg0, arg1, arg2, arg3);
		if (o != null) return (TReturn) o;
		else return default(TReturn);
	}

	protected override InvokableCallbackBase GetDelegate(MethodInfo methodInfo, object target) {
		return new InvokableCallback<T0, T1, T2, T3>(target, methodInfo);
	}
}