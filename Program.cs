using System;
using System.IO;
using LibGit2Sharp;

namespace git_semver
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

        public static Error NotARepository => new Error(1000, "Not a git repository!");
        public static Error NoHeadCommit => new Error(2000, "Not head commit!");
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var repoPath = Repository.Discover(Directory.GetCurrentDirectory());
            if (repoPath is null) ExitWithError(Error.NotARepository);
            using var repo = new Repository(repoPath);

            var headCommit = repo.Head.Tip;
            if(headCommit is null) ExitWithError(Error.NoHeadCommit);
            
            var describeString = repo.Describe(
                headCommit, 
                new DescribeOptions
                {
                    AlwaysRenderLongFormat = true,
                    Strategy = DescribeStrategy.Tags
                });
                
            Console.WriteLine(describeString);
        }

        private static void ExitWithError(Error error)
        {
            Console.Error.WriteLine(error.Message);
            Environment.Exit(error.Code);
        }
    }
}
