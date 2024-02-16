using System.Text;

namespace _7dsgcAnimExporter
{
    public class Program
    {
        public const string BundleDataFileName = "BundleData.bytes";

        public const string DirectoryFileName = "directory.txt";

        public const string OutputFolderName = "output";

        public const string CharacterNameFileName = "characters.txt";

        public const int MinModelNumber = 0;

        public const int MaxModelNumber = 9999;

        public static void Main(string[] args)
        {
            Console.WriteLine("The Seven Deadly Sins Grand Cross Animation exporter");
            if (!File.Exists(DirectoryFileName) || !File.Exists(BundleDataFileName))
            {
                Console.WriteLine($"Some files are missing...\nMake sure {DirectoryFileName} and {BundleDataFileName} exist in the executable directory.");
                return;
            }
            string bundlesDirectory = File.ReadAllText(DirectoryFileName);
            if (!Directory.Exists(bundlesDirectory))
            {
                Console.WriteLine($@"The folder {bundlesDirectory} does not exist.\nPaste in {DirectoryFileName} the path to the game m folder. (...\Game\7dsgc_Data\master\bm\m)");
                return;
            }
            BundleData[]? bundleArray = ReadBundleData(File.ReadAllBytes(BundleDataFileName));
            if (bundleArray == null)
            {
                Console.WriteLine($"The content of {BundleDataFileName} is invalid.");
                return;
            }
            //WriteAllCharacterName(bundleArray);
            string characterName = string.Empty, bodyNumber = string.Empty, headNumber = string.Empty;
            while (true)
            {
                AskModelInfo(ref characterName, ref bodyNumber, ref headNumber);
                CharacterBundles? characterBundles = CharacterBundles.GetCharacterBundles(characterName, bodyNumber, headNumber, bundleArray);
                if (characterBundles != null)
                {
                    if (!characterBundles.CreateFixedBundles(bundlesDirectory))
                    {
                        Console.WriteLine($"\nWarning, some bundles could not be fixed for character {characterName} body {bodyNumber} head {headNumber}.\nThe export may fail or some animation may be missing.");
                    }
                    Exporter.ExportFromCharacterBundles(characterBundles, OutputFolderName);
                    characterBundles.DeleteFixedBundles();
                }
            }
        }

        // The file is an array of BundleData with the length written at the beginning
        private static BundleData[]? ReadBundleData(byte[] bundleData)
        {
            BundleData[]? bundleArray = null;
            using (MemoryStream bundleDataStream = new MemoryStream(bundleData))
            {
                using (BinaryReader reader = new BinaryReader(bundleDataStream))
                {
                    try
                    {
                        int count = reader.ReadInt32();  // Reads the size of the array
                        bundleArray = new BundleData[count];
                        for (int i = 0; i < count; i++)
                            bundleArray[i] = new BundleData(reader);  // The constructor reads the row and advances the position of the reader
                    }
                    catch { return null; }
                }
            }
            return bundleArray;
        }

        // Asks user to enter the models they want until the input is valid
        private static void AskModelInfo(ref string characterName, ref string bodyNumber, ref string headNumber)
        {
            Console.Write("\nEnter the name of the character you're looking for : ");
            string? inCharacterName = Console.ReadLine();
            Console.Write("Enter the body number : ");
            string? inBodyNumber = Console.ReadLine();
            Console.Write("Enter the head number : ");
            string? inHeadNumber = Console.ReadLine();
            int num;
            if (inCharacterName == null || inBodyNumber == null || inHeadNumber == null ||
                !int.TryParse(inBodyNumber, out num) || num < MinModelNumber || num > MaxModelNumber ||
                !int.TryParse(inHeadNumber, out num) || num < MinModelNumber || num > MaxModelNumber)
            {
                Console.WriteLine("\nYou entered invalid info, here is an exemple :\nCharacter name : escanor_one\nBody number : 10\nHead number : 3\n*Models number must be within [0;9999]");
                AskModelInfo(ref characterName, ref bodyNumber, ref headNumber);
                return;
            }
            characterName = inCharacterName;
            // The body and head strings have 4 digits, the numbers are preceded by 0 if they are too small
            bodyNumber = "0000".Substring(inBodyNumber.Length) + inBodyNumber;
            headNumber = "0000".Substring(inHeadNumber.Length) + inHeadNumber;
        }

        // Write all the characters name to CharacterNameFileName
        private static void WriteAllCharacterName(BundleData[] bundleArray)
        {
            StringBuilder sb = new StringBuilder();
            string startPattern = "char_prf_";
            string endPattern = "_body_0001";
            foreach (BundleData bundle in bundleArray)
                if (bundle.Name.StartsWith(startPattern) && bundle.Name.EndsWith(endPattern))
                    sb.AppendLine(bundle.Name.Substring(startPattern.Length, bundle.Name.Length - startPattern.Length - endPattern.Length));
            File.WriteAllText(CharacterNameFileName, sb.ToString());
        }
    }
}