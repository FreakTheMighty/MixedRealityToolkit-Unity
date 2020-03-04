﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.TestTools;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public class GltfTests
    {
        const string relativePath = "Assets/MixedRealityToolkit.Examples/Demos/Gltf/Models/Avocado/glTF/AvocadoCustomAttr.gltf";

        private IEnumerator WaitForTask(Task task)
        {
            while (!task.IsCompleted) { yield return null; }
            if (task.IsFaulted) { throw task.Exception; }
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestGltfLoads()
        {
            // Load glTF
            string path = Path.GetFullPath(relativePath);
            var task = GltfUtility.ImportGltfObjectFromPathAsync(relativePath);

            yield return WaitForTask(task);

            GltfObject gltfObject = task.Result;

            Assert.IsNotNull(gltfObject);
            Assert.AreEqual(1, gltfObject.meshes.Length);
            Assert.AreEqual(1, gltfObject.nodes.Length);

            // Check if mesh variables have been set by attributes
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.uv.Length);
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.normals.Length);
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.tangents.Length);
            Assert.AreEqual(406, gltfObject.meshes[0].Mesh.vertexCount);
        }

        [UnityTest]
        public IEnumerator TestGltfCustomAttributes()
        {
            // Load glTF
            string path = Path.GetFullPath(relativePath);
            var task = GltfUtility.ImportGltfObjectFromPathAsync(relativePath);

            yield return WaitForTask(task);

            GltfObject gltfObject = task.Result;

            // Check for custom attribute
            int temperatureIdx;
            gltfObject.meshes[0].primitives[0].Attributes.TryGetValue("_TEMPERATURE", out temperatureIdx);

            int temperature = gltfObject.accessors[temperatureIdx].count;
            Assert.AreEqual(100, temperature);
        }

    }
}
#endif