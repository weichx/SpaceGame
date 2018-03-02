using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using Freespace.POFModel;
using Freespace.POFModel.Geometry;
using SpaceGame.Util;
using TreeEditor;
using UnityEngine;

namespace Freespace {

    public class Importer {

        public class ScaleModelPopup : EditorWindow {

            [MenuItem("Assets/Scale Selected Prefab")]
            static void Init() {
                ScaleModelPopup window = CreateInstance<ScaleModelPopup>();
                window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 250, 150);
                window.ShowPopup();
            }

            private float scale;
            private void OnGUI() {
                EditorGUILayout.LabelField("Scale selected model?", EditorStyles.wordWrappedLabel);
                GUILayout.Space(70);
                
                scale = EditorGUILayout.FloatField(scale);
                
                if (GUILayout.Button("Scale it!") && scale > 0) {
                    if (Selection.activeGameObject != null) {
                        GameObject selected = Selection.activeGameObject;
                        bool isPrefab = PrefabUtility.GetPrefabParent(selected) == null && PrefabUtility.GetPrefabObject(selected) != null;
                        UnityEngine.Object parentObject = PrefabUtility.FindPrefabRoot(selected);
                        string path = AssetDatabase.GetAssetPath(parentObject);

                        ((GameObject) parentObject).SetActive(true);
                        if (path != null && isPrefab) {
                            MeshFilter[] filters = selected.GetComponentsInChildren<MeshFilter>();

                            for (int i = 0; i < filters.Length; i++) {
                                MeshFilter filter = filters[i];
                                Mesh mesh = filter.sharedMesh;
                                string meshPath = AssetDatabase.GetAssetPath(mesh);
                                EditorUtility.DisplayProgressBar("Scaling", "Scaling " + meshPath, (i / (float) filters.Length));
                                Vector3[] vertices = new Vector3[mesh.vertices.Length];

                                for (int j = 0; j < mesh.vertices.Length; j++) {
                                    vertices[j] = mesh.vertices[j] * scale;
                                }

                                mesh.vertices = vertices;
                                mesh.RecalculateBounds();
                            }

                            Transform transform = selected.transform;
                            transform.Traverse((t) => {
                                t.position *= scale;
                                BoxCollider[] boxColliders = t.GetComponents<BoxCollider>();
                                if (boxColliders != null) {
                                    for (int i = 0; i < boxColliders.Length; i++) {
                                        boxColliders[i].size = boxColliders[i].size * scale;
                                    }
                                }
                                
                                SphereCollider[] sphereColliders = t.GetComponents<SphereCollider>();
                                if (sphereColliders != null) {
                                    for (int i = 0; i < sphereColliders.Length; i++) {
                                        SphereCollider sphereCollider = sphereColliders[i];
                                        sphereCollider.radius *= scale;
                                    }
                                }
                                CapsuleCollider[] capsuleColliders = t.GetComponents<CapsuleCollider>();
                                if (capsuleColliders != null) {
                                    for (int i = 0; i < capsuleColliders.Length; i++) {
                                        CapsuleCollider capsuleCollider = capsuleColliders[i];
                                        capsuleCollider.height *= scale;
                                        capsuleCollider.radius *= scale;
                                    }
                                }
                            });
                            
                            AssetDatabase.SaveAssets();
                            EditorUtility.ClearProgressBar();
                        }
                   
                    }
                    this.Close();
                }
                else if (GUILayout.Button("Nope")) {
                    this.Close();
                }
            }

        }

//        [MenuItem("Assets/Scale")]
        private static void ScaleMeshes() {
            string[] assets = AssetDatabase.FindAssets("t:mesh", new[] {"Assets/Freespace Assets/Medium Ships"});
            // float scale = 0.1f;
            float scale = 0.1f;

            for (int i = 0; i < assets.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
                EditorUtility.DisplayProgressBar("Shrinking", "Shrinking " + assetPath, (i / (float) assets.Length));
                Vector3[] vertices = new Vector3[mesh.vertices.Length];

                for (int j = 0; j < mesh.vertices.Length; j++) {
                    vertices[j] = mesh.vertices[j] * scale;
                }

                mesh.vertices = vertices;
                mesh.RecalculateBounds();
            }

            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Assets/Clean Imports")]
        private static void CleanDirectories() {
            const string path = "Freespace Prefabs/Medium Ships";
            const string assetPath = "Freespace Assets/Medium Ships/";

            string[] fileEntries = Directory.GetFiles(Application.dataPath + "/" + assetPath);

            foreach (string fileName in fileEntries) {
                int index = fileName.LastIndexOf("/");
                string localPath = "Assets/" + path + "/";

                if (index > 0)
                    localPath += Path.GetFileName(fileName);

                if (localPath.Contains("meta")) continue;

                GameObject t = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);

                if (t == null) {
                    string name = Path.GetFileNameWithoutExtension(localPath);

                    if (Path.GetExtension(localPath).Contains("meta")) {
                        name = Path.GetFileNameWithoutExtension(localPath);
                    }

                    string p = Application.dataPath + "/" + assetPath + name;
                    Directory.Delete(p, true);
                    Debug.Log("Deleted " + p);
                }
            }
        }

        [MenuItem("Assets/Import Freespace")]
        public static void ImportFreespace() {
            string path = EditorUtility.OpenFilePanel("POF file", "", "pof");

            if (string.IsNullOrEmpty(path)) return;
            Importer importer = new Importer(path, "Importing " + Path.GetFileNameWithoutExtension(path));

            try {
                importer.Import();
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Assets/Mass Import Freespace")]
        public static void MassImportFreespace() {
            string path = EditorUtility.OpenFolderPanel("POF Directory", "", "pof");
            if (string.IsNullOrEmpty(path)) return;

            string[] paths = Directory.GetFiles(path, "*.pof", SearchOption.AllDirectories);

            try {
                for (int i = 0; i < paths.Length; i++) {
                    try {
                        string countStr = "(" + i + "/" + paths.Length + ") ";
                        Importer importer = new Importer(paths[i],
                            "Importing " + countStr + Path.GetFileNameWithoutExtension(paths[i]));
                        importer.Import();
                    }
                    catch (Exception e) {
                        Debug.Log("Couldn't import " + paths[i] + ": " + e.Message);
                    }
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        private readonly HashSet<SubObject> processedSubObjects;
        private readonly Dictionary<int, Material> materialMap;
        private POFModel.POFModel model;
        private readonly string progressTitle;
        private readonly string originalFilePath;
        private readonly string modelName;
        private readonly string texturePath;
        private readonly string materialPath;
        private readonly string meshPath;
        private readonly string AssetRootPath;
        private readonly string PrefabRootPath;

        public Importer(string originalFilePath, string progressTitle = "Importing") {
            this.originalFilePath = originalFilePath;
            this.progressTitle = progressTitle;
            modelName = ImportUtil.CultureInfo.TextInfo.ToTitleCase(Path.GetFileNameWithoutExtension(originalFilePath)
                .ToLower());
            string importPath = GetImportPath();
            texturePath = Path.Combine(importPath, "Textures");
            materialPath = Path.Combine(importPath, "Materials");
            meshPath = Path.Combine(importPath, "Meshes");
            AssetRootPath = "Assets/Freespace Assets/" + ImportUtil.GetShipClassFromPath(originalFilePath) + "/" +
                            modelName;
            PrefabRootPath = ImportUtil.MakePath(new[]
                {Application.dataPath, "Freespace Prefabs", ImportUtil.GetShipClassFromPath(originalFilePath)});
            Directory.CreateDirectory(PrefabRootPath);
            materialMap = new Dictionary<int, Material>();
            processedSubObjects = new HashSet<SubObject>();
        }

        private bool Import() {
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetPrefabPath());

            ShowProgress("Parsing POF file", 0);

            model = POFModelParser.Parse(originalFilePath);

            ImportTextures();
            CreateMaterialAssets();
            CreateMeshAssets();
            CreatePrefabs(existingPrefab);

            return true;
        }

        private void ImportTextures() {
            ShowProgress("Copying Textures", 0.25f);
            string textureImportRoot = ImportUtil.GetTexturePathFromImportLocation(originalFilePath);

            Directory.CreateDirectory(texturePath);

            string[] modifiers = {"", "-glow", "-shine", "-normal"};

            // if we dont have the texture, and it does not already exist, copy it in

            for (int i = 0; i < model.TextureCount; i++) {
                for (int j = 0; j < modifiers.Length; j++) {
                    string[] pathSegments = {
                        textureImportRoot, model.GetTextureNameByIndex(i) + modifiers[j]
                    };
                    string pathToTexture = ImportUtil.MakePath(pathSegments);
                    string textureName = model.GetTextureNameByIndex(i) + modifiers[j];

                    if (ImportUtil.TryToImportTexture(pathToTexture, GetTextureAssetPath(textureName))) {
                        break;
                    }
                }
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        private void CreateMaterialAssets() {
            ShowProgress("Creating Materials", 0.25f);

            Shader standardShader = Shader.Find("Standard");

            Directory.CreateDirectory(materialPath);

            for (int i = 0; i < model.TextureCount; i++) {
                string materialString = "(" + i + "/" + model.TextureCount + ")";
                float progress = (i / (float) model.TextureCount);
                ShowProgress("Creating Materials " + materialString, progress);

                string textureName = model.GetTextureNameByIndex(i);
                Texture2D diffuse = ImportUtil.GetTexture(AssetRootPath, textureName, string.Empty);
                Texture2D shine = ImportUtil.GetTexture(AssetRootPath, textureName, "-shine");
                Texture2D glow = ImportUtil.GetTexture(AssetRootPath, textureName, "-glow");
                //Texture2D normal = ImportUtil.GetTexture(AssetRootPath, textureName, "-glow");

                //todo -- convert normal maps to proper format

                Material material = new Material(standardShader);

                material.SetColor("_EmissionColor", Color.white);
                material.SetTexture("_MainTex", diffuse);
//                material.SetTexture("_NormalMap", normal);
                material.SetTexture("_EmissionMap", glow);
                material.SetTexture("_MetallicGlossMap", shine);
                materialMap.Add(i, material);
                AssetDatabase.CreateAsset(material, GetMaterialPath(textureName));
            }
        }

        private void CreateMeshAssets() {
            Directory.CreateDirectory(meshPath);
            List<SubObject> subObjects = model.subObjects;

            for (int i = 0; i < subObjects.Count; i++) {
                float progress = (i / (float) subObjects.Count);
                string meshCount = "(" + (i + 1) + "/" + subObjects.Count + ")";
                ShowProgress("Creating mesh " + meshCount, progress);

                KeyValuePair<Mesh, int[]> pair = new GeometryParser(subObjects[i].bspData).GetMeshAndTextureIndices();
                Mesh mesh = pair.Key;
                subObjects[i].mesh = pair.Key;
                subObjects[i].textureIndices = pair.Value;

                string assetPath = GetMeshPath(subObjects[i].submodelName);
                Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

                if (!(existing != null && existing.triangles.Length == mesh.triangles.Length &&
                      existing.vertexCount == mesh.vertexCount && existing.subMeshCount == mesh.subMeshCount &&
                      existing.uv.Length == mesh.uv.Length && existing.normals.Length == mesh.normals.Length)) {
                    AssetDatabase.CreateAsset(subObjects[i].mesh, assetPath);
                }
            }

            AssetDatabase.SaveAssets();
        }

        private void ShowProgress(string description, float progress) {
            EditorUtility.DisplayProgressBar(progressTitle, description, progress);
        }

        private string GetImportPath() {
            string rootPath = Path.Combine(Application.dataPath, "Freespace Assets");
            string classPath = Path.Combine(rootPath, ImportUtil.GetShipClassFromPath(originalFilePath));
            return Path.Combine(classPath, modelName);
        }

        private string GetTextureAssetPath(string textureName) {
            return Path.Combine(texturePath, Path.GetFileName(textureName));
        }

        private string GetPrefabPath() {
            return "Assets/Freespace Prefabs/" + ImportUtil.GetShipClassFromPath(originalFilePath) + "/" + modelName +
                   ".prefab";
        }

        private string GetMeshPath(string meshName) {
            return AssetRootPath + "/Meshes/" + meshName + ".asset";
        }

        private string GetMaterialPath(string materialName) {
            return AssetRootPath + "/Materials/" + materialName + ".mat";
        }

        private GameObject CreateTurretAssets() {
            List<TurretInfo> turrets = model.turrets;
            if (turrets.Count == 0) return null;
            GameObject turretRoot = new GameObject("Turrets");

            for (int i = 0; i < turrets.Count; i++) {
                TurretInfo turret = turrets[i];
                SubObject obj = model.GetSubObjectByIndex(turret.parentSubObjectIndex);
                GameObject turretObj = CreateRenderableGameObject(obj);

                if (turret.type == TurretType.Gun) {
                    turretObj.name = "[gun] " + turretObj.name;
                }
                else {
                    turretObj.name = "[missile] " + turretObj.name;
                }

                BoxCollider collider = turretObj.AddComponent<BoxCollider>();
                collider.size = obj.boundingBoxMax - obj.boundingBoxMin;
                turretObj.transform.parent = turretRoot.transform;
                turretObj.transform.localPosition = obj.offset;
                Turret turretComponent = turretObj.AddComponent<Turret>();
                turretComponent.normal = turret.turretNormal;
                turretComponent.firingPoints = turret.firingPoints;

                List<SubObject> children = GetChildSubObjects(obj);

                for (int j = 0; j < children.Count; j++) {
                    SubObject child = children[j];
                    GameObject turretChild = CreateRenderableGameObject(child);
                    turretChild.transform.parent = turretObj.transform;
                    turretChild.transform.localPosition = new Vector3(); // not offset for some reason 
                }
            }

            return turretRoot;
        }

        private GameObject CreateRenderableGameObject(SubObject obj) {
            GameObject root = new GameObject(obj.submodelName);
            MeshFilter meshFilter = root.AddComponent<MeshFilter>();
            meshFilter.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(GetMeshPath(obj.submodelName));
            Renderer renderer = root.AddComponent<MeshRenderer>();
            Material[] materials = new Material[obj.textureIndices.Length];
            processedSubObjects.Add(obj);

            for (int i = 0; i < obj.textureIndices.Length; i++) {
                Material material;

                if (materialMap.TryGetValue(obj.textureIndices[i], out material)) {
                    int textureIndex = obj.textureIndices[i];
                    string materialName = model.GetTextureNameByIndex(textureIndex);
                    materials[i] = AssetDatabase.LoadAssetAtPath<Material>(GetMaterialPath(materialName));
                }
            }

            renderer.sharedMaterials = materials;
            return root;
        }

        private void CreatePrefabs(GameObject existingPrefab) {
            ShowProgress("Creating Prefabs", 0.9f);

            GameObject assetRoot = new GameObject(modelName);

            //todo instead of replacing existing prefabs, just copy the diffs so we don't break connections
            //todo overrides predefined (optionally) in a json file per model
            //override would attach components, move things to/from extras, gen paths, whatever else needs doing that 
            //automatic importing can't handle
            GameObject turretRoot = CreateTurretAssets();
            GameObject models = CreateLODModels();
            GameObject debris = CreateDebrisModels();
            GameObject shield = CreateShieldModel();
            GameObject thrusterRoot = CreateThrusters();
            GameObject gunpointRoot = CreateGunPoints();
            GameObject missilePointRoot = CreateMissilePoints();
            GameObject extras = CreateExtras();

            if (models != null) models.transform.parent = assetRoot.transform;
            if (turretRoot != null) turretRoot.transform.parent = assetRoot.transform;
            if (debris != null) debris.transform.parent = assetRoot.transform;
            if (shield != null) shield.transform.parent = assetRoot.transform;
            if (thrusterRoot != null) thrusterRoot.transform.parent = assetRoot.transform;
            if (gunpointRoot != null) gunpointRoot.transform.parent = assetRoot.transform;
            if (missilePointRoot != null) missilePointRoot.transform.parent = assetRoot.transform;
            if (extras != null) extras.transform.parent = assetRoot.transform;

            if (existingPrefab) {
                PrefabUtility.ReplacePrefab(assetRoot, existingPrefab);
            }
            else {
                string p = GetPrefabPath();
                PrefabUtility.CreatePrefab(p, assetRoot);
            }

            UnityEngine.Object.DestroyImmediate(assetRoot);
            EditorUtility.ClearProgressBar();
        }

        private GameObject CreateLODModels() {
            GameObject retn = new GameObject("Detail Root");
            BoxCollider collider = retn.AddComponent<BoxCollider>();
            collider.size = model.GetBoundingBoxSize();

            for (int i = 0; i < model.DetailLevels; i++) {
                SubObject subObject = model.GetDetailLevel(i);
                GameObject go = CreateRenderableGameObject(subObject);
                go.transform.parent = retn.transform;
            }

            if (model.DetailLevels > 1) {
                LODGroup lodGroup = retn.AddComponent<LODGroup>();
                LOD[] lods = new LOD[model.DetailLevels];

                for (int i = 0; i < model.DetailLevels; i++) {
                    lods[i] = new LOD(1f / (i + 1), retn.transform.GetChild(i).GetComponents<Renderer>());
                }

                lodGroup.SetLODs(lods);
                lodGroup.RecalculateBounds();
            }

            return retn;
        }

        private GameObject CreateDebrisModels() {
            if (!model.HasDebris) {
                return null;
            }

            GameObject retn = new GameObject("Debris Root");

            for (int i = 0; i < model.DebrisCount; i++) {
                SubObject subObject = model.GetDebrisPiece(i);
                GameObject go = CreateRenderableGameObject(subObject);
                BoxCollider collider = go.AddComponent<BoxCollider>();
                collider.size = subObject.boundingBoxMax - subObject.boundingBoxMax;
                go.transform.parent = retn.transform;
                go.transform.localPosition = subObject.offset;
            }

            retn.SetActive(false);
            return retn;
        }

        private GameObject CreateShieldModel() {
            if (!model.HasShieldData) return null;

            GameObject shieldObj = new GameObject("Shield");
            Mesh mesh = new Mesh();

            Face[] shieldFaces = model.ShieldFaces;
            int[] triangles = new int[shieldFaces.Length * 3];
            int triangleCount = 0;

            for (int i = 0; i < shieldFaces.Length; i++) {
                Face face = shieldFaces[i];
                triangles[triangleCount++] = face.vertexIndices[0];
                triangles[triangleCount++] = face.vertexIndices[1];
                triangles[triangleCount++] = face.vertexIndices[2];
            }

            mesh.vertices = model.ShieldVertices;
            mesh.triangles = triangles;
            AssetDatabase.CreateAsset(mesh, AssetRootPath + "/Meshes/Shield.asset");
            MeshFilter filter = shieldObj.AddComponent<MeshFilter>();
            filter.mesh = mesh;
            MeshRenderer renderer = shieldObj.AddComponent<MeshRenderer>();
            renderer.enabled = false;
            return shieldObj;
        }

        //todo -- this needs to fleshed out more once weapons work.
        //probably dont want an object for each point. Probably want
        //to add a gunslot component here to contain this data instead.
        private GameObject CreateGunPoints() {
            if (!model.HasGunSlots) return null;
            GameObject retn = new GameObject("Gun Points");
            List<GunSlot> gunSlots = model.gunSlots;

            for (int i = 0; i < gunSlots.Count; i++) {
                GunSlot slot = gunSlots[i];

                for (int j = 0; j < slot.gunPoints.Length; j++) {
                    GameObject go = new GameObject("Gun Point " + i + " -- " + j);
                    PositionNormal gunPoint = slot.gunPoints[j];
                    go.transform.parent = retn.transform;
                    go.transform.position = gunPoint.point;
                    if (gunPoint.normal == Vector3.zero) gunPoint.normal = Vector3.forward;
                    go.transform.rotation = Quaternion.LookRotation(gunPoint.normal, Vector3.up);
                }
            }

            return retn;
        }

        //todo -- this needs to fleshed out more once weapons work.
        //probably dont want an object for each point. Probably want
        //to add a gunslot component here to contain this data instead.
        private GameObject CreateMissilePoints() {
            if (!model.HasMissileSlots) return null;
            GameObject retn = new GameObject("Missile Points");
            List<MissileSlot> missileSlots = model.missileSlots;

            for (int i = 0; i < missileSlots.Count; i++) {
                MissileSlot slot = missileSlots[i];

                for (int j = 0; j < slot.missilePoints.Length; j++) {
                    GameObject go = new GameObject("Missile Point " + i + " -- " + j);
                    PositionNormal missilePoint = slot.missilePoints[j];
                    go.transform.parent = retn.transform;
                    go.transform.position = missilePoint.point;
                    if (missilePoint.normal == Vector3.zero) missilePoint.normal = Vector3.forward;
                    go.transform.rotation = Quaternion.LookRotation(missilePoint.normal, Vector3.up);
                }
            }

            return retn;
        }

        //todo -- we are using this information for anything yet
        //todo -- create a ThursterGlow component and use normal/radius from glows for effects
        private GameObject CreateThrusters() {
            if (!model.HasThrusters) return null;
            GameObject retn = new GameObject("Thrusters");
            List<Thruster> thrusters = model.thrusters;

            for (int i = 0; i < thrusters.Count; i++) {
                Thruster thruster = thrusters[i];
                GameObject thrusterGo = new GameObject("Thruster " + i);
                thrusterGo.transform.parent = retn.transform;

                for (int j = 0; j < thruster.glows.Length; j++) {
                    ThrusterGlow glow = thruster.glows[j];
                    GameObject glowGo = new GameObject("Thruster Glow " + j);
                    glowGo.transform.parent = thrusterGo.transform;
                    glowGo.transform.position = glow.position;
                    glowGo.transform.rotation = Quaternion.LookRotation(glow.normal, Vector3.up);
                    //glowGo.AddComponent<ThrusterGlowPulse>();
                }
            }

            return retn;
        }

        private GameObject CreateExtras() {
            GameObject extras = new GameObject("Extras");
            List<SubObject> subObjects = model.subObjects;

            for (int i = 0; i < subObjects.Count; i++) {
                SubObject subObject = subObjects[i];

                if (processedSubObjects.Contains(subObject)) {
                    continue;
                }

                // ignore pilots for now
                if (subObject.submodelName.Contains("pilot")) {
                    processedSubObjects.Add(subObject);
                    continue;
                }

                // ignore destroyed turrets for now
                if (subObject.submodelName.Contains("-destroyed")) {
                    processedSubObjects.Add(subObject);
                    continue;
                }

                GameObject go = CreateRenderableGameObject(subObject);
                go.transform.parent = extras.transform;
                go.transform.localPosition = subObject.offset;
                if (subObject.properties.Length > 0) Debug.Log(subObject.properties);
            }

            return extras;
        }

        private List<SubObject> GetChildSubObjects(SubObject parent) {
            List<SubObject> retn = new List<SubObject>();
            List<SubObject> subObjects = model.subObjects;

            for (int i = 0; i < subObjects.Count; i++) {
                if (subObjects[i].submodelParent == parent.subModelNumber) {
                    retn.Add(subObjects[i]);
                }
            }

            return retn;
        }

    }

}