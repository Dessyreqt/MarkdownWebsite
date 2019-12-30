namespace MarkdownWebsite.Features.Markdown
{
    using System.IO;
    using CommandLine;
    using Markdig;
    using MediatR;

    [Verb("markdown", HelpText = "Processes a Markdown file and outputs to HTML.")]
    public class Request : IRequest
    {
        [Option('i', "input", HelpText = "The path to the input file.")]
        public string Input { get; set; }

        [Option('o', "output", HelpText = "The path to the output file.")]
        public string Output { get; set; }
    }

    public class Handler : RequestHandler<Request>
    {
        protected override void Handle(Request request)
        {
            string inputText = "";

            using (var reader = new StreamReader(request.Input))
            {
                inputText = reader.ReadToEnd();
            }

            var outputDirectory = Path.GetDirectoryName(request.Output);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using var writer = new StreamWriter(request.Output, false);
            writer.Write(Markdown.ToHtml(inputText));
        }
    }
}