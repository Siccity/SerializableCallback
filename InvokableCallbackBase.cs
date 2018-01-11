using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InvokableCallbackBase<TReturn> {
	public abstract TReturn Invoke(params object[] args);
}