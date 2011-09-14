namespace WebGitNet
{
    using System;

    public class ConvertGlobFailedException : Exception
    {
        public ConvertGlobFailedException(string message)
            : base(message)
        {
        }
    }
}
