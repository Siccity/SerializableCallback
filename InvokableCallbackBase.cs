using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class InvokableCallbackBase {

	protected InvokableCallbackBase(object target, MethodInfo function) {
		if (target == null) throw new ArgumentNullException("target");
		if (function == null) throw new ArgumentNullException("function");
	}

	public abstract object Invoke(params object[] args);

	protected static void ThrowOnInvalidArg<T>(object arg) {
		if (arg != null && !(arg is T)) {
			throw new ArgumentException("Passed argument 'args[0]' is of the wrong type. Type:" + arg.GetType() + " Expected:" + typeof(T));
		}
	}
}