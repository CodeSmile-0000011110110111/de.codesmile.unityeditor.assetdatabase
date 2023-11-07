﻿// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using CodeSmile.Editor;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class AssetPathTests : AssetTestBase
{
	[Test] public void AssetPath_DefaultCtor_EqualsAssetsString() =>
		Assert.AreEqual("Assets", new Asset.Path().ToString());

	[Test] public void AssetPath_DefaultCtor_ImplicitConversionEqualsAssetsString() =>
		Assert.AreEqual("Assets", new Asset.Path());

	[Test] public void AssetPath_DefaultCtor_ExplicitConversionEqualsAssetsString()
	{
		Assert.AreEqual("Assets", (String)new Asset.Path());
		new Action<Asset.Path>(assetPath => Assert.AreEqual("Assets", assetPath)).Invoke((Asset.Path)"Assets");
	}

	[Test] public void AssetPath_NullString_Throws() =>
		Assert.Throws<ArgumentNullException>(() => new Asset.Path(null));

	[Test] public void AssetPath_EmptyString_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset.Path(String.Empty));

	[Test] public void AssetPath_WhitespaceString_Throws() =>
		Assert.Throws<ArgumentException>(() => new Asset.Path(" "));

	[TestCase("AssetsData")]
	[TestCase("SomeFolder/")]
	[TestCase("Some\\Folder\\")]
	public void AssetPath_RelativePathDoesNotStartWithAssets_Throws(String inputPath) =>
		Assert.Throws<ArgumentException>(() => new Asset.Path(inputPath));

	[Test] public void AssetPathExists_NotExistingFolderPath_False() =>
		Assert.IsFalse(new Asset.Path("Assets/doesnotexist").Exists);

	[Test] public void AssetPathExists_ExistingFolderPath_True() => Assert.IsTrue(new Asset.Path("Assets").Exists);

	[Test] public void AssetPathExists_NotExistingFilePath_False() =>
		Assert.IsFalse(new Asset.Path("Assets/doesnotexist.file").Exists);

	[Test] public void AssetPathExists_ExistingFilePath_True() =>
		Assert.IsTrue(new Asset.Path(Asset.Path.Get(CreateTestAssetObject())).Exists);

	[Test] public void AssetPathExistsInFileSystem_NotExistingFolderPath_False() =>
		Assert.IsFalse(new Asset.Path("Assets/doesnotexist").ExistsInFileSystem);

	[Test] public void AssetPathExistsInFileSystem_ExistingFolderPath_True() =>
		Assert.IsTrue(new Asset.Path("Assets").ExistsInFileSystem);

	[Test] public void AssetPathExistsInFileSystem_NotExistingFilePath_False() =>
		Assert.IsFalse(new Asset.Path("Assets/doesnotexist.file").ExistsInFileSystem);

	[Test] public void AssetPathExistsInFileSystem_ExistingFilePath_True() =>
		Assert.IsTrue(new Asset.Path(Asset.Path.Get(CreateTestAssetObject())).ExistsInFileSystem);

	[TestCase("Assets", "Assets")]
	[TestCase("assets", "assets")]
	[TestCase("Assets/", "Assets/")]
	[TestCase("Assets\\", "Assets/")]
	[TestCase("/Assets/folder", "Assets/folder")]
	[TestCase("\\Assets\\folder", "Assets/folder")]
	[TestCase("/Assets/folder\\", "Assets/folder")]
	[TestCase("\\Assets\\folder/", "Assets/folder")]
	public void AssetPath_WithRelativeAssetsPath_AsExpected(String inputPath, String expected) =>
		Assert.AreEqual(expected, new Asset.Path(inputPath));

	[TestCase("Packages/com.codesmile.assetdb/package.json", "Packages/com.codesmile.assetdb/package.json")]
	[TestCase("Packages/com.codesmile.assetdb/Tests/Editor/CodeSmile.Editor.Tests.AssetDB.asmdef",
		"Packages/com.codesmile.assetdb/Tests/Editor/CodeSmile.Editor.Tests.AssetDB.asmdef")]
	public void AssetPath_WithRelativePackagesPath_AsExpected(String inputPath, String expected) =>
		Assert.AreEqual(expected, new Asset.Path(inputPath));

	[TestCase("Assets", "file name", "Assets/file name.asset")]
	[TestCase("assets", "file name", "assets/file name.asset")]
	[TestCase("Assets/", "file name", "Assets/file name.asset")]
	[TestCase("Assets\\", "file name", "Assets/file name.asset")]
	[TestCase("/Assets/folder", "file name", "Assets/folder/file name.asset")]
	[TestCase("\\Assets\\folder", "file name", "Assets/folder/file name.asset")]
	[TestCase("/Assets/folder\\", "file name", "Assets/folder/file name.asset")]
	[TestCase("\\Assets\\folder/", "file name", "Assets/folder/file name.asset")]
	public void AssetPath_CombinePathAndFileName_AsExpected(String inputPath, String fileName,
		String expected) => Assert.AreEqual(expected, new Asset.Path(inputPath, fileName));

	[TestCase("\\Assets\\folder/", "fn", "text", "Assets/folder/fn.text")]
	public void AssetPath_CombinePathFileNameExtension_AsExpected(String inputPath, String fileName, String extension,
		String expected) => Assert.AreEqual(expected, new Asset.Path(inputPath, fileName, extension));

	[Test] public void AssetPath_WithFullAssetsPath_IsRelativePath()
	{
		var filePath = "/sub/FN.test";
		var fullPath = Application.dataPath + filePath;
		Assert.AreEqual("Assets" + filePath, new Asset.Path(fullPath));
	}

	[Test] public void AssetPath_WithFullPackagesPath_IsRelativePath()
	{
		var filePath = "/com.codesmile.assetdb/package.json";
		var fullPath = Asset.Path.FullPackagesPath + filePath;
		Assert.AreEqual("Packages" + filePath, new Asset.Path(fullPath));
	}

	[Test] public void AssetPath_WithFullPath_EqualsFullPath()
	{
		var fullPath = Application.dataPath + "/sub/FN.test";
		Assert.AreEqual(fullPath, new Asset.Path(fullPath).FullPath);
	}

	[Test] public void AssetPath_WithFullPathBackslashes_EqualsFullPath()
	{
		var fullPath = Application.dataPath + "/sub/FN.test";
		Assert.AreEqual(fullPath, new Asset.Path(fullPath.Replace('/', '\\')).FullPath);
	}

	[Test] public void AssetPath_CombineFullPath_MadeRelative() =>
		Assert.AreEqual("Assets/FN.test", new Asset.Path(Application.dataPath, "FN", ".test"));

	[TestCase("/filename")]
	[TestCase("filename/")]
	[TestCase("\\filename\\")]
	[TestCase("file/name")]
	[TestCase("file\\name")]
	public void AssetPath_CombineFileWithSeparators_Throws(String fileName) =>
		Assert.Throws<ArgumentException>(() => new Asset.Path("Assets", fileName));

	[TestCase("ext")] [TestCase(".ext")]
	public void AssetPath_CombineExtension_AsExpected(String extension) =>
		Assert.AreEqual("Assets/FN." + extension.TrimStart('.'), new Asset.Path("Assets", "FN", extension));

	[TestCase(null, "file name", "extension")]
	[TestCase("Assets", null, "extension")]
	[TestCase("Assets", "file name", null)]
	public void AssetPath_CombineNullStrings_Throws(String path, String filename, String extension) =>
		Assert.Throws<ArgumentNullException>(() => new Asset.Path(path, filename, extension));

	[TestCase(" ", "file name", "extension")]
	[TestCase("Assets", " ", "extension")]
	[TestCase("Assets", "file name", " ")]
	public void AssetPath_CombineWhitespaceStrings_Throws(String path, String filename, String extension) =>
		Assert.Throws<ArgumentException>(() => new Asset.Path(path, filename, extension));

	/// <summary>
	///     These paths do not point to the *current* project's root or subfolder.
	/// </summary>
	/// <param name="path"></param>
	[TestCase("C:\\Users\\Urso Clever\\Untiy Projects\\First Porject\\Assets\\folder")]
	[TestCase("\\Users\\Urso Clever\\Untiy Projects\\First Porject\\Assets\\folder")]
	public void AssetPath_NotAProjectPath_Throws(String path) =>
		Assert.Throws<ArgumentException>(() => new Asset.Path(path));

	[Test] public void FolderPath_WithAssetsFolder_SameAsInput()
	{
		var assetPath = new Asset.Path("Assets");

		Assert.AreEqual(assetPath, assetPath.FolderPath);
	}

	[Test] public void FolderPath_WithValidSubFoldersFilePath_ReturnsFolderOfTheFile()
	{
		var testPath = TestSubFoldersAssetPath;
		CreateTestAssetObject(testPath);

		var assetPath = new Asset.Path(testPath);

		Assert.AreEqual(TestSubFoldersPath, assetPath.FolderPath);
	}

	[Test] public void FolderPath_WithValidSubFoldersPath_SameAsInput()
	{
		var testPath = TestSubFoldersPath;
		var guid = Asset.Path.CreateFolders(testPath);

		var assetPath = new Asset.Path(testPath);

		Assert.AreEqual(testPath, assetPath.FolderPath);
		Assert.AreEqual(guid, assetPath.Guid);
	}

	[ExcludeFromCodeCoverage] // code coverage considers the 'fp' variable unused because property throws
	[Test] public void FolderPath_WithNonExistingPath_Throws()
	{
		var folderPath = "Assets/path/to/folder";
		Assert.Throws<InvalidOperationException>(() =>
		{
			var path = new Asset.Path(folderPath);
			var fp = path.FolderPath;
		});
		Assert.Throws<InvalidOperationException>(() =>
		{
			var path = new Asset.Path(folderPath + "/with.file");
			var fp = path.FolderPath;
		});
	}
}