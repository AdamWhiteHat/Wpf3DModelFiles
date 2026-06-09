using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wpf3DModelFiles;

namespace TestWpf3DModelFiles
{
    [STATestClass]
    public sealed class TestXaml
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestSave()
        {
            FileInfo inputFileInfo = new FileInfo("TestFiles\\Pentahedron.3mf");
            FileInfo outputFileInfo = new FileInfo( Path.GetFullPath("WpfView.xaml"));

            List<CommonFileData> commonFileDatas = _3MFFile.Load(inputFileInfo);

            Assert.IsTrue(commonFileDatas.Any(), $"_3MFFile.Load: \"{Path.GetFileName(inputFileInfo.FullName)}\"");

            CommonFileData commonFileData = commonFileDatas.First();

            string outputFileData =  commonFileData.ToXamlString();
            File.WriteAllText(outputFileInfo.FullName, outputFileData);

            Assert.IsTrue(outputFileInfo.Exists, "XAML File Exists");
            Assert.IsTrue(outputFileInfo.Length > 0, "XAML File Length");
        }
    }
}
