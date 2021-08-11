using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using Semver;

namespace git_tagged_semver
{
    class Program
    {
        static void Main(string[] args)
        {
            var dirPath = args.FirstOrDefault() ?? Directory.GetCurrentDirectory();
            var repoPath = Repository.Discover(dirPath);
            if (repoPath is null) ExitWithError(Error.NotARepository);
            using var repo = new Repository(repoPath);
            
            var headCommit = repo.Head.Tip;
            if(headCommit is null) ExitWithError(Error.NoHeadCommit);
            
            string describeString = null;

            try
            {
                describeString = repo.Describe(
                    headCommit, 
                    new DescribeOptions
                    {
                        AlwaysRenderLongFormat = true,
                        Strategy = DescribeStrategy.Tags
                    });
            }
            catch (LibGit2SharpException)
            {
                ExitWithError(Error.NoReferenceFound);
            }
            
            if(describeString is null)
                ExitWithUnexpectedError();

            describeString = Regex.Replace(describeString, @"-(\d*-[a-z0-9]{8})$", "+$1");

            if(!SemVersion.TryParse(describeString, out var semver))
                ExitWithError(Error.InvalidFormat(describeString));
            
            Console.Out.WriteLine(semver.ToString());
        }

        private static void ExitWithError(Error error)
        {
            Console.Error.WriteLine($"Error: {error.Message}");
            Environment.Exit(error.Code);
        }

        private static void ExitWithUnexpectedError()
        {
            Console.Error.WriteLine("An unexpected error occured");
            Environment.Exit(-1);
        }
    }
}
