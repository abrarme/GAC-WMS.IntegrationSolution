using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GAC.WMS.IntegrationSolution.Tests
{
    public class DirectoryConfig
    {
        public string Path { get; set; }
        public string Endpoint { get; set; }
    }

    public interface IFileProcessor
    {
        Task ProcessAsync(string filePath, string endpoint);
    }
        
    public interface IFileProcessorFactory
    {   
        IFileProcessor GetProcessor(string extension);
    }




}
