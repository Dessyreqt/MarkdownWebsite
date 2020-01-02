namespace MarkdownWebsite
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using CommandLine;
    using CommandLine.Text;
    using LightInject;
    using MarkdownWebsite.Infrastructure;
    using MarkdownWebsite.Infrastructure.DependencyResolution;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    class Program
    {
        static void Main(string[] args)
        {
            var configuration = IoC.Container.GetInstance<IConfiguration>();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            var watch = new Stopwatch();
            watch.Start();
            Log.Debug("Initializing...");

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
            Log.Debug("Initialization Complete in {Elapsed:000}ms", watch.ElapsedMilliseconds);

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
            Log.Information(helpText);
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
