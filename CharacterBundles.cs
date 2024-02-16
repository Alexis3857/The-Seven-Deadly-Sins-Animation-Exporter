using System.Text;

namespace _7dsgcAnimExporter
{
    public class CharacterBundles
    {
        public const string FixedFolderName = "fixed";  // Fixed bundles will be written in this folder

        public const string FakeHeaderSignature = "UnityArchive";  // All bundles have this signature at the beginning of the file

        public const int BundleLengthIndex = 0x37;  // Index in the bundles where the real bundle size can be read

        public const string BodyBundleNamePattern = "char_prf_{0}_body_{1}";  // Bodies bundles names always have this pattern. {0} = character name, {1} = body number

        public const string HeadBundleNamePattern = "char_prf_{0}_head_{1}";  // Heads bundles names always have this pattern. {0} = character name, {1} = head number

        public const string AnimationBundlePattern = "char_ani_{0}";  // Animations bundles names always have this pattern. {0} = character name

        private CharacterBundles(string characterName, string bodyNumber, string headNumber, BundleData bodyBundle, BundleData headBundle, List<BundleData> animationBundles)
        {
            CharacterName = characterName;
            BodyNumber = bodyNumber;
            HeadNumber = headNumber;
            BodyBundle = bodyBundle;
            HeadBundle = headBundle;
            AnimationBundles = animationBundles;
        }

        public string CharacterName { get; }

        public string BodyNumber { get; }

        public string HeadNumber { get; }

        public BundleData BodyBundle { get; }

        public BundleData HeadBundle { get; }

        public List<BundleData> AnimationBundles { get; }

        public List<string> FixedBundlesPath { get; } = new List<string>();

        // Returns an instance of CharacterBundles if the given character models exist, has animation and the bundles could be found and fixed
        public static CharacterBundles? GetCharacterBundles(string characterName, string bodyNumber, string headNumber, BundleData[] bundleArray)
        {
            BundleData? bodyBundle = null, headBundle = null;
            List<BundleData> animationBundles = new List<BundleData>();
            foreach (BundleData bundle in bundleArray)
            {
                if (bundle.Name.StartsWith(String.Format(BodyBundleNamePattern, characterName, bodyNumber)))
                    bodyBundle = bundle;
                if (bundle.Name.StartsWith(String.Format(HeadBundleNamePattern, characterName, headNumber)))
                    headBundle = bundle;
                if (bundle.Name.StartsWith(String.Format(AnimationBundlePattern, characterName)))
                    animationBundles.Add(bundle);
            }
            if (bodyBundle == null || headBundle == null || animationBundles.Count == 0)
            {
                Console.WriteLine($"Necessary bundles info could not be found for character {characterName} body {bodyNumber} head {headNumber}.");
                return null;
            }
            return new CharacterBundles(characterName, bodyNumber, headNumber, bodyBundle, headBundle, animationBundles);
        }

        // Tries to reads the bundles of this instance in bundleDirectory and fix them
        public bool CreateFixedBundles(string bundleDirectory)
        {
            bool allFixed = true;
            foreach (BundleData bundle in AnimationBundles)
                if (!FixBundle(bundleDirectory, bundle.Checksum))
                    allFixed = false;
            return FixBundle(bundleDirectory, BodyBundle.Checksum) && FixBundle(bundleDirectory, HeadBundle.Checksum) && allFixed;
        }

        // Fixes the bundle and writes it in FixedFolderName/bundleChecksum
        private bool FixBundle(string bundleDirectory, string bundleChecksum)
        {
            if (!Directory.Exists(FixedFolderName))
                Directory.CreateDirectory(FixedFolderName);
            string fixedPath = Path.Combine(FixedFolderName, bundleChecksum);  // The file name is the checksum of the bundle
            string bundlePath = Path.Combine(bundleDirectory, bundleChecksum);
            if (!File.Exists(bundlePath))  // If the file does not exist in the game folder
                return false;
            byte[] bundleContent = File.ReadAllBytes(bundlePath);
            // BundleLengthIndex + 4 because we read an int (size 4) at BundleLengthIndex
            if (bundleContent.Length < BundleLengthIndex + 4 || Encoding.UTF8.GetString(bundleContent, 0, FakeHeaderSignature.Length) != FakeHeaderSignature)
                return false;  // The bundle isn't valid
            int bundleSize = BitConverter.ToInt32(bundleContent, BundleLengthIndex);  // The length of the real bundle is stored in the fake header at bundleContent[BundleLengthIndex]
            File.WriteAllBytes(fixedPath, bundleContent[^bundleSize..].ToArray());  // Writes the last bundleSize bytes from bundleContent
            FixedBundlesPath.Add(fixedPath);
            return true;
        }

        // Deletes bundles that were precedently fixed by this instance
        public void DeleteFixedBundles()
        {
            foreach (string fixedBundlePath in FixedBundlesPath)
            {
                File.Delete(fixedBundlePath);
            }
        }
    }
}
