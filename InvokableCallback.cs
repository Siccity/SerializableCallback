using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace SerializableCallback {
	public class InvokableCallback<TReturn> : InvokableCallbackBase<TReturn> {
		public Func<TReturn> func;

		public InvokableCallback(object target, string methodInfo, params object[] args) : base(target, methodInfo) {
			func = CacheFunction(methodInfo, target, args);
		}

		public override TReturn Invoke(params object[] args) {
			return func();
		}

		private Func<TReturn> CacheFunction(string methodInfo, object target, object[] arguments) {
			return (System.Func<TReturn>) Delegate.CreateDelegate(typeof(Func<TReturn>), target, methodInfo);
		}
	}

	public class InvokableCallback<T0, TReturn> : InvokableCallbackBase<TReturn> {
		public Func<T0, TReturn> func;

		public InvokableCallback(object target, string methodInfo, params object[] args) : base(target, methodInfo) {
			func = CacheFunction(methodInfo, target);
		}

		public override TReturn Invoke(params object[] args) {
			return func((T0)args[0]);
		}

		private Func<T0, TReturn> CacheFunction(string methodInfo, object target) {
			return (System.Func<T0, TReturn>) Delegate.CreateDelegate(typeof(Func<T0, TReturn>), target, methodInfo);
		}
	}
}