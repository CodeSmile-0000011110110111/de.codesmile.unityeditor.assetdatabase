﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor;
using UnityEngine;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		/// <summary>
		///     Loads the main asset object at the path.
		///     Commonly this is the only object of the asset, but there are assets that
		///     consist of multiple objects such as Mesh assets that may contain for example animations and materials.
		/// </summary>
		/// <param name="path"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>The asset object or null if the path does not exist or the asset is not imported.</returns>
		public static T Load<T>(Path path) where T : Object
		{
			ThrowIf.ArgumentIsNull(path, nameof(path));
			ThrowIf.DoesNotExistInFileSystem(path);

			var assetType = MainType(path);
			ThrowIf.AssetNotImported(path, assetType);
			ThrowIf.AssetTypeMismatch<T>(path, assetType);

			var obj = (T)AssetDatabase.LoadMainAssetAtPath(path);
			ThrowIf.AssetLoadReturnedNull(obj, path); // this may not be necessary but just to be sure

			return obj;
		}

		/// <summary>
		///     Loads the main asset object for the guid.
		///     Commonly this is the only object of the asset, but there are assets that
		///     consist of multiple objects such as Mesh assets that may contain for example animations and materials.
		/// </summary>
		/// <param name="guid"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>The asset object or null if the guid is not an asset guid.</returns>
		public static T Load<T>(GUID guid) where T : Object
		{
			ThrowIf.NotAnAssetGuid(guid);
			return Load<T>(Path.Get(guid));
		}

		/// <summary>
		///     Tries to load the object at path. If it cannot be loaded, it will be created using the Object instance
		///     returned by the getObjectInstance Func callback.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="getInstance"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T LoadOrCreate<T>(Path path, System.Func<Object> getInstance) where T : Object
		{
			if (path.Exists)
				return Load<T>(path);

			var obj = getInstance.Invoke() as T;
			Create(obj, path);

			return obj;
		}

		/// <summary>
		///     Private on purpose: the main object is automatically loaded when instantiating an Asset class.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private T Load<T>() where T : Object => m_AssetPath != null ? (T)(m_MainObject = Load<T>(m_AssetPath)) : null;

		// public static T LoadFirst<T>(AssetPath assetPath) where T : Object =>
		// 	AssetDatabase.LoadAssetAtPath<T>(assetPath);
		//
		// public static Object[] LoadAll(AssetPath assetPath) => AssetDatabase.LoadAllAssetsAtPath(assetPath);
		//
		// public static Object[] LoadOnlyVisible(AssetPath assetPath) =>
		// 	AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);

		// public Object[] LoadAll() => SelectAndAssignMainObject(LoadAll(m_AssetPath));
		// public Object[] LoadOnlyVisible() => SelectAndAssignMainObject(LoadOnlyVisible(m_AssetPath));
		//
		// public Object LoadMainAsync(Action<Object> onLoadComplete)
		// {
		// 	// TODO: use coroutine to load async
		// 	Object obj = null;
		// 	onLoadComplete?.Invoke(obj);
		// 	return null;
		// }
		//
		// public Object LoadAsync(Int32 fileId, Action<Object> onLoadComplete)
		// {
		// 	// TODO: use coroutine to load async
		// 	Object obj = null;
		// 	onLoadComplete?.Invoke(obj);
		// 	return null;
		// }

		//
		// public Boolean OpenExternal(Int32 lineNumber = -1, Int32 columnNumber = -1) =>
		// 	// TODO: overload for object and instanceId
		// 	// TODO: check null
		// 	AssetDatabase.OpenAsset(MainObject, lineNumber, columnNumber);
		//
		// public void Import()
		// {
		// 	// TODO: check that path is valid
		// }
	}
}
