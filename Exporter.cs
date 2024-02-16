using AssetStudio;

namespace _7dsgcAnimExporter
{
    public static class Exporter
    {
        private const string BodyModelNamePattern = "hero_{0}_body_{1}";  // Bodies game object names always have this pattern. {0} = character name, {1} = body number

        private const string HeadModelNamePattern = "hero_{0}_head_{1}";  // Heads game object names always have this pattern. {0} = character name, {1} = head number

        private static readonly AssetsManager AssetsManager = new AssetsManager();

        public static void ExportFromCharacterBundles(CharacterBundles characterBundles, string outputFolder)
        {
            Animator? bodyModel = null, headModel = null;  // Will contain the body and head models
            List<AnimationClip> animationList = new List<AnimationClip>();  // Will store the animations
            AssetsManager.LoadFiles(characterBundles.FixedBundlesPath.ToArray());
            foreach (SerializedFile file in AssetsManager.assetsFileList)
            {
                foreach (ObjectInfo obj in file.m_Objects)
                {
                    ObjectReader objectReader = new ObjectReader(file.reader, file, obj);
                    if (objectReader.type == ClassIDType.GameObject)
                    {
                        GameObject gameObject = new GameObject(objectReader);
                        string gameObjectFileName = Path.GetFileName(gameObject.m_Name);
                        if (bodyModel == null && gameObjectFileName == String.Format(BodyModelNamePattern, characterBundles.CharacterName, characterBundles.BodyNumber))
                            bodyModel = FindAnimator(file, gameObject);
                        else if (headModel == null && gameObjectFileName == String.Format(HeadModelNamePattern, characterBundles.CharacterName, characterBundles.HeadNumber))
                            headModel = FindAnimator(file, gameObject);
                    }
                    else if (objectReader.type == ClassIDType.AnimationClip)
                        animationList.Add(new AnimationClip(objectReader));
                }
            }
            Console.WriteLine($"\nBody found : {bodyModel != null}.");
            Console.WriteLine($"Head found : {headModel != null}.");
            Console.WriteLine($"Animations found : {animationList.Count}.");
            if (bodyModel == null || headModel == null || animationList.Count == 0)
            {
                Console.WriteLine("Some assets could not be found...\nThe body or head doesn't exist, or there is no animation.");
                return;
            }
            for (int i = 0; i < animationList.Count; i++)
            {
                AnimationClip animation = animationList[i];
                // Prepend i+1 before animation.m_Name in case there are several animations with the same name
                ExportAnimatedModel(bodyModel, animation, Path.Combine(outputFolder, characterBundles.CharacterName, $"{i}{animation.m_Name}", $"{String.Format(BodyModelNamePattern, characterBundles.CharacterName, characterBundles.BodyNumber)}.fbx"));
                ExportAnimatedModel(headModel, animation, Path.Combine(outputFolder, characterBundles.CharacterName, $"{i}{animation.m_Name}", $"{String.Format(HeadModelNamePattern, characterBundles.CharacterName, characterBundles.HeadNumber)}.fbx"));
                Console.Write($"\rExported {i + 1}/{animationList.Count} animations.");
            }
            Console.WriteLine();
            AssetsManager.Clear();
        }

        // Finds the animator referenced in gameObject inside file
        private static Animator? FindAnimator(SerializedFile file, GameObject gameObject)
        {
            foreach (var component in gameObject.m_Components)
            {
                if (file.ObjectsDic[component.m_PathID].type == ClassIDType.Animator)
                {
                    return new Animator(file.ObjectsDic[component.m_PathID].reader);
                }
            }
            return null;
        }

        // Exports the animated model in outputPath 
        private static void ExportAnimatedModel(Animator model, AnimationClip animation, string outputPath)
        {
            ModelConverter headModel = new ModelConverter(model, ImageFormat.Png, new AnimationClip[] { animation });
            ModelExporter.ExportFbx(outputPath, headModel, true, (float)0.25, true, true, true, true, false, 10, false, (float)1, 3, false);
        }
    }
}
