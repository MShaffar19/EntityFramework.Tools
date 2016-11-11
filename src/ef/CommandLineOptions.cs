﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Tools.Commands;
using Microsoft.Extensions.CommandLineUtils;

namespace Microsoft.EntityFrameworkCore.Tools
{
    public class CommandLineOptions
    {
        public ICommand Command { get; set; }
        public bool IsHelp { get; set; }
        public bool NoColor { get; set; }
        public bool Verbose { get; set; }
        public string EnvironmentName { get; set; }
        public string Assembly { get; set; }
        public string RootNamespace { get; set; }
        public string ContentRootPath { get; set; }
        public string ProjectDirectory { get; set; }
        public string DataDirectory { get; set; }
        public string StartupAssembly { get; set; }
        public bool NoAppDomain { get; set; }

        public static CommandLineOptions Parse(params string[] args)
        {
            var options = new CommandLineOptions();

            var app = new CommandLineApplication
            {
#if NET451
                Name = "ef",
#else
                Name = "ef",
#endif
                FullName = "Entity Framework Core Command Line Tools"
            };

            app.HelpOption();
            app.VersionOption(Program.GetVersion);
            var verbose = app.Option("--verbose", "Show verbose output", inherited: true);
            var noColor = app.Option("--no-color", "Do not use color in console output");
            // so "WriteLogo" gets the right option set before parsing the remainder of the args
            Reporter.NoColor = args.Any(a => a.Equals("--no-color"));

            var prefixOutput = app.Option("--prefix-output", "(internal flag) prefix output");
            prefixOutput.ShowInHelpText = false;
            Reporter.PrefixOutput = args.Any(a => a.Equals("--prefix-output"));

            // required
            var assembly = app.Option("--assembly <assembly>",
                "The assembly file to load.", inherited: true);
#if NET451
            var noAppDomain = app.Option("--no-appdomain",
                "Do not use app domains to execute the command");
#endif

            // common options
            var startupAssembly = app.Option("--startup-assembly <assembly>",
                "The assembly file containing the startup class.", inherited: true);
            var dataDirectory = app.Option("--data-dir <dir>",
                "The folder used as the data directory (defaults to current working directory).", inherited: true);
            var projectDirectory = app.Option("--project-dir <dir>",
                "The folder used as the project directory (defaults to current working directory).", inherited: true);
            var contentRootPath = app.Option("--content-root-path <dir>",
                "The folder used as the content root path for the application (defaults to application base directory).", inherited: true);
            var rootNamespace = app.Option("--root-namespace <namespace>",
                "The root namespace of the target project (defaults to the project assembly name).", inherited: true);
            var environment = app.Option(
                "-e|--environment <environment>",
                "The environment to use. If omitted, \"Development\" is used.", inherited: true);

            EfCommand.Configure(app, options);

            var result = app.Execute(args);

            if (result != 0)
            {
                return null;
            }

            options.IsHelp = app.IsShowingInformation;

            options.Verbose = verbose.HasValue();
            options.NoColor = noColor.HasValue();

            options.Assembly = assembly.Value();
            options.StartupAssembly = startupAssembly.Value();
            options.DataDirectory = dataDirectory.Value();
            options.ProjectDirectory = projectDirectory.Value();
            options.ContentRootPath = contentRootPath.Value();
            options.RootNamespace = rootNamespace.Value();
            options.EnvironmentName = environment.Value();
#if NET451
            options.NoAppDomain = noAppDomain.HasValue();
#endif

            return options;
        }
    }
}
