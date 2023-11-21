﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using Object = UnityEngine.Object;

public class AssetDeleteTests : AssetTestBase
{
	[Test] public void DeleteStatic_Null_DoesNotThrow()
	{
		Asset.File.Delete((String)null);
		Asset.File.Delete((Asset.Path)null);
		Asset.File.Delete((Object)null);
	}

	[Test] public void DeleteStatic_ExistingAssetObject_FileDeleted()
	{
		var obj = CreateTestAssetObject(TestAssetPath);
		Assert.True(TestAssetPath.ExistsInFileSystem);
		Assert.True(Asset.IsImported(obj));

		Asset.File.Delete(obj);

		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.False(Asset.IsImported(obj));
	}

	[Test] public void DeleteStatic_ExistingAssetPath_FileDeleted()
	{
		var obj = CreateTestAssetObject(TestAssetPath);
		Assert.True(TestAssetPath.ExistsInFileSystem);
		Assert.True(Asset.IsImported(obj));

		Asset.File.Delete(TestAssetPath);

		Assert.False(TestAssetPath.ExistsInFileSystem);
		Assert.False(Asset.IsImported(obj));
	}

	[Test] public void Delete_ExistingAssetObject_FileDeleted()
	{
		var asset = new Asset(CreateTestAssetObject(TestAssetPath));

		var deletedObj = asset.Delete();

		Assert.NotNull(deletedObj);
		Assert.Null(asset.AssetPath);
		Assert.False(Asset.IsImported(deletedObj));
		Assert.False(TestAssetPath.ExistsInFileSystem);
	}

	[Test] public void Delete_Again_Throws()
	{
		var asset = new Asset(CreateTestAssetObject(TestAssetPath));
		asset.Delete();

		Assert.Throws<InvalidOperationException>(() => asset.Delete());
	}

	[Test] public void Delete_SaveAfterDelete_Throws()
	{
		var asset = new Asset(CreateTestAssetObject(TestAssetPath));
		asset.Delete();

		Assert.Throws<InvalidOperationException>(() => asset.Save());
	}
}
