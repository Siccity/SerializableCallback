using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableCallbackBase), true)]
public class SerializableCallbackDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		GUI.Box(position, "", (GUIStyle)
			"flow overlay box");
		position.y += 4;
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);
		// Draw label
		Rect pos2 = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		Rect targetRect = new Rect(pos2.x, pos2.y, pos2.width, EditorGUIUtility.singleLineHeight);

		// Get target
		SerializedProperty targetProp = property.FindPropertyRelative("_target");
		object target = targetProp.objectReferenceValue;
		EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);

		if (target != null) {
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel++;

			// Get method name
			SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
			string methodName = methodProp.stringValue;

			// Get args
			SerializedProperty argProps = property.FindPropertyRelative("_args");
			Type[] argTypes = GetArgTypes(argProps);

			// Get dynamic
			SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
			bool dynamic = dynamicProp.boolValue;

			// Get active method
			MethodInfo activeMethod = GetMethod(target, methodName, argTypes);

			GUIContent methodlabel = new GUIContent("n/a");
			if (activeMethod != null) methodlabel = new GUIContent(PrettifyMethod(activeMethod));

			Rect methodRect = new Rect(position.x, targetRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

			// Method select button
			pos2 = EditorGUI.PrefixLabel(methodRect, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(dynamic ? "Method (dynamic)" : "Method"));
			if (EditorGUI.DropdownButton(pos2, methodlabel, FocusType.Keyboard)) {
				MethodSelector(property);
			}

			if (activeMethod != null && !dynamic) {
				// Args
				ParameterInfo[] activeParameters = activeMethod.GetParameters();
				Rect argRect = new Rect(position.x, methodRect.max.y + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
				string[] types = new string[argProps.arraySize];
				for (int i = 0; i < types.Length; i++) {
					SerializedProperty argProp = argProps.FindPropertyRelative("Array.data[" + i + "]");
					GUIContent argLabel = new GUIContent(ObjectNames.NicifyVariableName(activeParameters[i].Name));

					EditorGUI.BeginChangeCheck();
					switch ((Arg.ArgType) argProp.FindPropertyRelative("argType").enumValueIndex) {
						case Arg.ArgType.Bool:
						EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("boolValue"), argLabel);
						break;
						case Arg.ArgType.Int:
						EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("intValue"), argLabel);
						break;
						case Arg.ArgType.Float:
						EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("floatValue"), argLabel);
						break;
						case Arg.ArgType.String:
						EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("stringValue"), argLabel);
						break;
						case Arg.ArgType.Object:
						EditorGUI.PropertyField(argRect, argProp.FindPropertyRelative("objectValue"), argLabel);
						break;
					}

					argRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				}
			}
			EditorGUI.indentLevel = indent;
		}

		// Set indent back to what it was
		EditorGUI.EndProperty();
	}

	private class MenuItem {
		public GenericMenu.MenuFunction action;
		public string path;
		public GUIContent label;

		public MenuItem(string path, string name, GenericMenu.MenuFunction action) {
			this.action = action;
			this.label = new GUIContent(path + '/' + name);
			this.path = path;
		}
	}
	void MethodSelector(SerializedProperty property) {
		// Return type constraint
		Type returnType = null;
		// Arg type constraint
		Type[] argTypes = new Type[0];

		// Get return type and argument constraints
		SerializableCallbackBase dummy = GetDummyFunction(property);
		Type[] genericTypes = dummy.GetType().BaseType.GetGenericArguments();
		if (genericTypes != null && genericTypes.Length > 0) {
			// The last generic argument is the return type
			returnType = genericTypes[genericTypes.Length - 1];
			if (genericTypes.Length > 1) {
				argTypes = new Type[genericTypes.Length - 1];
				Array.Copy(genericTypes, argTypes, genericTypes.Length - 1);
			}
		}

		SerializedProperty targetProp = property.FindPropertyRelative("_target");
		MethodInfo[] methods = targetProp.objectReferenceValue.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

		List<MenuItem> dynamicItems = new List<MenuItem>();
		List<MenuItem> staticItems = new List<MenuItem>();

		for (int i = 0; i < methods.Length; i++) {
			MethodInfo method = methods[i];

			// Skip methods with wrong return type
			if (returnType != null && method.ReturnType != returnType) continue;
			// Skip methods with null return type
			if (method.ReturnType == typeof(void)) continue;
			// Skip generic methods
			if (method.IsGenericMethod) continue;

			Type[] parms = method.GetParameters().Select(x => x.ParameterType).ToArray();

			// Skip methods with more than 4 args
			if (parms.Length > 4) continue;
			// Skip methods with unsupported args
			if (parms.Any(x => !Arg.IsSupported(x))) continue;

			string methodPrettyName = PrettifyMethod(methods[i]);
			staticItems.Add(new MenuItem(methods[i].DeclaringType.Name, methodPrettyName, () => SetMethod(property, method, false)));

			// Skip methods with wrong constrained args
			if (argTypes.Length == 0 || !Enumerable.SequenceEqual(argTypes, parms)) continue;

			dynamicItems.Add(new MenuItem(methods[i].DeclaringType.Name, methods[i].Name, () => SetMethod(property, method, true)));
		}

		// Construct and display context menu
		GenericMenu menu = new GenericMenu();
		if (dynamicItems.Count > 0) {
			string[] paths = dynamicItems.GroupBy(x => x.path).Select(x => x.First().path).ToArray();
			foreach (string path in paths) {
				menu.AddItem(new GUIContent(path + "/Dynamic " + PrettifyTypes(argTypes)), false, null);
			}
			for (int i = 0; i < dynamicItems.Count; i++) {
				menu.AddItem(dynamicItems[i].label, false, dynamicItems[i].action);
			}
			foreach (string path in paths) {
				menu.AddItem(new GUIContent(path + "/  "), false, null);
				menu.AddItem(new GUIContent(path + "/Static parameters"), false, null);
			}
		}
		for (int i = 0; i < staticItems.Count; i++) {
			menu.AddItem(staticItems[i].label, false, staticItems[i].action);
		}
		menu.ShowAsContext();
	}

	string PrettifyMethod(MethodInfo methodInfo) {
		if (methodInfo == null) throw new ArgumentNullException("methodInfo");
		ParameterInfo[] parms = methodInfo.GetParameters();
		string parmnames = PrettifyTypes(parms.Select(x => x.ParameterType).ToArray());
		return GetTypeName(methodInfo.ReturnParameter.ParameterType) + " " + methodInfo.Name + "(" + parmnames + ")";
	}

	string PrettifyTypes(Type[] types) {
		if (types == null) throw new ArgumentNullException("types");
		return string.Join(", ", types.Select(x => GetTypeName(x)).ToArray());
	}

	MethodInfo GetMethod(object target, string methodName, Type[] types) {
		MethodInfo activeMethod = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, null, CallingConventions.Any, types, null);
		return activeMethod;
	}

	private Type[] GetArgTypes(SerializedProperty argProp) {
		Type[] types = new Type[argProp.arraySize];
		for (int i = 0; i < argProp.arraySize; i++) {
			types[i] = Arg.RealType((Arg.ArgType) argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex);
		}
		return types;
	}

	private void SetMethod(SerializedProperty property, MethodInfo methodInfo, bool dynamic) {
		SerializedProperty methodProp = property.FindPropertyRelative("_methodName");
		methodProp.stringValue = methodInfo.Name;
		SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
		dynamicProp.boolValue = dynamic;
		SerializedProperty argProp = property.FindPropertyRelative("_args");
		ParameterInfo[] parameters = methodInfo.GetParameters();
		argProp.arraySize = parameters.Length;
		for (int i = 0; i < parameters.Length; i++) {
		argProp.FindPropertyRelative("Array.data[" + i + "].argType").enumValueIndex = (int) Arg.FromRealType(parameters[i].ParameterType);
		}
		property.serializedObject.ApplyModifiedProperties();
		property.serializedObject.Update();
	}

	private static string GetTypeName(Type t) {
		if (t == typeof(int)) return "int";
		else if (t == typeof(float)) return "float";
		else if (t == typeof(string)) return "string";
		else if (t == typeof(bool)) return "bool";
		else if (t == typeof(void)) return "void";
		else return t.Name;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		float lineheight = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
		SerializedProperty targetProp = property.FindPropertyRelative("_target");
		SerializedProperty argProps = property.FindPropertyRelative("_args");
		SerializedProperty dynamicProp = property.FindPropertyRelative("_dynamic");
		float height = lineheight;
		if (targetProp.objectReferenceValue != null) {
			height += lineheight;
			if (!dynamicProp.boolValue) height += argProps.arraySize * lineheight;
		}
		height += 8;
		return height;
	}

	private static SerializableCallbackBase GetDummyFunction(SerializedProperty prop) {
		string stringValue = prop.FindPropertyRelative("_typeName").stringValue;
		Type type = Type.GetType(stringValue, false);
		SerializableCallbackBase result;
		if (type == null) {
			result = new SerializableCallback();
		} else {
			result = (Activator.CreateInstance(type) as SerializableCallbackBase);
		}
		return result;
	}
}