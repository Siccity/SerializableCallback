using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class InvokableCallback : InvokableCallbackBase {
	public Func<object> func;

	public InvokableCallback(object target, MethodInfo methodInfo, params object[] args) : base(target, methodInfo) {
		func = CacheFunction(methodInfo, target, args);
	}

	public override object Invoke(params object[] args) {
		return func();
	}

	private Func<object> CacheFunction(MethodInfo methodInfo, object target, object[] arguments) {
		ConstantExpression instance = Expression.Constant(target);
		ConstantExpression[] args = arguments.Select(x => Expression.Constant(x)).ToArray();
		MethodCallExpression call = Expression.Call(instance, methodInfo, args);
		ConstantExpression constNull = Expression.Constant(null);
		UnaryExpression convert = Expression.Convert(call, typeof(object));
		Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(convert);
		return lambda.Compile();
	}
}

public class InvokableCallback<T0> : InvokableCallbackBase {
	public Func<T0, object> func;

	public InvokableCallback(object target, MethodInfo methodInfo, params object[] args) : base(target, methodInfo) {
		func = CacheFunction(methodInfo, target);
	}

	public override object Invoke(params object[] args) {
		return func((T0) args[0]);
	}

	private Func<T0, object> CacheFunction(MethodInfo methodInfo, object target) {
		ConstantExpression instance = Expression.Constant(target);
		ParameterExpression[] parms = new ParameterExpression[] {
			Expression.Parameter(typeof(T0), "arg0")
		};
		MethodCallExpression call = Expression.Call(instance, methodInfo, parms);
		UnaryExpression convert = Expression.Convert(call, typeof(object));
		Expression<Func<T0, object>> lambda = Expression.Lambda<Func<T0, object>>(convert, parms);
		return lambda.Compile();
	}
}

public class InvokableCallback<T0, T1> : InvokableCallbackBase {
	public Func<T0, T1, object> func;

	public InvokableCallback(object target, MethodInfo methodInfo, params object[] args) : base(target, methodInfo) {
		func = CacheFunction(methodInfo, target);
	}

	public override object Invoke(params object[] args) {
		return func((T0) args[0], (T1) args[1]);
	}

	private Func<T0, T1, object> CacheFunction(MethodInfo methodInfo, object target) {
		ConstantExpression instance = Expression.Constant(target);
		ParameterExpression[] parms = new ParameterExpression[] {
			Expression.Parameter(typeof(T0), "arg0"),
				Expression.Parameter(typeof(T1), "arg1")
		};
		MethodCallExpression call = Expression.Call(instance, methodInfo, parms);
		UnaryExpression convert = Expression.Convert(call, typeof(object));
		Expression<Func<T0, T1, object>> lambda = Expression.Lambda<Func<T0, T1, object>>(convert, parms);
		return lambda.Compile();
	}
}

public class InvokableCallback<T0, T1, T2> : InvokableCallbackBase {
	public Func<T0, T1, T2, object> func;

	public InvokableCallback(object target, MethodInfo methodInfo, params object[] args) : base(target, methodInfo) {
		func = CacheFunction(methodInfo, target);
	}

	public override object Invoke(params object[] args) {
		return func((T0) args[0], (T1) args[1], (T2) args[1]);
	}

	private Func<T0, T1, T2, object> CacheFunction(MethodInfo methodInfo, object target) {
		ConstantExpression instance = Expression.Constant(target);
		ParameterExpression[] parms = new ParameterExpression[] {
			Expression.Parameter(typeof(T0), "arg0"),
				Expression.Parameter(typeof(T1), "arg1"),
				Expression.Parameter(typeof(T2), "arg2")
		};
		MethodCallExpression call = Expression.Call(instance, methodInfo, parms);
		UnaryExpression convert = Expression.Convert(call, typeof(object));
		Expression<Func<T0, T1, T2, object>> lambda = Expression.Lambda<Func<T0, T1, T2, object>>(convert, parms);
		return lambda.Compile();
	}
}

public class InvokableCallback<T0, T1, T2, T3> : InvokableCallbackBase {
	public Func<T0, T1, T2, T3, object> func;

	public InvokableCallback(object target, MethodInfo methodInfo, params object[] args) : base(target, methodInfo) {
		func = CacheFunction(methodInfo, target);
	}

	public override object Invoke(params object[] args) {
		return func((T0) args[0], (T1) args[1], (T2) args[2], (T3) args[3]);
	}

	private Func<T0, T1, T2, T3, object> CacheFunction(MethodInfo methodInfo, object target) {
		ConstantExpression instance = Expression.Constant(target);
		ParameterExpression[] parms = new ParameterExpression[] {
			Expression.Parameter(typeof(T0), "arg0"),
				Expression.Parameter(typeof(T1), "arg1"),
				Expression.Parameter(typeof(T2), "arg2"),
				Expression.Parameter(typeof(T3), "arg3")
		};
		MethodCallExpression call = Expression.Call(instance, methodInfo, parms);
		UnaryExpression convert = Expression.Convert(call, typeof(object));
		Expression<Func<T0, T1, T2, T3, object>> lambda = Expression.Lambda<Func<T0, T1, T2, T3, object>>(convert, parms);
		return lambda.Compile();
	}
}