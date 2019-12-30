namespace MarkdownWebsite
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using MarkdownWebsite.Infrastructure;
    using MarkdownWebsite.Infrastructure.DependencyResolution;
    using MarkdownWebsite.Infrastructure.Logging;
    using CommandLine;
    using CommandLine.Text;
    using LightInject;
    using MediatR;

    class Program
    {
        static void Main(string[] args)
        {
            Logger.Initialize(new LoggerSettings());

            var watch = new Stopwatch();
            watch.Start();
            Logger.Instance.Information("Initializing...");

            var appState = InitializeState();
            var mediator = BuildMediator();
            var parser = new Parser(
                with =>
                {
                    with.EnableDashDash = true;
                    with.CaseSensitive = false;
                    with.HelpWriter = null;
                });

            watch.Stop();
            Logger.Instance.Information("Initialization Complete in {Elapsed:000}ms", watch.ElapsedMilliseconds);

            var types = appState.AvailableVerbs.ToArray();
            var parserResult = parser.ParseArguments(args, types);

            if (parserResult is Parsed<object> result)
            {
                mediator.Send((IRequest)result.Value);
            }
            else
            {
                DisplayHelp(parserResult, types);
                Environment.Exit(1);
            }
        }

        private static void DisplayHelp<T>(ParserResult<T> result, params Type[] types)
        {
            var helpText = HelpText.AutoBuild(
                result,
                h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    h.Heading = "MarkdownWebsite";
                    h.Copyright = "";

                    return HelpText.DefaultParsingErrorsHandler(result, h);
                },
                e => e);
            Console.WriteLine(helpText);
        }

        private static IMediator BuildMediator()
        {
            return IoC.Container.GetInstance<IMediator>();
        }

        private static AppState InitializeState()
        {
            var appState = IoC.Container.GetInstance<AppState>();

            appState.AvailableVerbs.Clear();

            var types = GetCommands();
            foreach (var type in types)
            {
                appState.AvailableVerbs.Add(type);
            }

            return appState;
        }

        private static IEnumerable<Type> GetCommands()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes().Where(type => typeof(IRequest).IsAssignableFrom(type));
        }
    }
}
