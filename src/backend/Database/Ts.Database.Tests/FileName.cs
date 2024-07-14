using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelemetryStash.Database.Tests
{
    public class FileName
    {
        [Fact]
        public void Question7751186ff5()
        {
            var t = typeof(Ts.TelemetryDatabase.Sql.TelemetryDatabaseSql).Assembly;

            var ee = Path.GetRelativePath(t.Location, "C:\\Git\\telemetry-stash\\src\\backend\\Database\\Ts.TelemetryDatabase.Sql\\bin\\Debug\\Ts.TelemetryDatabase.Sql.dacpac");

            var uu = Path.GetFullPath(@"..\..\..\..\..\Ts.TelemetryDatabase.Sql\bin\Debug\Ts.TelemetryDatabase.Sql.dacpac");

            Assert.True(true);
        }
    }
}
