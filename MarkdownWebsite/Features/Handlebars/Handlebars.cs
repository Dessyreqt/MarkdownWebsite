namespace MarkdownWebsite.Features.Handlebars
{
    using System.IO;
    using CommandLine;
    using HandlebarsDotNet;
    using MediatR;

    [Verb("handlebars", HelpText = "Processes Handlebars templates in file")]
    public class Request : IRequest
    {
        [Option('i', "input", HelpText = "The path to the input file.")]
        public string Input { get; set; }

        [Option('o', "output", HelpText = "The path to the output file.")]
        public string Output { get; set; }

        [Option('p', "partials", HelpText = "The path to the partials folder.")]
        public string Partials { get; set; }
    }

    public class Handler : RequestHandler<Request>
    {
        protected override void Handle(Request request)
        {
            if (!string.IsNullOrWhiteSpace(request.Partials))
            {
                var files = Directory.GetFiles(request.Partials, "*.hbs");

                foreach (var file in files)
                {
                    var partialName = Path.GetFileNameWithoutExtension(file);
                    string partialText = "";
                    
                    using (var reader = new StreamReader(file))
                    {
                        partialText = reader.ReadToEnd();
                    }

                    Handlebars.RegisterTemplate(partialName, partialText);
                } 
            }

            string inputText = "";

            using (var reader = new StreamReader(request.Input))
            {
                inputText = reader.ReadToEnd();
            }

            var template = Handlebars.Compile(inputText);
            var outputDirectory = Path.GetDirectoryName(request.Output);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using var writer = new StreamWriter(request.Output, false);
            writer.Write(template(inputText));
        }
    }
}