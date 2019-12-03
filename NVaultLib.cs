using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Navigation;

namespace nVault.NET
{
    public class NVaultLib
    {
        public List<VaultEntry> Entries { get; set; } = new List<VaultEntry>();
        public ushort Version { get; set; }

        public NVaultLib(string path)
        {
            var reader = new BinaryReader(File.Open(path, FileMode.Open));

            if (!reader.ReadChars(4).SequenceEqual(Utils.MagickChars))
                throw new Exception("Invalid nvault file.");

            Version = reader.ReadUInt16();
            var kvCount = reader.ReadInt32();
            for (int i = 0; i < kvCount; i++)
            {
                var timestamp = reader.ReadInt32();
                var keyLen = reader.ReadByte();

                if(keyLen==0)
                    throw new Exception("Entry key cannot be empty");
                var valueLen = reader.ReadUInt16();
                var key = reader.ReadChars(keyLen);
 
                var value = reader.ReadChars(valueLen);
                Entries.Add(new VaultEntry
                {
                    Key = new string(key),
                    Value = new string(value),
                    TimestampRaw = timestamp
                });
            }

            reader.Dispose();
        }

        public NVaultLib()
        {

        }

        public void Save(string path)
        {
            var writer = new BinaryWriter(File.OpenWrite(path));
            writer.Write(Utils.MagickChars);
            writer.Write(Version);
            writer.Write(Convert.ToInt32(Entries.Count));
            foreach (VaultEntry entry in Entries)
            {
                writer.Write(entry.TimestampRaw);
                writer.Write(Convert.ToByte(entry.Key.Length));
                writer.Write(Convert.ToUInt16(entry.Value.Length));
                writer.Write(Encoding.ASCII.GetBytes(entry.Key));
                writer.Write(Encoding.ASCII.GetBytes(entry.Value));
            }
            writer.Dispose();
        }
    }

    public class VaultEntry
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int TimestampRaw { get; set; }

        public DateTime Timestamp
        {
            get => DateTimeOffset.FromUnixTimeSeconds(TimestampRaw).UtcDateTime;
            set => TimestampRaw = Convert.ToInt32(new DateTimeOffset(value).ToUnixTimeSeconds());
        }
    }
}
