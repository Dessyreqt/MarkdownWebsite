namespace MarkdownWebsite.Features.Website
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using CommandLine;
    using HandlebarsDotNet;
    using Markdig;
    using MediatR;
    using Serilog;

    [Verb("website", HelpText = "Processes Handlebars and Markdown files in a folder and produces a website")]
    public class Request : IRequest
    {
        [Option('i', "input", HelpText = "The path to the input folder.")]
        public string Input { get; set; }

        [Option('o', "output", HelpText = "The path to the output folder.")]
        public string Output { get; set; }

        [Option('l', "layout", HelpText = "The path to the layout folder.")]
        public string Layout { get; set; }
    }

    public class Handler : RequestHandler<Request>
    {
        protected override void Handle(Request request)
        {
            if (Directory.Exists(request.Output))
            {
                Log.Information("Deleting {OutputFolder}...", request.Output);
                Directory.Delete(request.Output, true);
            }

            if (!string.IsNullOrWhiteSpace(request.Layout))
            {
                var layoutAssetsPath = $"{request.Layout}\\assets";
                if (Directory.Exists(layoutAssetsPath))
                {
                    Log.Information("Copying Assets...", request.Output);
                    var outputAssetsPath = $"{request.Output}\\assets";
                    Directory.CreateDirectory(outputAssetsPath);

                    var assetFiles = Directory.GetFiles(layoutAssetsPath, "*.*", SearchOption.AllDirectories);
                    foreach (var assetFile in assetFiles)
                    {
                        var relativePath = Path.GetRelativePath(request.Layout, Path.GetDirectoryName(assetFile));

                        if (relativePath == ".")
                        {
                            relativePath = string.Empty;
                        }

                        var outputDirectory = Path.Combine(request.Output, relativePath);

                        if (!Directory.Exists(outputDirectory))
                        {
                            Directory.CreateDirectory(outputDirectory);
                        }

                        var filename = Path.GetFileName(assetFile);

                        File.Copy(assetFile, Path.Combine(outputDirectory, filename), true);
                    }
                }

                var partialsDirectory = $"{request.Layout}\\partials";
              
                if (Directory.Exists(partialsDirectory))
                {
                    Log.Information("Registering Partials...");
                    var partials = Directory.GetFiles(partialsDirectory, "*.hbs");

                    foreach (var partial in partials)
                    {
                        var partialName = Path.GetFileNameWithoutExtension(partial);
                        string partialText = "";

                        using (var reader = new StreamReader(partial))
                        {
                            partialText = reader.ReadToEnd();
                        }

                        Handlebars.RegisterTemplate(partialName, partialText);
                    }
                }
            }

            var inputFiles = Directory.GetFiles(request.Input, "*.*", SearchOption.AllDirectories);

            foreach (var inputFile in inputFiles)
            {
                string inputText;
                var relativePath = Path.GetRelativePath(request.Input, Path.GetDirectoryName(inputFile));

                if (relativePath == ".")
                {
                    relativePath = string.Empty;
                }

                var outputDirectory = Path.Combine(request.Output, relativePath);

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                using (var reader = new StreamReader(inputFile))
                {
                    inputText = reader.ReadToEnd();
                }

                if (inputFile.EndsWith("md", StringComparison.InvariantCultureIgnoreCase))
                {
                    RegisterAssetsHelper(outputDirectory, Path.Combine(request.Output, "assets"));

                    var template = Handlebars.Compile(inputText);
                    var handlebarsMd = FixMarkdownLinks(template(inputText));
                    var html = Markdown.ToHtml(handlebarsMd);
                    var filename = Path.GetFileNameWithoutExtension(inputFile);
                    var outputFile = Path.Combine(outputDirectory, $"{filename}.html");

                    using (var writer = new StreamWriter(outputFile, false))
                    {
                        writer.Write(html);
                    }

                    Log.Information("{Input} -> {Output}", inputFile, outputFile);
                }
                else
                {
                    var filename = Path.GetFileName(inputFile);
                    var outputFile = Path.Combine(outputDirectory, filename);

                    File.Copy(inputFile, outputFile);
                    Log.Information("{Input} -> {Output}", inputFile, outputFile);
                }
            }
        }

        private string FixMarkdownLinks(string handlebarsMd)
        {
            return Regex.Replace(handlebarsMd, @"\[(.*?)\]\((.*?)\.md\)", "[$1]($2.html)");
        }

        private void RegisterAssetsHelper(string outputDirectory, string assetsDirectory)
        {
            var relativePathToAssets = Path.GetRelativePath(outputDirectory, assetsDirectory).Replace('\\', '/');

            Handlebars.RegisterHelper("asset", (writer, context, parameters) => {
                writer.WriteSafeString($"{relativePathToAssets}/{parameters[0]}");
            });
        }
    }
}
