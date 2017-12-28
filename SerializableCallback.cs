using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace SerializableCallback {
	[Serializable]
	public class SerializableCallback : SerializableCallback<bool> { }

	public abstract class SerializableCallback<TReturn> : SerializableCallbackBase<TReturn> {

		public TReturn Invoke() {
			if (invokable == null) {
				if (_dynamic) invokable = GetDelegate(_methodName, _target);
				else invokable = GetDelegate(_methodName, _target, _args);
			}
			return invokable.Invoke();
		}

		protected override InvokableCallbackBase<TReturn> GetDelegate(string methodInfo, object target) {
			return new InvokableCallback<TReturn>(target, methodInfo);
		}
	}

	public abstract class SerializableCallback<T0, TReturn> : SerializableCallbackBase<TReturn> {

		public TReturn Invoke(T0 arg0) {
			if (invokable == null) {
				if (_dynamic) invokable = GetDelegate(_methodName, _target);
				else invokable = GetDelegate(_methodName, _target, _args);
			}
			return invokable.Invoke(arg0);
		}

		protected override InvokableCallbackBase<TReturn> GetDelegate(string methodInfo, object target) {
			return new InvokableCallback<T0, TReturn>(target, methodInfo);
		}
	}
}