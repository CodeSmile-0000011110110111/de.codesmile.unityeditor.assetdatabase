﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using System;
using System.Diagnostics.CodeAnalysis;
using Object = UnityEngine.Object;

namespace CodeSmile.Editor
{
	public sealed partial class Asset
	{
		private Path m_AssetPath;
		private Object m_MainObject;
		private Object[] m_AssetObjects;

		public Object MainObject => m_MainObject;
		public Type MainObjectType => m_MainObject.GetType();
		public Path AssetPath => m_AssetPath;

		public Boolean IsDeleted => m_AssetPath == null && m_MainObject == null;

		[ExcludeFromCodeCoverage] public Boolean IsForeignAsset => Status.IsForeignAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsNativeAsset => Status.IsNativeAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsSubAsset => Status.IsSubAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsMainAsset => Status.IsMainAsset(m_MainObject);
		[ExcludeFromCodeCoverage] public Boolean IsLoaded => Status.IsLoaded(m_MainObject);

		private void InvalidateInstance()
		{
			m_AssetPath = null;
			m_MainObject = null;
			m_AssetObjects = null;
		}
	}
}
