using System.Text;

namespace FSGSave
{
    public class FSGSaveSection
    {
        public const string Magic = "!FSGSAVE";

        public static byte[] MagicBytes { get => Encoding.ASCII.GetBytes(Magic); }

        public string Name { get; }

        public const int NameBufferLength = 0x20;

        public FSGSession[] Sessions { get; }

        public int SessionCount { get => Sessions.Length; }

        public int Version { get; }

        public long? Length { get; set; }

        public FSGSaveSection(string name, FSGSession[] sessions, int version)
        {
            Name = name;
            Sessions = sessions;
            Version = version;
        }

        public FSGSaveSection(string name, FSGSession[] sessions) : this(name, sessions, 1) { }

        public FSGSaveSection(string name, int sessionCount, int version)
            : this(name, new FSGSession[sessionCount], version) { }

        public FSGSaveSection(string name, int sessionCount) : this(name, sessionCount, 1) { }
    }
}
