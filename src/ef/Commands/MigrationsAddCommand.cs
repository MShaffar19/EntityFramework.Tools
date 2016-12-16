﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using Microsoft.EntityFrameworkCore.Tools.Properties;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class MigrationsAddCommand : ContextCommandBase
    {
        protected override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(_name.Value))
            {
                throw new CommandException(string.Format(Resources.MissingArgument, _name.Name));
            }
        }

        protected override int Execute()
        {
            var files = CreateExecutor().AddMigration(_name.Value, _outputDir.Value(), Context.Value());

            if (_json.HasValue())
            {
                ReportJson(files);
            }
            else
            {
                Reporter.WriteInformation("Done. To undo this action, use 'dotnet ef migrations remove'");
            }

            return base.Execute();
        }

        private static void ReportJson(IDictionary files)
        {
            Reporter.WriteData("{");
            Reporter.WriteData("  \"migrationFile\": \"" + Json.Escape(files["MigrationFile"] as string) + "\",");
            Reporter.WriteData("  \"metadataFile\": \"" + Json.Escape(files["MetadataFile"] as string) + "\",");
            Reporter.WriteData("  \"snapshotFile\": \"" + Json.Escape(files["SnapshotFile"] as string) + "\"");
            Reporter.WriteData("}");
        }
    }
}
