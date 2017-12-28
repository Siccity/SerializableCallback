using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SerializableCallback {
	public abstract class InvokableCallbackBase<TReturn> {

		protected InvokableCallbackBase(object target, string methodInfo) {
			if (target == null) throw new ArgumentNullException("target");
			if (methodInfo == null) throw new ArgumentNullException("methodInfo");
		}

		protected static void ThrowOnInvalidArg<T>(object arg) {
			if (arg != null && !(arg is T)) {
				throw new ArgumentException("Passed argument 'args[0]' is of the wrong type. Type:" + arg.GetType() + " Expected:" + typeof(T));
			}
		}

		public abstract TReturn Invoke(params object[] args);
	}
}