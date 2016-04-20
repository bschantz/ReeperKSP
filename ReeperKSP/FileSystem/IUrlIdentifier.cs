namespace ReeperKSP.FileSystem
{
    public interface IUrlIdentifier
    {
        string Url { get; }
        string Path { get; }
        int Depth { get; }
        string[] Parts { get; }
        UrlType Type { get; }

        string this[int i] { get; }
    }
}
