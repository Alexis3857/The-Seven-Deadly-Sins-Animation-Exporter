namespace _7dsgcAnimExporter
{
    public class BundleData
    {
        public BundleData(BinaryReader reader)
        {
            Compressed = reader.ReadBoolean();
            Encrypt = reader.ReadBoolean();
            UseStreamingLoad = reader.ReadBoolean();
            Type = reader.ReadByte();
            CRC = reader.ReadUInt32();
            CRC16 = reader.ReadUInt16();
            Priority = reader.ReadInt32();
            Version = reader.ReadInt32();
            Size = reader.ReadInt64();
            Name = reader.ReadString();
            Variant = reader.ReadString();
            Checksum = reader.ReadString();
            int num = reader.ReadInt32();
            Assets = new string[num];
            for (int i = 0; i < num; i++)
            {
                Assets[i] = reader.ReadString();
            }
            num = reader.ReadInt32();
            Dependencies = new string[num];
            for (int i = 0; i < num; i++)
            {
                Dependencies[i] = reader.ReadString();
            }
            num = reader.ReadInt32();
            AutoDependencies = new string[num];
            for (int i = 0; i < num; i++)
            {
                AutoDependencies[i] = reader.ReadString();
            }
        }

        public bool Compressed { get; }

        public bool Encrypt { get; }

        public bool UseStreamingLoad { get; }

        public byte Type { get; }

        public int Priority { get; }

        public int Version { get; }

        public uint CRC { get; }

        public ushort CRC16 { get; }

        public long Size { get; }

        public string Name { get; }

        public string Variant { get; }

        public string Checksum { get; }

        public string[] Assets { get; }

        public string[] Dependencies { get; }

        public string[] AutoDependencies { get; }
    }
}
