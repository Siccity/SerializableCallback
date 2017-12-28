using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SerializableCallback {

	public class CachedInvokableCallback<TReturn> : InvokableCallback<TReturn> {

		public CachedInvokableCallback(Object target, string methodName) : base(target, methodName) {
		}

		public override TReturn Invoke(params object[] args) {
			return base.Invoke();
		}
	}

	public class CachedInvokableCallback<T0, TReturn> : InvokableCallback<T0, TReturn> {
		private readonly object[] args = new object[1];

		public CachedInvokableCallback(Object target, string methodName, T0 arg0) : base(target, methodName) {
			this.args[0] = arg0;
		}

		public override TReturn Invoke(params object[] args) {
			return base.Invoke(this.args);
		}
	}

	public class CachedInvokableCallback<T0, T1, TReturn> : InvokableCallback<T0, TReturn> {
		private readonly object[] args = new object[2];

		public CachedInvokableCallback(Object target, string methodName, T0 arg0, T1 arg1) : base(target, methodName) {
			this.args[0] = arg0;
			this.args[1] = arg1;
		}

		public override TReturn Invoke(params object[] args) {
			return base.Invoke(this.args);
		}
	}
}