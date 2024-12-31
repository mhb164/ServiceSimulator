namespace ServiceSimulator
{
    public class OperationInfo(string request, string responseRelativeFilename)
    {
        public readonly string Request = request ?? string.Empty;
        public readonly string ResponseRelativeFilename = responseRelativeFilename ?? string.Empty;
    }
}
