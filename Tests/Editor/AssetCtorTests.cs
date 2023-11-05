﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

public class AssetCtorTests : AssetTestBase
{
	[Test] public void AssetCtorCreate_NullObject_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset(null, (String)TestAssetPath));

	[Test] public void AssetCtorCreate_NullPath_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset(Instantiate.ExampleSO(), (String)null));

	[Test] public void AssetCtorCreate_ObjectAlreadyAnAsset_Throws()
	{
		var existing = CreateTestAsset(TestAssetPath);
		Assert.Throws<ArgumentException>(() => new Asset(existing, (String)TestAssetPath));
	}

	[Test] public void AssetCtorCreate_AssetExistsNoOverwrite_CreatesAssetWithUniqueName()
	{
		var testPath = TestAssetPath;
		var existing = CreateTestAsset(testPath);
		var expectedPath = Asset.Path.UniquifyFilename(testPath);
		var newObject = Instantiate.ExampleSO();

		var newAsset = DeleteAfterTest(new Asset(newObject, (String)TestAssetPath));

		Assert.True(Asset.Path.FileExists(testPath));
		Assert.True(Asset.Path.FileExists(expectedPath));
		Assert.AreEqual(expectedPath, newAsset.AssetPath);
		Assert.AreNotEqual(existing, newAsset.MainObject);
	}

	[Test] public void AssetCtorCreate_AssetExistsShouldOverwrite_ReplacesExistingAsset()
	{
		var testPath = TestAssetPath;
		var existing = CreateTestAsset(testPath);
		var expectedPath = Asset.Path.UniquifyFilename(testPath);
		var newObject = Instantiate.ExampleSO();

		var newAsset = DeleteAfterTest(new Asset(newObject, (String)TestAssetPath, true));

		Assert.True(Asset.Path.FileExists(testPath));
		Assert.False(Asset.Path.FileExists(expectedPath));
		Assert.AreEqual(testPath, newAsset.AssetPath);
		Assert.AreNotEqual(existing, newAsset.MainObject);
	}

	[Test] public void AssetCtorCreate_ObjectNotAnAssetAndValidPath_CreatesAsset()
	{
		var obj = DeleteAfterTest(Instantiate.ExampleSO());

		new Asset(obj, (String)TestAssetPath);

		Assert.True(TestAssetPath.ExistsInFileSystem);
	}

	[Test] public void AssetCtorCreate_NotExistingSubFoldersPath_CreatesFoldersAndAsset()
	{
		var obj = DeleteAfterTest(Instantiate.ExampleSO());

		new Asset(obj, (String)TestSubFoldersAssetPath);

		Assert.True(TestSubFoldersAssetPath.ExistsInFileSystem);
	}

	[Test] public void AssetCtorPath_Null_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset((Asset.Path)null));

	[Test] public void AssetCtorPath_NotExistingPath_Throws() =>
		Assert.Throws<FileNotFoundException>(() => new Asset("Assets/does not.exist"));

	[Test] public void AssetCtorPath_ExistingPath_Succeeds()
	{
		var assetObject = CreateTestAsset(TestAssetPath);

		var asset = new Asset(TestAssetPath);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.AreEqual(asset.AssetPath.Guid, Asset.Path.GetGuid(TestAssetPath));
	}

	[Test] public void AssetCtorObject_Null_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset((Object)null));

	[Test] public void AssetCtorObject_NotAnAsset_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset(Instantiate.ExampleSO()));

	[Test] public void AssetCtorObject_ExistingAsset_Succeeds()
	{
		var assetObject = CreateTestAsset(TestAssetPath);

		var asset = new Asset(assetObject);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.AreEqual(asset.AssetPath.Guid, Asset.Path.GetGuid(TestAssetPath));
	}

	[Test] public void AssetCtorGuid_EmptyGuid_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset(new GUID()));

	[Test] public void AssetCtorGuid_NotAnAsset_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset(GUID.Generate()));

	[Test] public void AssetCtorGuid_ExistingAsset_Succeeds()
	{
		var assetObject = CreateTestAsset(TestAssetPath);
		var guid = Asset.Path.GetGuid(TestAssetPath);

		var asset = new Asset(guid);

		Assert.True(asset.AssetPath == TestAssetPath);
		Assert.AreEqual(asset.MainObject, assetObject);
		Assert.True(asset.AssetPath.Guid.Equals(guid));
	}

	[Test] public void AssetCtorGuid_ExistingFolder_Succeeds()
	{
		var guid = Asset.Path.GetGuid("Assets");

		var asset = new Asset(guid);

		Assert.True(asset.AssetPath == "Assets");
		Assert.NotNull(asset.MainObject);
		Assert.AreEqual(asset.MainObject.GetType(), typeof(DefaultAsset));
		Assert.True(asset.AssetPath.Guid.Equals(guid));
	}
}
