namespace git_tagged_semver
{
    public class Error
    {
        private Error(int code, string message = null)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; }
        public string Message { get; }

        public static Error NotARepository => new Error(100, "Not a git repository.");
        public static Error NoHeadCommit => new Error(101, "No head commit");
        public static Error NoReferenceFound => new Error(102, "No tag found in tree. Can't describe current head commit.");
        public static Error InvalidFormat(string versionString)
        {
            return new Error(200, $"Format of string \"{versionString}\" is not compliant with Semver 2.0.0. Please refer to https://semver.org/ for details");
        }
    }
}